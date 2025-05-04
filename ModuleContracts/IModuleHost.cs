namespace ModuleContracts
{
    public interface IModuleHost
    {
        IModuleDatabase RegisterDatabase(string moduleId, string migrationsFolder);
    }
}