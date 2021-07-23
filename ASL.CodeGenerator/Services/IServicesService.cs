namespace ASL.CodeGenerator.Services
{
    public interface IServicesService
    {
        void Create(
            string name,
            string path,
            string namespaceName,
            string interfaceName = null,
            params string[] usings
        );

        void CreateInterface(
           string name,
           string path,
           string namespaceName,
           params string[] usings
        );
    }
}