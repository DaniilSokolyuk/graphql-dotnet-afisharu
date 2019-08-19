using Afisha.Graphql.Infrastructure.DataLoader;
using GraphQL.Types;

namespace Afisha.Graphql.Infrastructure
{
    public static class ResolveFieldContextExtensions
    {
        public static IResolverContext Fix<T>(this ResolveFieldContext<T> ctx)
        {
            return new ResolverContext(((Schema)ctx.Schema).DependencyResolver, ctx.CancellationToken);
        }

        public static UserContext.UserContext GetUserContext<TSource>(this ResolveFieldContext<TSource> context) => (UserContext.UserContext)context.UserContext;
    }
}
