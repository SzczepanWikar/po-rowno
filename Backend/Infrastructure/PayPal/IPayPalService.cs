using Core.Common.PayPal;

namespace Infrastructure.PayPal
{
    public interface IPayPalService
    {
        Task<CreatedOrder> Create(NewOrder payment);
        Task<CapturedOrder> Capture(string orderId);
    }
}
