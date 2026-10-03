using System.Runtime.Serialization;

namespace Time2Pay.Api.Entities.Enums;

public enum Currency
{
    None = 0,

    [EnumMember(Value = "UAH")] Uah = 1,

    [EnumMember(Value = "USD")] Usd = 2,

    [EnumMember(Value = "EUR")] Eur = 3,
}
