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
using System.IO;

namespace ASL.CodeGenerator
{
    public class ServicesService : IServicesService
    {
        private readonly IServiceCollectionExtensionsService _serviceCollectionExtensionsService;
        private readonly IClassGenerator<ClassEntityBase, ClassMethodEntity> _classGenerator;
        private readonly IInterfaceGenerator<InterfaceEntityBase, InterfaceMethodEntity> _interfaceGenerator;
        private readonly INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> _namespaceGenerator;
        private readonly ICodeFileGenerator<TypeEntityBase> _codeFileGenerator;

        public ServicesService(
            IServiceCollectionExtensionsService serviceCollectionExtensionsService,
            IClassGenerator<ClassEntityBase, ClassMethodEntity> classGenerator,
            IInterfaceGenerator<InterfaceEntityBase, InterfaceMethodEntity> interfaceGenerator,
            INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> namespaceGenerator,
            ICodeFileGenerator<TypeEntityBase> codeFileGenerator
        )
        {
            _serviceCollectionExtensionsService = serviceCollectionExtensionsService;
            _classGenerator = classGenerator;
            _interfaceGenerator = interfaceGenerator;
            _namespaceGenerator = namespaceGenerator;
            _codeFileGenerator = codeFileGenerator;
        }

        public void CreateService(
            string path,
            string namespaceName,
            string name,
            string interfacePath = null,
            string namespaceInterface = null,
            bool isRepository = false
        )
        {
            if (isRepository)
            {
                name += "Repository";
            }
            else
            {
                name += "Service";
            }

            var modifiers = AccessModifiers.Public;

            ClassEntityBase @class;

            if ((interfacePath != null) && (namespaceInterface != null))
            {
                var interfaceName = string.Concat("I", name);

                var @interface = _interfaceGenerator
                    .Initialize(interfaceName, modifiers)
                    .Generate();

                var interfaceNamespace = _namespaceGenerator.Initialize(namespaceInterface)
                .SetMemebers(@interface)
                .Generate();

                interfacePath = Path.Combine(interfacePath, interfaceName + ".cs" );

                _codeFileGenerator.CreateFile(interfacePath, interfaceNamespace, "System");

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

            path = Path.Combine(path, name + ".cs");

            _codeFileGenerator.CreateFile(path, @namespace, "System");
        }
    }
}