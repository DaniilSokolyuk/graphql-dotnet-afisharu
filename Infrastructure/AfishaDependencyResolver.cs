using System;
using GraphQL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Afisha.Graphql.Infrastructure
{
    public class AfishaDependencyResolver : IDependencyResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AfishaDependencyResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public T Resolve<T>() => _httpContextAccessor.HttpContext.RequestServices.GetService<T>();

        public object Resolve(Type type) => _httpContextAccessor.HttpContext.RequestServices.GetService(type);
    }
}
