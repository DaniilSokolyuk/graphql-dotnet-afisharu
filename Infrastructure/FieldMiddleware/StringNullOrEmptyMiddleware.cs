using System.Threading.Tasks;
using GraphQL.Instrumentation;
using GraphQL.Types;

namespace Afisha.Graphql.Infrastructure.FieldMiddleware
{
    public class StringNullOrEmptyMiddleware
    {
        public Task<object> Resolve(ResolveFieldContext context, FieldMiddlewareDelegate next)
        {
            if (context.ReturnType is StringGraphType)
            {
                return FastDecorator(context, next, null);
            }

            if (context.ReturnType is NonNullGraphType nonnull && nonnull.ResolvedType is StringGraphType)
            {
                return FastDecorator(context, next, string.Empty);
            }

            return next(context);
        }

        public async Task<object> FastDecorator(ResolveFieldContext context, FieldMiddlewareDelegate next, string emptyChar)
        {
           var result = await next(context) as string;

           return string.IsNullOrEmpty(result) ? emptyChar : result;
        }
    }
}
