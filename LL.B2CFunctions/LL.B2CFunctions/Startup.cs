using LL.B2CFunctions;
using LL.B2CFunctions.Abstractions;
using LL.B2CFunctions.Entities;
using LL.B2CFunctions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Startup))]
namespace LL.B2CFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IMembershipDetailsDataService, MembershipDetailsDataService>();
            builder.Services.AddScoped<IRolesAppsDataService, RolesAppsDataService>();
            builder.Services.AddScoped<IRolesMembershipsDataService, RolesMembershipsDataService>();
            builder.Services.AddScoped<IRolesDataService, RolesDataService>();
        }
    }
}
