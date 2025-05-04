# MAUI Plugin Demo

Dieses Repository enthält eine Demo-Anwendung auf Basis von **.NET MAUI** (Android & Windows), die zur Laufzeit ein externes UI-Plugin per Remote-Download lädt, initialisiert und in die App-Shell integriert. Zusätzlich wird eine Mikro-ORM-Lösung (Dapper + SQLite3) mit automatisierten Migrationen pro Modul gezeigt.

---

## Projektstruktur

* **ModuleContracts**

  * `IAppModule`, `IModuleHost`, `IModuleDatabase`
  * Definiert das Host–Modul-API für Plugin-Entwickler.

* **PluginModule**

  * Beispiel-Plugin als MAUI-Klassenbibliothek
  * `SampleUIPluginModule` implementiert `IAppModule`
  * `PluginPage` mit einfacher UI
  * **Embedded-SQL** unter `Migrations/*.sql` als Ressource

* **DemoMauiApp**

  * MAUI-App mit Shell und Home-Page
  * **Services**:

    * `ModuleHost`: Verwaltung der globalen SQLite-DB, Migration-Runner, Mikro-ORM (Dapper)
    * `ModuleManager`: Remote-ZIP-Download der Plugins (URL konfigurierbar)
    * `PluginLoader`: AssemblyLoadContext-basiertes Laden und Initialisieren der Module
  * `MainPage` mit Buttons zum Nachladen und Navigieren zum Plugin
  * AndroidManifest unter `Platforms/Android/AndroidManifest.xml`

---

## Voraussetzungen

* [.NET 6 SDK](https://dotnet.microsoft.com/download)
* MAUI-Workload installiert (`dotnet workload install maui`)
* Android SDK & Emulator oder verbundenes Gerät
* (Optional) Windows 10 SDK für Windows-Target

---

## Inbetriebnahme

### 1. Projekte bauen

```bash
# Contracts
dotnet build ModuleContracts/ModuleContracts.csproj

# Plugin
dotnet build PluginModule/PluginModule.csproj

# Demo App (Android)
dotnet build DemoMauiApp/DemoMauiApp.csproj -f net6.0-android
```

### 2. Plugin bereitstellen

1. Erzeuge ein ZIP mit dem Inhalt von `PluginModule/bin/Debug/net6.0-android/` inklusive DLL & Migrations-Ordner.
2. Lade das ZIP unter der URL hoch, die in `DemoMauiApp/Services/ModuleManager.RemoteUrl` konfiguriert ist.

### 3. DemoMauiApp starten

```bash
dotnet run -p DemoMauiApp/DemoMauiApp.csproj -f net6.0-android
```

### 4. Plugin laden & verwenden

1. In der App: **Load Plugin** tippen → Plugin wird heruntergeladen, entpackt und initialisiert.
2. Anschließend **Go to Plugin Page** drücken → Anzeige der Plugin-UI.

---

## SQLite3 & Migrationen

* **Gemeinsame DB**: `{AppDataDirectory}/global.db`
* **History-Tabelle** pro Modul: `{moduleId}_MigrationsHistory`
* **SQL-Skripte** in numerischer Reihenfolge (`001_…`, `002_…`) mit Platzhalter `{prefix}_` für Tabellen-Präfix.

### Migrationen aus Embedded Resources

Plugins liefern ihre Migrationsskripte **als Embedded Resources** in der DLL mit:

```xml
<ItemGroup>
  <EmbeddedResource Include="Migrations\*.sql" />
</ItemGroup>
```

Das Host-System sucht beim Aufruf von `RegisterDatabase` zunächst im entpackten Migrations-Ordner und fällt bei Fehlen auf die eingebetteten Ressourcen zurück:

```csharp
public IModuleDatabase RegisterDatabase(string moduleId, Assembly moduleAssembly)
{
    // Open DB, create history table...

    // Versuch 1: Dateisystem
    var folder = Path.Combine(AppData, "Modules", moduleId, "Migrations");
    if (Directory.Exists(folder))
        // lade SQL-Dateien aus dem Ordner

    // Versuch 2: Embedded Resources
    var prefix = moduleAssembly.GetName().Name + ".Migrations.";
    var resources = moduleAssembly.GetManifestResourceNames()
        .Where(r => r.StartsWith(prefix) && r.EndsWith(".sql"));
    foreach (var res in resources)
    {
        using var stream = moduleAssembly.GetManifestResourceStream(res)!;
        using var rdr    = new StreamReader(stream);
        var sql = rdr.ReadToEnd().Replace("{prefix}_", moduleId + "_");
        connection.Execute(sql);
        // schreibe Eintrag in History
    }
}
```

Dadurch musst du keine physischen `.sql`-Dateien verteilen – alle Migrationen werden direkt aus der Plugin-DLL ausgeführt.

---

## Android-Konfiguration

* Manifest: `DemoMauiApp/Platforms/Android/AndroidManifest.xml`
* Passe `package`-Attribut und Icons im `Mipmap`-Ordner an.

---

## Erweiterung & Anpassung

* **Neue Module**: Erstelle ein MAUI-Klassenbibliothek-Projekt, referenziere `ModuleContracts`, implementiere `IAppModule` und binde Migrationsskripte als `EmbeddedResource` ein.
* **Migrationen**: Pflege SQL-Dateien im `Migrations`-Ordner deiner Plugin-Bibliothek.
* **Remote-URL** ändern in `ModuleManager.RemoteUrl`.
* **CI/CD**: Baue Plugin-ZIP-Artefakte und stelle sie im CDN bereit.

---

© 2025 – Frei verwendbar und anpassbar. Viel Spaß mit deinem modularen MAUI-Plugin-System!
