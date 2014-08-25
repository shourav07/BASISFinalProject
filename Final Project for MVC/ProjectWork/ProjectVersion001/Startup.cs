using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjectVersion001.Startup))]
namespace ProjectVersion001
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
