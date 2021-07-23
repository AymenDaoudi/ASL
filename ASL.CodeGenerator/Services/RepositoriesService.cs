using System.IO;

using CSCG.Abstract.Entities;
using CSCG.Abstract.Entities.Methods.Classes;
using CSCG.Abstract.Entities.Methods.Interfaces;
using CSCG.Abstract.Entities.Namespaces;
using CSCG.Abstract.Entities.Types;
using CSCG.Abstract.Entities.Types.Classes;
using CSCG.Abstract.Entities.Types.Interfaces;
using CSCG.Abstract.Generators.Files;
using CSCG.Abstract.Generators.Namespaces;
using CSCG.Abstract.Generators.Types.Classes;
using CSCG.Abstract.Generators.Types.Interfaces;
using static ASL.CodeGenerator.Consts;

namespace ASL.CodeGenerator.Services
{
    public class RepositoriesService : IServicesService
    {
        private const string REPOSITORY = "Repository";

        private readonly IClassGenerator<ClassEntityBase, ClassMethodEntity> _classGenerator;
        private readonly IInterfaceGenerator<InterfaceEntityBase, InterfaceMethodEntity> _interfaceGenerator;
        private readonly INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> _namespaceGenerator;
        private readonly ICodeFileGenerator<TypeEntityBase> _codeFileGenerator;

        public RepositoriesService(
            IClassGenerator<ClassEntityBase, ClassMethodEntity> classGenerator,
            IInterfaceGenerator<InterfaceEntityBase, InterfaceMethodEntity> interfaceGenerator,
            INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> namespaceGenerator,
            ICodeFileGenerator<TypeEntityBase> codeFileGenerator
        )
        {
            _classGenerator = classGenerator;
            _interfaceGenerator = interfaceGenerator;
            _namespaceGenerator = namespaceGenerator;
            _codeFileGenerator = codeFileGenerator;
        }

        public void Create(
            string name,
            string path,
            string namespaceName,
            string interfaceName = null,
            params string[] usings
        )
        {
            var modifiers = AccessModifiers.Public;

            ClassEntityBase @class;

            if (interfaceName != null)
            {
                var @interface = _interfaceGenerator
                    .Initialize(interfaceName, modifiers)
                    .Generate();

                @class = _classGenerator
                    .Initialize(className: name, modifiers)
                    .ImplementInterfaces(@interface)
                    .Generate();

            }
            else
            {
                @class = _classGenerator
                    .Initialize(className: name, modifiers)
                    .Generate();
            }

            var @namespace = _namespaceGenerator.Initialize(namespaceName)
                .SetMemebers(@class)
                .Generate();

            path = Path.Combine(path, name + CS_EXTENSION);

            _codeFileGenerator.CreateFile(path, @namespace, usings);
        }

        public void CreateInterface(
            string name,
            string path,
            string namespaceName,
            params string[] usings
        )
        {
            var modifiers = AccessModifiers.Public;

            var @interface = _interfaceGenerator
                .Initialize(name, modifiers)
                .Generate();

            var interfaceNamespace = _namespaceGenerator
                .Initialize(namespaceName)
                .SetMemebers(@interface)
                .Generate();

            path = Path.Combine(path, string.Concat(name, CS_EXTENSION));

            _codeFileGenerator.CreateFile(path, interfaceNamespace, usings);
        }
    }
}
