using System.Threading.Tasks;

namespace ASL.CodeGenerator
{
    public interface IStartupClassService
    {
        Task AddRegisterRepositoriesAndRegisterServicesAsync(string filePath);
    }
}