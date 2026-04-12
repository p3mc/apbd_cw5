using System;

namespace LegacyRenewalApp;

public class InvoiceBuilder
{
    private const decimal MinFinalAmount = 500m;

    public RenewalInvoice Build(
        Customer customer,
        RenewalRequest renewalRequest,
        decimal baseAmount,
        decimal discountAmount,
        decimal subtotalAfterDiscount,
        decimal supportFee,
        decimal paymentFee,
        decimal taxAmount,
        string notes
        )
    {
        decimal finalAmount = subtotalAfterDiscount + supportFee + paymentFee + taxAmount;
        string finalNotes = notes;
        
        if (finalAmount < MinFinalAmount)
        {
            finalAmount = MinFinalAmount;
            finalNotes += "minimum invoice amount applied; ";
        }


        return new RenewalInvoice
        {
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customer.Id}-{renewalRequest.PlanCode}",
            CustomerName = customer.FullName,
            PlanCode = renewalRequest.PlanCode,
            PaymentMethod = renewalRequest.PaymentMethod,
            SeatCount = renewalRequest.SeatCount,
            BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
            DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
            SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
            PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
            TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
            FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
            Notes = notes.Trim(),
            GeneratedAt = DateTime.UtcNow
        };
    }
}