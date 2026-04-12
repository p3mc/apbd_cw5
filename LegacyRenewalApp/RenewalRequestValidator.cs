using System;

namespace LegacyRenewalApp;

public class RenewalRequestValidator
{
    private static readonly string[] SupportedPaymentMethods =
        { "CARD", "BANK_TRANSFER", "PAYPAL", "INVOICE" };

    public void Validate(int customerId, string planCode, int seatCount, string paymentMethod)
    {
        if (customerId <= 0)
            throw new ArgumentException("Customer id must be positive");

        if (string.IsNullOrWhiteSpace(planCode))
            throw new ArgumentException("Plan code is required");

        if (seatCount <= 0)
            throw new ArgumentException("Seat count must be positive");

        if (string.IsNullOrWhiteSpace(paymentMethod))
            throw new ArgumentException("Payment method is required");

        string normalized = paymentMethod.Trim().ToUpperInvariant();
        if (Array.IndexOf(SupportedPaymentMethods, normalized) < 0)
            throw new ArgumentException("Unsupported payment method");
    }
}