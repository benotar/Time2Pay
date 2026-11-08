namespace Time2Pay.BLL.Utils;

public sealed record ValidationViolationItem(string Rule, string PropertyName, string ErrorMessage);
