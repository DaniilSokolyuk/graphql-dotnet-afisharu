using System.Collections.Generic;
using System.Threading;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.Server;
using GraphQL.Server.Internal;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.Extensions.Options;

namespace Afisha.Graphql.Infrastructure.FieldMiddleware
{
    public class GraphQlExecutorDecorator<TSchema> : DefaultGraphQLExecuter<TSchema> where TSchema : ISchema
    {
        public GraphQlExecutorDecorator(TSchema schema, IDocumentExecuter documentExecuter, IOptions<GraphQLOptions> options, IEnumerable<IDocumentExecutionListener> listeners, IEnumerable<IValidationRule> validationRules) : base(schema, documentExecuter, options, listeners, validationRules)
        {
        }

        protected override ExecutionOptions GetOptions(
            string operationName, 
            string query, 
            Inputs variables, 
            object context,
            CancellationToken cancellationToken)
        {
            var options = base.GetOptions(operationName, query, variables, context, cancellationToken);

            options.FieldMiddleware = options.FieldMiddleware
                .Use(typeof(StringNullOrEmptyMiddleware));
                //.Use(typeof(SOME YOUR MIDDLEWARE))

            return options;
        }
    }
}
