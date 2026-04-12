namespace LegacyRenewalApp;

public class LoyaltyPointsDiscountPolicy : IDiscountPolicy
{
    private const int MaxPointsPerRenewal = 200;

    public DiscountResult Calculate(
        Customer customer,
        SubscriptionPlan plan,
        int seatCount,
        decimal baseAmount,
        bool useLoyaltyPoints)
    {
        if (!useLoyaltyPoints || customer.LoyaltyPoints <= 0)
            return DiscountResult.None();

        int pointsToUse = customer.LoyaltyPoints > MaxPointsPerRenewal ? MaxPointsPerRenewal : customer.LoyaltyPoints;

        return new DiscountResult(pointsToUse, $"loyalty points used: {pointsToUse}; ");
    }
}