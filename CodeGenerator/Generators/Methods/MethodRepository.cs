using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeGenerator.Abstract.Generators.Statements;
using CodeGenerator.Abstract.Generators.Methods;
using CodeGenerator.Abstract.Entities.Expressions;
using CodeGenerator.Abstract.Entities.Statements;
using CodeGenerator.Abstract.Entities.Types.Classes;

namespace CodeGenerator.Roslyn.Generators.Methods
{
    public class MethodRepository : IMethodRepository
    {
        private readonly IReturnStatementGenerator<ReturnStatementEntity, ExpressionEntityBase> _returnStatementGenerator;

        public MethodRepository(IReturnStatementGenerator<ReturnStatementEntity, ExpressionEntityBase> returnStatementGenerator)
        {
            _returnStatementGenerator = returnStatementGenerator;
        }

        public ReturnStatementEntity GetReturnStatement(ClassEntityBase @class, string methodName)
        {
            var classDeclarationSyntax = @class.TypeRoot as ClassDeclarationSyntax;

            var statements = new List<StatementSyntax>();

            var method = classDeclarationSyntax
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .SingleOrDefault(m => m.Identifier.ValueText == methodName);

            var returnStatementSyntax = method
                .DescendantNodes()
                .OfType<ReturnStatementSyntax>()
                .SingleOrDefault();

            var expression = new ExpressionEntityBase(returnStatementSyntax.Expression);
            var returnStatement = _returnStatementGenerator.Generate(expression);

            return returnStatement;
        }
    }
}
