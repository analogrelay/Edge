using Owin;
using Edge;

namespace Bakery
{
    public class Startup
    {
        public void Configure(IAppBuilder app) {
            app.UseEdge();
        }
    }
}