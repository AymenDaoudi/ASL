using System.Threading.Tasks;

namespace ASL.CodeGenerator.ServiceCollectionExtensions
{
    public interface IServiceCollectionExtensionsService
    {
        void CreateFile(string filePath, string namespaceName, params string[] usings);

        Task RegisterNewRepositoryAsync(
            string filePath,
            DILifetime dILifeTime,
            string abstractTypeName,
            string ImplementationTypeName,
            params string[] usings
        );

        Task RegisterNewRepositoryAsync(
            string filePath,
            DILifetime dILifeTime,
            string ImplementationTypeName,
            params string[] usings
        );

        Task RegisterNewServiceAsync(
            string filePath,
            DILifetime dILifeTime,
            string abstractTypeName,
            string ImplementationTypeName,
            params string[] usings
        );

        Task RegisterNewServiceAsync(
            string filePath,
            DILifetime dILifeTime,
            string ImplementationTypeName,
            params string[] usings
        );
    }
}