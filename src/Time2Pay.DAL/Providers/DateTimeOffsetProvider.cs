using Time2Pay.DAL.Providers.Interfaces;

namespace Time2Pay.DAL.Providers;

public sealed class DateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
