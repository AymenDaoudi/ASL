using System.Threading.Tasks;

namespace ASL.CodeGenerator
{
    public interface IServiceCollectionExtensionsService
    {
        void CreateFile(string filePath, string namespaceName, params string[] usings);

        Task RegisterNewRepositoryAsync(
            string filePath,
            DILifetime dILifeTime,
            string abstractTypeName,
            string ImplementationTypeName
        );

        Task RegisterNewRepositoryAsync(
            string filePath,
            DILifetime dILifeTime,
            string ImplementationTypeName
        );

        Task RegisterNewServiceAsync(
            string filePath,
            DILifetime dILifeTime,
            string abstractTypeName,
            string ImplementationTypeName
        );

        Task RegisterNewServiceAsync(
            string filePath,
            DILifetime dILifeTime,
            string ImplementationTypeName
        );
    }
}