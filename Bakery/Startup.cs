using Owin;
using Edge;

namespace Bakery
{
    public class Startup
    {
        public void Configuration(IAppBuilder app) {
            app.UseEdge();
        }
    }
}