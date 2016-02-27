using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MailTicketAzureMvc.Startup))]
namespace MailTicketAzureMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
