using System.Runtime.Serialization;

namespace Time2Pay.Api.Entities.Enums;

public enum Currency
{
    [EnumMember(Value = "UAH")] Uah = 0,

    [EnumMember(Value = "USD")] Usd = 1
}
