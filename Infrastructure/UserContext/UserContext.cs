using System;

namespace Afisha.Graphql.Infrastructure.UserContext
{
    public class UserContext
    {
        public UserContext(DateTimeOffset nowDateTimeOffset)
        {
            NowDateTimeOffset = nowDateTimeOffset;
        }

        public DateTime NowLocalDateTime => NowDateTimeOffset.LocalDateTime;

        public DateTimeOffset NowDateTimeOffset { get; }
    }
}
