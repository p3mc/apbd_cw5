using System;
using System.Collections.Generic;

namespace LegacyRenewalApp;

public class PaymentFeeCalculator : IPaymentFeeCalculator
{
    private static readonly Dictionary<string, (decimal Rate, string Notes)> Rates =
        new Dictionary<string, (decimal, string)>
        {
            { "CARD",          (0.020m, "card payment fee; ") },
            { "BANK_TRANSFER", (0.010m, "bank transfer fee; ") },
            { "PAYPAL",        (0.035m, "paypal fee; ") },
            { "INVOICE",       (0.000m, "invoice payment; ") }
        };

    public (decimal Fee, string Notes) Calculate(string paymentMethod, decimal taxableBase)
    {
        if (!Rates.TryGetValue(paymentMethod, out var entry))
            throw new ArgumentException("Unsupported payment method");

        return (taxableBase * entry.Rate, entry.Notes);
    }
}