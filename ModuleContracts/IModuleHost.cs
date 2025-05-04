using System.Reflection;

namespace ModuleContracts
{
    public interface IModuleHost
    {
        /// <summary>
        /// Registriert das Modul in der gemeinsamen DB, f�hrt Migrationen aus
        /// und liefert das kleine ORM-Objekt zur�ck.
        /// </summary>
        IModuleDatabase RegisterDatabase(string moduleId, Assembly moduleAssembly);
    }

}