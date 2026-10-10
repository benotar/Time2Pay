using Time2Pay.DAL.Entities;

namespace Time2Pay.DAL.Utils;

public static class IdGenerator
{
    public static string PrefixId<TPrefix>() where TPrefix : IPrefix
    {
        return $"{TPrefix.Prefix}{Guid.CreateVersion7()}";
    }
}
