namespace Services
{
    public interface IServiceCollectionExtensionsService
    {
        void CreateFile(string filePath, string namespaceName, params string[] usings);
    }
}