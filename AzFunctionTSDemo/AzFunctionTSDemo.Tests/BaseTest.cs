﻿using AzFunctionTSDemo.Abstractions;
using AzFunctionTSDemo.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzFunctionTSDemo.Tests
{
    public abstract class BaseTest
    {
        protected static string AppSettingsFileName => "appsettings.test.json";

        protected static readonly List<MembershipDetails> MembershipDetailsTestData = new()
        {
            new MembershipDetails() { MembershipNumber = "1", Email = "testuser1@email.com" },
            new MembershipDetails() { MembershipNumber = "2", Email = "testuser2@email.com" },
            new MembershipDetails() { MembershipNumber = "3", Email = "testuser3@email.com" },
            new MembershipDetails() { MembershipNumber = "4", Email = "testuser4@email.com" },
            new MembershipDetails() { MembershipNumber = "5", Email = "testuser5@email.com" }
        };

        protected static readonly List<Organisation> RolesTestData = new()
        {
            new Organisation() { RowKey = "1", Name = "Role1", Active = true },
            new Organisation() { RowKey = "2", Name = "Role2", Active = true },
            new Organisation() { RowKey = "3", Name = "Role3", Active = false }
        };

        protected static readonly List<Message> RolesAppsTestData = new()
        {
            new Message() { OrganisationId = "1", Tenant = "devtenant.onmicrosoft.com", AppId = "4f8242de-6291-4dcb-9aca-e021981710dc", Active = true },
            new Message() { OrganisationId = "1", Tenant = "uattenant.onmicrosoft.com", AppId = "0edaef3f-8b75-44b8-be2b-2a5e2e74a759", Active = true },
            new Message() { OrganisationId = "1", Tenant = "prodtenant.onmicrosoft.com", AppId = "7770a5d0-7c1d-4008-831c-efb6b591e42e", Active = true },
            new Message() { OrganisationId = "2", Tenant = "devtenant.onmicrosoft.com", AppId = "7c08da3c-8808-4c3e-9196-7adbaa72e9bf", Active = true },
            new Message() { OrganisationId = "2", Tenant = "uattenant.onmicrosoft.com", AppId = "82415132-9153-4413-aabe-b60336104f19", Active = true },
            new Message() { OrganisationId = "2", Tenant = "prodtenant.onmicrosoft.com", AppId = "2fbef7e6-c825-4623-b7b5-25c7e4cb25a7", Active = false },
            new Message() { OrganisationId = "3", Tenant = "devtenant.onmicrosoft.com", AppId = "c780c845-d49c-4d54-973c-97ce84c85a57", Active = true },
            new Message() { OrganisationId = "3", Tenant = "uattenant.onmicrosoft.com", AppId = "e89da21f-14b4-4265-bfd5-663f785c62d9", Active = true },
            new Message() { OrganisationId = "3", Tenant = "prodtenant.onmicrosoft.com", AppId = "fcfd16dd-9559-49df-92e4-477102a0b9f2", Active = false },
        };

        protected static readonly List<RolesMemberships> RolesMembershipsTestData = new()
        {
            new RolesMemberships() { RoleId = "1", MembershipNumber = "1", Active = true },
            new RolesMemberships() { RoleId = "2", MembershipNumber = "1", Active = true },
            new RolesMemberships() { RoleId = "5", MembershipNumber = "4", Active = true },
            new RolesMemberships() { RoleId = "1", MembershipNumber = "2", Active = false },
            new RolesMemberships() { RoleId = "2", MembershipNumber = "2", Active = true },
            new RolesMemberships() { RoleId = "3", MembershipNumber = "5", Active = true }
        };

        protected IConfigurationRoot Configuration { get; }

        protected Mock<IMembershipDetailsDataService>? MembershipDetailsDSMock;
        protected Mock<IOrganisationDataService>? RolesDSMock;
        protected Mock<IMessageDataService>? RolesAppsDSMock;
        protected Mock<IRolesMembershipsDataService>? RolesMembershipsDSMock;

        public BaseTest()
        {
            Configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile(AppSettingsFileName)
                                    .Build();
        }

        protected static Mock<ILogger<T>> GetMockedILogger<T>()
        {
            var logger = new Mock<ILogger<T>>();

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var logLevel = (LogLevel)invocation.Arguments[0];
                    var eventId = (EventId)invocation.Arguments[1];
                    var state = invocation.Arguments[2];
                    var exception = (Exception)invocation.Arguments[3];
                    var formatter = invocation.Arguments[4];

                    var invokeMethod = formatter.GetType().GetMethod("Invoke");
                    var logMessage = (string?)invokeMethod?.Invoke(formatter, new[] { state, exception }) ?? "";

                    Trace.WriteLine($"{logLevel} - {logMessage}");
                }));

            return logger;
        }

        protected abstract void MockDependencies();

        protected virtual void MockMembershipDetailsDataService()
        {
            MembershipDetailsDSMock = new Mock<IMembershipDetailsDataService>();
            foreach (var membershipDetails in MembershipDetailsTestData)
            {
                MembershipDetailsDSMock.Setup(x => x.Get(membershipDetails.MembershipNumber, membershipDetails.Email))
                    .ReturnsAsync(membershipDetails);
            }
        }

        protected void MockRolesDataService()
        {
            RolesDSMock = new Mock<IOrganisationDataService>();
            foreach (var role in RolesTestData.Where(r => r.Active == true))
            {
                RolesDSMock.Setup(x => x.Get(role.RowKey))
                    .ReturnsAsync(role);
            }
        }

        protected void MockRolesMembershipsDataService()
        {
            RolesMembershipsDSMock = new Mock<IRolesMembershipsDataService>();
            foreach (var membership in MembershipDetailsTestData)
            {
                RolesMembershipsDSMock.Setup(x => x.GetRoles(membership.MembershipNumber))
                    .ReturnsAsync(RolesMembershipsTestData.Where(r => r.MembershipNumber == membership.MembershipNumber && r.Active == true));
            }
        }

        protected void MockRolesAppsDataService()
        {
            RolesAppsDSMock = new Mock<IMessageDataService>();
            foreach (var membershipRoles in RolesMembershipsTestData.Where(m => m.Active == true).GroupBy(m => m.MembershipNumber))
            {
                var roles = membershipRoles.Select(m => m.RoleId);
                var appRoles = RolesAppsTestData.Where(r => r.Active == true && roles.Contains(r.OrganisationId));
                foreach (var appRole in appRoles)
                {
                    RolesAppsDSMock.Setup(x => x.Get(roles, appRole.Tenant, appRole.AppId))
                        .ReturnsAsync(appRole);
                }
            }
        }
    }
}