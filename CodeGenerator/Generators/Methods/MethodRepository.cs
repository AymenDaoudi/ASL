using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeGenerator.Abstract.Generators.Statements;
using CodeGenerator.Abstract.Generators.Methods;
using CodeGenerator.Abstract.Entities.Expressions;
using CodeGenerator.Abstract.Entities.Statements;
using CodeGenerator.Abstract.Entities.Types.Classes;
using CodeGenerator.Roslyn.Exceptions;

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
                throw new NoOrMultipleMethodException(string.Format(NoOrMultipleMethodException.NO_METHOD_ERROR_MESSAGE, methodName), exception);
            }
            catch (InvalidOperationException exception) when (exception.Message == "Sequence contains more than one matching element")
            {
                throw new NoOrMultipleMethodException(string.Format(NoOrMultipleMethodException.MULTIPLE_METHODS_ERROR_MESSAGE, methodName), exception);
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
