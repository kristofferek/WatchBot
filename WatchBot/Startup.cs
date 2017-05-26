using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WatchBot.Startup))]
namespace WatchBot
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
