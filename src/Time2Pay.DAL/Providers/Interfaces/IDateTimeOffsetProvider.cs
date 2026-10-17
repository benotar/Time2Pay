namespace Time2Pay.DAL.Providers.Interfaces;

public interface IDateTimeOffsetProvider
{
    DateTimeOffset UtcNow { get; }
}
