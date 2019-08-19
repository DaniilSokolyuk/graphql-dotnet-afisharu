using System;
using System.Threading.Tasks;
using GraphQL.Server.Transports.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace Afisha.Graphql.Infrastructure.UserContext
{
    public class UserContextBuilder : IUserContextBuilder
    {
        public async Task<object> BuildUserContext(HttpContext httpContext)
        {
            return new UserContext(DateTimeOffset.Now);
        }
    }
}
