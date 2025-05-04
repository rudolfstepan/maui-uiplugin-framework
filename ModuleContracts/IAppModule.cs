namespace ModuleContracts
{
    public interface IAppModule
    {
        string Id { get; }
        void Initialize(IModuleHost host);
    }
}