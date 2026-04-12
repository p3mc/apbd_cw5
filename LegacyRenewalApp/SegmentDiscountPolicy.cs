namespace LegacyRenewalApp;

public class SegmentDiscountPolicy : IDiscountPolicy
{
    public DiscountResult Calculate(
        Customer customer,
        SubscriptionPlan plan,
        int seatCount,
        decimal baseAmount,
        bool useLoyaltyPoints)
    {
        return customer.Segment switch
        {
            "Silver"    => new DiscountResult(baseAmount * 0.05m, "silver discount; "),
            "Gold"      => new DiscountResult(baseAmount * 0.10m, "gold discount; "),
            "Platinum"  => new DiscountResult(baseAmount * 0.15m, "platinum discount; "),
            "Education" when plan.IsEducationEligible => new DiscountResult(baseAmount * 0.20m, "education discount; "),
            _ => DiscountResult.None()
        };
    }
}