using AzFunctionTSDemo;
using AzFunctionTSDemo.Abstractions;
using AzFunctionTSDemo.Entities;
using AzFunctionTSDemo.Services;
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
namespace AzFunctionTSDemo
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IMembershipDetailsDataService, MessageDataService>();
            builder.Services.AddScoped<IMessageDataService, OrganisationDataService>();
            builder.Services.AddScoped<IRolesMembershipsDataService, RolesMembershipsDataService>();
            builder.Services.AddScoped<IOrganisationDataService, OrganisationDataService>();
        }
    }
}
