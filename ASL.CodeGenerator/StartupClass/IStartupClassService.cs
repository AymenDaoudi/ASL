using System.Threading.Tasks;

namespace ASL.CodeGenerator.StartupClass
{
    public interface IStartupClassService
    {
        Task AddRegisterRepositoriesAndRegisterServicesAsync(string filePath);
    }
}