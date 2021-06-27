using Domain.Entities.Namespaces;
using Domain.Entities.Types;

namespace CodeGenerator.Generators
{
    public interface ICodeFileGenerator<TType> where TType : TypeEntityBase
    {
        void CreateFile(
            string filePath,
            NamespaceEntityBase<TType> namespaceEntity,
            params string[] usings
        );
    }
}