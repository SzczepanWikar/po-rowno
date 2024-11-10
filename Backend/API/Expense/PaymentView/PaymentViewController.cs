using Microsoft.AspNetCore.Mvc;

namespace API.Expense.PaymentView
{
    public class PaymentViewController : Controller
    {
        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
