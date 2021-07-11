using CodeGenerator.Abstract.Entities.Expressions;
using CodeGenerator.Abstract.Entities.Statements;
using CodeGenerator.Abstract.Generators.Statements;

namespace CodeGenerator.Roslyn.Generators.Statements
{
    public class StatementGenerator : IStatementGenerator<StatementEntityBase, ExpressionEntityBase>
    {
        public StatementEntityBase Generate(ExpressionEntityBase expression)
        {
            StatementEntityBase returnStatement = new StatementEntityBase(expression);

            return returnStatement;
        }
    }
}