namespace LegacyRenewalApp;

public interface ITaxCalculator
{
    decimal GetRate(string country);
}