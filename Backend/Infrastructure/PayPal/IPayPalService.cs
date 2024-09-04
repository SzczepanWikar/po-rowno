using Core.Common.PayPal;
using Core.Group;

namespace Infrastructure.PayPal
{
    public interface IPayPalService
    {
        Task<CreatedOrder> Create(Currency currency, decimal amount);
        Task<CapturedOrder> Capture(string orderId);
    }
}
