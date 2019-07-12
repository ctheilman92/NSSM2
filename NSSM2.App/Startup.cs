using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NSSM2.App.Startup))]
namespace NSSM2.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
