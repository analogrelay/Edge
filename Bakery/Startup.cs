using Owin;
using Edge;
using KillScreen;

namespace Bakery
{
    public class Startup
    {
        public void Configuration(IAppBuilder app) {
            app.UseKillScreen();
            app.UseEdge();
        }
    }
}