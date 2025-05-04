using System.Reflection;

namespace ModuleContracts
{
    public interface IModuleHost
    {
        /// <summary>
        /// Registriert das Modul in der gemeinsamen DB, führt Migrationen aus
        /// und liefert das kleine ORM-Objekt zurück.
        /// </summary>
        IModuleDatabase RegisterDatabase(string moduleId, Assembly moduleAssembly);
    }

}