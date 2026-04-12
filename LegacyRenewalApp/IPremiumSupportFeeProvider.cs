namespace LegacyRenewalApp;

public interface IPremiumSupportFeeProvider
{
    decimal GetFee(string planCode);
}