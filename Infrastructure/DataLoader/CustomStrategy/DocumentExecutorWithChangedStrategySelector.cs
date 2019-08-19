using System;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Language.AST;

namespace Afisha.Graphql.Infrastructure.DataLoader.CustomStrategy
{
    public class DocumentExecutorWithChangedStrategySelector : DocumentExecuter
    {
        protected override IExecutionStrategy SelectExecutionStrategy(ExecutionContext context)
        {
            switch (context.Operation.OperationType)
            {
                case OperationType.Query:
                    return new AfishaBestQueryExecutionStrategy();

                case OperationType.Mutation:
                    return new SerialExecutionStrategy();

                case OperationType.Subscription:
                    return new SubscriptionExecutionStrategy();

                default:
                    throw new InvalidOperationException($"Unexpected OperationType {context.Operation.OperationType}");
            }
        }
    }
}
