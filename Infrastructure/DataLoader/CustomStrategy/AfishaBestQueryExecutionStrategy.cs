using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Types;

namespace Afisha.Graphql.Infrastructure.DataLoader.CustomStrategy
{
    /// <summary>
    /// TODO: some tasks allocations problems
    /// </summary>
    public class AfishaBestQueryExecutionStrategy : ExecutionStrategy
    {
        protected override async Task ExecuteNodeTreeAsync(ExecutionContext context, ObjectExecutionNode rootNode)
        {
            var pendingNodes = new List<ExecutionNode>
            {
                rootNode
            };

            var batchOperationHandler = CreateBatchOperationHandler(context);

            try
            {
                while (pendingNodes.Count > 0)
                {
                    var currentTasks = new Task<ExecutionNode>[pendingNodes.Count];

                    // Start executing all pending nodes
                    for (int i = 0; i < pendingNodes.Count; i++)
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();
                        currentTasks[i] = ExecuteNodeAsync(context, pendingNodes[i]);
                    }

                    pendingNodes.Clear();

                    await OnBeforeExecutionStepAwaitedAsync(context);

                    //batch data loader execution
                    if (batchOperationHandler != null)
                    {
                        await batchOperationHandler.CompleteAsync(new Memory<Task>(currentTasks.Cast<Task>().ToArray()), context.CancellationToken);
                    }
                    else
                    {
                        await Task.WhenAll(currentTasks);
                    }

                    // Await tasks for this execution step
                    foreach (var task in currentTasks)
                    {
                        var result = await task;

                        if (result is IParentExecutionNode parentNode)
                        {
                            pendingNodes.AddRange(parentNode.GetChildNodes());
                        }
                    }
                }
            }
            finally
            {
                batchOperationHandler?.Dispose();
            }
        }

        private BatchOperationHandler CreateBatchOperationHandler(ExecutionContext context)
        {
            var batchOperations = ((Schema)context.Schema).DependencyResolver.Resolve<IEnumerable<IBatchOperation>>();

            if (batchOperations != null && batchOperations.Any())
            {
               return new BatchOperationHandler(batchOperations);
            }

            return null;
        }
    }
}
