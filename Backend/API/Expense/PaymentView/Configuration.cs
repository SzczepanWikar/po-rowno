﻿namespace API.Expense.PaymentView
{
    public static class Configuration
    {
        public static WebApplication UsePaymentViews(this WebApplication app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "payment-success",
                    pattern: "payment/success",
                    defaults: new { controller = "PaymentView", action = "Success" }
                );

                endpoints.MapControllerRoute(
                    name: "payment-cancel",
                    pattern: "payment/cancel",
                    defaults: new { controller = "PaymentView", action = "Cancel" }
                );
            });

            return app;
        }
    }
}
