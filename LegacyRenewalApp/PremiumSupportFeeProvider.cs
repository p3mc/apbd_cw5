using System.Collections.Generic;

namespace LegacyRenewalApp;

public class PremiumSupportFeeProvider : IPremiumSupportFeeProvider
{
    private static readonly Dictionary<string, decimal> Fees = new Dictionary<string, decimal>
    {
        { "START",      250m },
        { "PRO",        400m },
        { "ENTERPRISE", 700m }
    };

    public decimal GetFee(string planCode)
        => Fees.TryGetValue(planCode, out decimal fee) ? fee : 0m;
}