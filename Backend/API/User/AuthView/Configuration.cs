namespace API.User.AuthView
{
    public static class Configuration
    {
        public static WebApplication UseAuthViews(this WebApplication app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=AuthView}/{action=AccountActivation}/{token}"
                );
            });

            return app;
        }
    }
}
