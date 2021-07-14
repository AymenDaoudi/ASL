using System.Threading.Tasks;

using CSCG.Abstract.Entities.Expressions;
using CSCG.Abstract.Entities.Statements;
using CSCG.Abstract.Generators.Expressions;
using CSCG.Abstract.Generators.Files;
using CSCG.Abstract.Generators.Statements;
using static ASL.CodeGenerator.Consts;

namespace ASL.CodeGenerator
{
    public class StartupClassService : IStartupClassService
    {
        private const string STARTUP = "Startup";
        private const string CONFIGURE_SERVICE = "ConfigureServices";

        private readonly ICodeFileModifier _classModifier;
        private readonly IObjectExpressionGenerator _objectExpressionGenerator;
        private readonly IMethodInvocationExpressionGenerator _methodInvocationExpressionGenerator;
        private readonly IStatementGenerator<StatementEntityBase, ExpressionEntityBase> _statementGenerator;

        public StartupClassService(
            ICodeFileModifier classModifier,
            IObjectExpressionGenerator objectExpressionGenerator,
            IMethodInvocationExpressionGenerator methodInvocationExpressionGenerator,
            IStatementGenerator<StatementEntityBase, ExpressionEntityBase> statementGenerator
        )
        {
            _classModifier = classModifier;
            _objectExpressionGenerator = objectExpressionGenerator;
            _methodInvocationExpressionGenerator = methodInvocationExpressionGenerator;
            _statementGenerator = statementGenerator;
        }

        public async Task AddRegisterRepositoriesAndRegisterServicesAsync(string filePath)
        {
            var objectExpression = _objectExpressionGenerator
                .Initialize(SERVICES)
                .Generate();

            var registerRepositoriesMethodInvocationExpression = _methodInvocationExpressionGenerator
                .Initialize(objectExpression, REGISTER_REPOSITORIES)
                .ChainMethodInvocation(REGISTER_SERVICES)
                .Generate();

            var registerRepositoriesMethodInvocationStatement = _statementGenerator.Generate(registerRepositoriesMethodInvocationExpression);

            await _classModifier.AddStatementToMethodOfClassAsync(
                filePath: filePath,
                className: STARTUP,
                methodName: CONFIGURE_SERVICE,
                statement: registerRepositoriesMethodInvocationStatement
            );
        }

    }
}
