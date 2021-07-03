using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Generators.Statements;
using Domain.AbstractRepositories.Methods;
using Domain.Entities.Expressions;
using Domain.Entities.Statements;
using Domain.Entities.Types.Classes;

namespace CodeGenerator.Generators.Methods
{
    public class MethodRepository : IMethodRepository
    {
        public ReturnStatementEntity GetReturnStatement(ClassEntityBase @class, string methodName)
        {
            var returnStatementGenerator = new ReturnStatementGenerator();

            var classDeclarationSyntax = (@class.TypeRoot) as ClassDeclarationSyntax;

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
            var returnStatement = returnStatementGenerator.Generate(expression);

            return returnStatement;
        }
    }
}
