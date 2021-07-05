using CodeGenerator.Abstract.Entities.Expressions;
using CodeGenerator.Abstract.Entities.Statements;
using CodeGenerator.Abstract.Generators.Statements;

namespace CodeGenerator.Roslyn.Generators.Statements
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