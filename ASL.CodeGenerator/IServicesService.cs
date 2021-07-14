namespace ASL.CodeGenerator
{
    public interface IServicesService
    {
        void CreateService(
            string path,
            string namespaceName,
            string name, 
            string interfacePath = null,
            string namespaceInterface = null,
            bool isRepository = false
        );
    }
}