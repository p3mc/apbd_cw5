using System;
using System.Collections.Generic;
using System.Text;

namespace LegacyRenewalApp;

public class DiscountCalculator
{
    private const decimal MinSubtotal = 200m;
    private readonly List<IDiscountPolicy> discountPolicies;
    
    public DiscountCalculator(List<IDiscountPolicy> discountPolicies)
    {
        this.discountPolicies = discountPolicies;
    }

    public (decimal DiscountAmount, decimal SubtotalAfterDiscount, string Notes) Calculate(
        Customer customer,
        SubscriptionPlan plan,
        int seatCount,
        decimal baseAmount,
        bool useLoyaltyPoints)
    {
        decimal totalDiscount = 0m;
        var notes = new StringBuilder();
        
        foreach (var policy in discountPolicies)
        {
            var result = policy.Calculate(customer, plan, seatCount, baseAmount, useLoyaltyPoints);
            totalDiscount += result.Amount;
            notes.Append(result.Notes);
        }
        
        decimal subtotal = baseAmount - totalDiscount;
        if (subtotal < MinSubtotal)
        {
            subtotal = MinSubtotal;
            notes.Append("minimum discounted subtotal applied; ");
        }
        
        return (Math.Round(totalDiscount, 2), subtotal, notes.ToString());
    }
}