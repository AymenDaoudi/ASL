using Domain.AbstractRepositories.Statements;
using Domain.Entities.Expressions;
using Domain.Entities.Statements;

namespace CodeGenerator.Generators.Statements
{
    public class ReturnStatementGenerator : IReturnStatementGenerator<ReturnStatementEntity, ExpressionEntityBase>
    {
        public ReturnStatementEntity Generate(ExpressionEntityBase expression)
        {
            ReturnStatementEntity returnStatement = new ReturnStatementEntity(expression);

            return returnStatement;
        }
    }
}