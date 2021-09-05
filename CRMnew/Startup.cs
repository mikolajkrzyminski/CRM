using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CRMnew.Startup))]
namespace CRMnew
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
