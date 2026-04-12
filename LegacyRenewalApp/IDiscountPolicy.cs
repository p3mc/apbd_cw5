namespace LegacyRenewalApp;

public interface IDiscountPolicy
{
    DiscountResult Calculate(
        Customer customer,
        SubscriptionPlan plan,
        int seatCount,
        decimal baseAmount,
        bool useLoyaltyPoints);
}