namespace LegacyRenewalApp;

public interface IPaymentFeeCalculator
{
    (decimal Fee, string Notes) Calculate(string paymentMethod, decimal taxableBase);
}