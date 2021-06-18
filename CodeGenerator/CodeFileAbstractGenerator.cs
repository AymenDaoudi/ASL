using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CodeGenerator
{
    public abstract class CodeFileAbstractGenerator
    {
        protected readonly string @namespace;
        protected readonly string @class;
        protected readonly TypeAttributes modifier;
        protected readonly bool isStatic;
        protected readonly string[] usings;

        protected Dictionary<string, string> codeSubsitutions;
        protected ICollection<CodeMemberMethod> methods = new List<CodeMemberMethod>();

        public CodeFileAbstractGenerator(
            string @namespace,
            string @class,
            TypeAttributes modifier,
            bool isStatic,
            params string[] usings
        )
        {
            this.@namespace = @namespace;
            this.@class = @class;
            this.modifier = modifier;
            this.isStatic = isStatic;
            this.usings = usings;
            this.codeSubsitutions = new Dictionary<string, string>();
        }

        public virtual void CreateFile(string filePath) => CreateFileDefault(filePath);

        protected void CreateFileDefault(string filePath)
        {
            var codeCompileUnit = GenerateCode();
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";

            using (FileStream fs = File.Create(filePath))
            {
                using (StreamWriter sourceWriter = new StreamWriter(fs))
                {
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        provider.GenerateCodeFromCompileUnit(
                            codeCompileUnit,
                            stringWriter,
                            options
                        );

                        var code = stringWriter.ToString();
                        foreach (var codeSubstituion in codeSubsitutions)
                        {
                            code = code.Replace(codeSubstituion.Key, codeSubstituion.Value);
                        }

                        sourceWriter.Write(code);
                    }
                }
            }
        }

        protected abstract CodeCompileUnit GenerateCode();

    }
}
