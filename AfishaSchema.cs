using Afisha.Graphql.Types;
using GraphQL;
using GraphQL.Types;

namespace Afisha.Graphql
{
    internal class AfishaSchema : Schema
    {
        public AfishaSchema(RootQuery query, IDependencyResolver resolver)
        {
            Query = query;
            DependencyResolver = resolver;
        }
    }
}
