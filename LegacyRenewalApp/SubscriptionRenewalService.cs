using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private  ICustomerRepository customerRepository;
        private ISubscriptionPlanRepository planRepository;
        private DiscountCalculator discountCalculator;
        private IPremiumSupportFeeProvider supportFeeProvider;
        private IPaymentFeeCalculator paymentFeeCalculator;
        private ITaxCalculator taxCalculator;
        private InvoiceBuilder invoiceBuilder;
        private IBillingGateway billingGateway;
        private RenewalRequestValidator validator;
        
        // konstruktor nie pusty
        public SubscriptionRenewalService(
            ICustomerRepository customerRepository,
            ISubscriptionPlanRepository planRepository,
            DiscountCalculator discountCalculator,
            IPremiumSupportFeeProvider supportFeeProvider,
            IPaymentFeeCalculator paymentFeeCalculator,
            ITaxCalculator taxCalculator,
            InvoiceBuilder invoiceBuilder,
            IBillingGateway billingGateway,
            RenewalRequestValidator validator)
        {
            this.customerRepository = customerRepository;
            this.planRepository = planRepository;
            this.discountCalculator = discountCalculator;
            this.supportFeeProvider = supportFeeProvider;
            this.paymentFeeCalculator = paymentFeeCalculator;
            this.taxCalculator = taxCalculator;
            this.invoiceBuilder = invoiceBuilder;
            this.billingGateway = billingGateway;
            this.validator = validator;
        }
        
        // nie ma argumentow
        
        
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            // if-else w walidatorze
            validator.Validate(customerId, planCode, seatCount, paymentMethod);

            var renewalRequest = new RenewalRequest(customerId, planCode, seatCount, paymentMethod, includePremiumSupport, useLoyaltyPoints);

            var customerRepository = new CustomerRepository();
            var planRepository = new SubscriptionPlanRepository();

            var customer = customerRepository.GetById(customerId);
            var plan = planRepository.GetByCode(renewalRequest.PlanCode);

            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }

            //--------liczenie disc----------
            decimal baseAmount = (plan.MonthlyPricePerSeat * renewalRequest.SeatCount * 12m) + plan.SetupFee;
            string notes = string.Empty;

            var (discountAmount, subtotal, discountNotes) = discountCalculator.Calculate(customer, plan, renewalRequest.SeatCount, baseAmount, renewalRequest.UseLoyaltyPoints);

            //--------liczenie supp fee-----------
            decimal supportFee   = 0m;
            string  supportNotes = string.Empty;
            if (includePremiumSupport)
            {
                supportFee   = supportFeeProvider.GetFee(renewalRequest.PlanCode);
                supportNotes = "premium support included; ";
            }

            var (paymentFee, paymentNotes) = paymentFeeCalculator.Calculate(renewalRequest.PaymentMethod, subtotal + supportFee);

            
            //------=liczenie podatku-----
            decimal taxRate   = taxCalculator.GetRate(customer.Country);

            decimal taxBase = subtotal + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            
            
            string allNotes = discountNotes + supportNotes + paymentNotes;
            
            var invoice = invoiceBuilder.Build(
                customer, renewalRequest, baseAmount, discountAmount, subtotal, supportFee, paymentFee, taxAmount, allNotes);

            billingGateway.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {renewalRequest.PlanCode} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                billingGateway.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
    }
}
