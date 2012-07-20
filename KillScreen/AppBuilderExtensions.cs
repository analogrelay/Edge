using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace KillScreen
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseKillScreen(this IAppBuilder builder)
        {
            builder.Use<AppDelegate>(new KillScreenMiddleware().Start);
        }
    }
}
