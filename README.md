# MAUI Plugin Demo

Dieses Repository enthält eine kleine Demo-Anwendung auf Basis von .NET MAUI (Android & Windows), die zur Laufzeit ein externes UI-Plugin per Remote-Download lädt, initialisiert und in die App-Shell integriert. Zusätzlich wird eine Mikro-ORM-Lösung (Dapper + SQLite3) mit automatisierten Migrationen pro Modul gezeigt.

## Projektstruktur

* **ModuleContracts**

  * `IAppModule`, `IModuleHost`, `IModuleDatabase`
  * Definiert das Host–Modul‑API für Plugin-Entwickler.

* **PluginModule**

  * Beispiel-Plugin als MAUI-Klassenbibliothek
  * `SampleUIPluginModule` implementiert `IAppModule`
  * `PluginPage` mit einfacher UI
  * Migrations-SQL unter `Migrations/001_CreateTable.sql`

* **DemoMauiApp**

  * MAUI-App mit Shell und Home-Page
  * **Services**:

    * `ModuleHost`: Verwaltung der globalen SQLite-DB, Migration-Runner, Mikro-ORM (Dapper)
    * `ModuleManager`: Remote-ZIP-Download der Plugins (URL konfigurierbar)
    * `PluginLoader`: AssemblyLoadContext-basiertes Laden und Initialisieren der Module
  * `MainPage` mit Buttons zum Nachladen und Navigieren zum Plugin
  * AndroidManifest unter `Platforms/Android/AndroidManifest.xml`

## Voraussetzungen

* [.NET 6 SDK](https://dotnet.microsoft.com/download)
* MAUI-Workload installiert (`dotnet workload install maui`)
* Android SDK & Emulator oder verbundenes Gerät
* (Optional) Windows 10 SDK für Windows-Target

## Inbetriebnahme

### 1. Projekte bauen

```bash
dotnet build ModuleContracts/ModuleContracts.csproj
dotnet build PluginModule/PluginModule.csproj
dotnet build DemoMauiApp/DemoMauiApp.csproj -f net6.0-android
```

### 2. Plugin bereitstellen

1. Erzeuge ein ZIP mit dem Inhalt von `PluginModule/bin/Debug/net6.0-android/` inklusive:

   * `PluginModule.dll`
   * Ordner `Migrations/` mit den SQL-Skripten
2. Lade das ZIP unter der URL hoch, die in `DemoMauiApp/Services/ModuleManager.RemoteUrl` konfiguriert ist (Standard: `https://example.com/PluginModule.zip`).

### 3. DemoMauiApp starten

```bash
dotnet run -p DemoMauiApp/DemoMauiApp.csproj -f net6.0-android
```

### 4. Plugin laden & verwenden

1. In der App: **Load Plugin** tippen → Plugin wird heruntergeladen, entpackt und initialisiert.
2. Anschließend **Go to Plugin Page** drücken → Anzeige der Plugin-UI.

## SQLite3 & Migrationen

* Gemeinsame DB: `{AppDataDirectory}/global.db`
* Module rufen in `Initialize`:

  ```csharp
  var db = host.RegisterDatabase(
      moduleId: Id,
      migrationsFolder: Path.Combine(FileSystem.AppDataDirectory, "Modules", Id, "Migrations")
  );
  ```
* Migrationen: SQL-Dateien in numerischer Reihenfolge (`001_…`, `002_…` …) mit Platzhalter `{prefix}_` für Tabellen-Prefix.
* History-Tabelle: `{moduleId}_MigrationsHistory` dokumentiert ausgeführte Skripte.

## Android-Konfiguration

* Manifest: `DemoMauiApp/Platforms/Android/AndroidManifest.xml`
* Passe `package`-Attribut und Icons im `Mipmap`-Ordner an.

## Erweiterung & Anpassung

* **Neue Module**: Neuen MAUI-Klassenbibliothek-Projekt gegen `ModuleContracts` erstellen, `IAppModule` implementieren.
* **Migrationen**: Weitere SQL-Skripte im Migrations-Ordner des Moduls anlegen.
* **Remote-URL** ändern in `ModuleManager.RemoteUrl`.
* **Automatisierung**: CI/CD-Pipeline kann ZIP-Artefakte bauen und ins CDN pushen.

---

© 2025 – Frei verwendbar und anpassbar. Viel Spaß mit deinem modularen MAUI-Plugin-System!
