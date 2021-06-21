using System.IO;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerator
{
    public abstract class CodeFileAbstractGenerator
    {
        protected readonly string @namespace;
        protected readonly string[] usings;

        public CodeFileAbstractGenerator(string @namespace, params string[] usings)
        {
            this.@namespace = @namespace;
            this.usings = usings;
        }

        public virtual void CreateFile(string filePath) => CreateFileDefault(filePath);

        protected void CreateFileDefault(string filePath)
        {
            var compileUnit = GenerateCode();

            using (FileStream fs = File.Create(filePath))
            {
                using (StreamWriter sourceWriter = new StreamWriter(fs))
                {
                    compileUnit.NormalizeWhitespace().WriteTo(sourceWriter);
                }
            }
        }

        protected abstract CompilationUnitSyntax GenerateCode();
    }
}
