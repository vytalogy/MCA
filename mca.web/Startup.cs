using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mca.web.Startup))]
namespace mca.web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
