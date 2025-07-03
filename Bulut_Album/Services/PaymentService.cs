using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;

public class PaymentService
{
    private readonly Options _options;

    public PaymentService()
    {
        _options = new Options
        {
            ApiKey = "sandbox-api-key",
            SecretKey = "sandbox-secret",
            BaseUrl = "https://sandbox-api.iyzipay.com"
        };
    }

    public async Task<Payment> MakePayment(string email, string cardNumber, string cvc, string expireMonth, string expireYear, decimal price)
    {
        var request = new CreatePaymentRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = Guid.NewGuid().ToString(),
            Price = price.ToString("0.00"),
            PaidPrice = price.ToString("0.00"),
            Currency = Currency.TRY.ToString(),
            Installment = 1,
            PaymentChannel = PaymentChannel.WEB.ToString(),
            PaymentGroup = PaymentGroup.PRODUCT.ToString()
        };

        request.PaymentCard = new PaymentCard
        {
            CardHolderName = "Ad Soyad",
            CardNumber = cardNumber,
            ExpireMonth = expireMonth,
            ExpireYear = expireYear,
            Cvc = cvc,
            RegisterCard = 0
        };

        request.Buyer = new Buyer
        {
            Id = "BY789",
            Name = "Ad",
            Surname = "Soyad",
            Email = email,
            IdentityNumber = "11111111111",
            RegistrationAddress = "Adres",
            Ip = "85.34.78.112",
            Country = "Turkey"
        };

        request.BasketItems = new List<BasketItem>
    {
        new BasketItem
        {
            Id = "BI101",
            Name = "Fotoğraf Yükleme Hizmeti",
            Category1 = "Hizmet",
            ItemType = BasketItemType.VIRTUAL.ToString(),
            Price = price.ToString("0.00")
        }
    };

      
        return await Payment.Create(request, _options);
    }

}
