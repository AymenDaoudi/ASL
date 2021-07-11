using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Abstract.Generators.Statements;
using CodeGenerator.Abstract.Entities.Expressions;
using CodeGenerator.Abstract.Entities.Statements;
using CodeGenerator.Abstract.Entities.Types.Classes;
using CodeGenerator.Roslyn.Exceptions;
using CodeGenerator.Abstract.Repositories;

namespace CodeGenerator.Roslyn.Repositories
{
    public class MethodRepository : IMethodRepository
    {
        private readonly IStatementGenerator<StatementEntityBase, ExpressionEntityBase> _returnStatementGenerator;

        public MethodRepository(IStatementGenerator<StatementEntityBase, ExpressionEntityBase> returnStatementGenerator)
        {
            _returnStatementGenerator = returnStatementGenerator;
        }

        public StatementEntityBase GetReturnStatement(ClassEntityBase @class, string methodName)
        {
            var classDeclarationSyntax = @class.TypeRoot as ClassDeclarationSyntax;

            var statements = new List<StatementSyntax>();

            MethodDeclarationSyntax method = null;

            try
            {
                method = classDeclarationSyntax
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Single(m => m.Identifier.ValueText == methodName);
            }
            catch (InvalidOperationException exception) when (exception.Message == "Sequence contains no matching element")
            {
                throw new NoOrMultipleMethodsException(string.Format(NoOrMultipleMethodsException.NO_METHOD_ERROR_MESSAGE, methodName), exception);
            }
            catch (InvalidOperationException exception) when (exception.Message == "Sequence contains more than one matching element")
            {
                throw new NoOrMultipleMethodsException(string.Format(NoOrMultipleMethodsException.MULTIPLE_METHODS_ERROR_MESSAGE, methodName), exception);
            }

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
