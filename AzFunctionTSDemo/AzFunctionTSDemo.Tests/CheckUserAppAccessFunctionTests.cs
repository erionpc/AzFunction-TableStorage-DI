using AzFunctionTSDemo.Abstractions;
using AzFunctionTSDemo.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzFunctionTSDemo.Tests
{
    public class CheckUserAppAccessFunctionTests : BaseTest
    {        
        private Mock<ILogger<GetMessagesFunction>> _loggerMock;
        private readonly GetMessagesFunction _azFunction;

        public CheckUserAppAccessFunctionTests()
        {
            MockDependencies();
            _azFunction = new GetMessagesFunction(RolesMembershipsDSMock!.Object, RolesDSMock!.Object, RolesAppsDSMock!.Object, _loggerMock!.Object);
        }

        protected override void MockDependencies()
        {
            MockRolesMembershipsDataService();
            MockRolesDataService();
            MockRolesAppsDataService();

            _loggerMock = GetMockedILogger<GetMessagesFunction>();
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var userNotLinkedToAnyRoles = MembershipDetailsTestData.FirstOrDefault(x => !RolesMembershipsTestData.Any(r => r.MembershipNumber == x.MembershipNumber && r.Active == true));
            var userLinkedToNonExistingRole = RolesMembershipsTestData.FirstOrDefault(x => x.Active == true && !RolesTestData.Select(r => r.RowKey).Contains(x.RoleId));
            var userLinkedToInactiveRole = RolesMembershipsTestData.FirstOrDefault(x => x.Active == true && RolesTestData.Where(r => r.Active == false).Select(r => r.RowKey).Contains(x.RoleId));
            var inactiveAccessToApp = RolesAppsTestData.FirstOrDefault(x => x.Active == false);
            var userWithInactiveAccessToApp = RolesMembershipsTestData.FirstOrDefault(x => x.RoleId == inactiveAccessToApp!.OrganisationId);
            var activeAccessToApp = RolesAppsTestData.FirstOrDefault(x => x.Active == true && RolesTestData.Where(r => r.Active == true).Select(r => r.RowKey).Contains(x.OrganisationId));
            var userWithAccessToApp = RolesMembershipsTestData.FirstOrDefault(x => x.RoleId == activeAccessToApp!.OrganisationId);

            yield return new object[]
            {
                "test invalid request",
                new MessageDto() { MembershipNumber = MembershipDetailsTestData[0].MembershipNumber, AppId = "", Tenant = RolesAppsTestData[0].Tenant },
                new OkObjectResult(new ErrorResponse("The request is invalid"))
            };
            yield return new object[]
            {
                "test user with inactive link to roles",
                new MessageDto() { MembershipNumber = userNotLinkedToAnyRoles!.MembershipNumber, AppId = RolesAppsTestData[0].AppId, Tenant = RolesAppsTestData[0].Tenant },
                new OkObjectResult(new ErrorResponse("The user is not linked to any roles"))
            };
            yield return new object[]
            {
                "test user with link to non existing role",
                new MessageDto() { MembershipNumber = userLinkedToNonExistingRole!.MembershipNumber, AppId = RolesAppsTestData[0].AppId, Tenant = RolesAppsTestData[0].Tenant },
                new OkObjectResult(new ErrorResponse("The user is not linked to any roles"))
            };
            yield return new object[]
            {
                "test user with link to inactive role",
                new MessageDto() { MembershipNumber = userLinkedToInactiveRole!.MembershipNumber, AppId = RolesAppsTestData[0].AppId, Tenant = RolesAppsTestData[0].Tenant },
                new OkObjectResult(new ErrorResponse("The user is not linked to any roles"))
            };
            yield return new object[]
            {
                "test user with inactive access to app",
                new MessageDto() { MembershipNumber = userWithInactiveAccessToApp!.MembershipNumber, AppId = inactiveAccessToApp!.AppId, Tenant = inactiveAccessToApp!.Tenant },
                new OkObjectResult(new ErrorResponse("The user doesn't have access to the app"))
            };
            yield return new object[]
            {
                "test user with no access to app",
                new MessageDto() { MembershipNumber = userWithAccessToApp!.MembershipNumber, AppId = "some value", Tenant = activeAccessToApp!.Tenant },
                new OkObjectResult(new ErrorResponse("The user doesn't have access to the app"))
            };
            yield return new object[]
            {
                "test user with access to app",
                new MessageDto() { MembershipNumber = userWithAccessToApp!.MembershipNumber, AppId = activeAccessToApp!.AppId, Tenant = activeAccessToApp!.Tenant },
                new OkObjectResult(new ContinueResponse())
            };
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task TestRunCheckUserAppAccess(string testCase, MessageDto req, OkObjectResult expectedResult)
        {
            var actual = await _azFunction.Run(req);

            Assert.Equal(expectedResult.Value.GetType().ToString(), ((OkObjectResult)actual).Value.GetType().ToString());

            if (expectedResult.Value is ContinueResponse expectedContinueResponse)
            {
                var actualContinueResponse = (ContinueResponse)((OkObjectResult)actual).Value;

                Assert.Equal(expectedContinueResponse.UserMessage, actualContinueResponse.UserMessage);
                Assert.Equal(expectedContinueResponse.Version, actualContinueResponse.Version);
                Assert.Equal(expectedContinueResponse.Action, actualContinueResponse.Action);
            }
            else
            {
                var expectedErrorResponse = (ErrorResponse)expectedResult.Value;
                var actualErrorResponse = (ErrorResponse)((OkObjectResult)actual).Value;

                Assert.Equal(expectedErrorResponse.UserMessage, actualErrorResponse.UserMessage);
                Assert.Equal(expectedErrorResponse.Version, actualErrorResponse.Version);
                Assert.Equal(expectedErrorResponse.Action, actualErrorResponse.Action);
                Assert.Equal(expectedErrorResponse.Status, actualErrorResponse.Status);
            }

            VerifyServicesCalled(req, (FunctionResponse)((OkObjectResult)actual).Value);
        }

        private void VerifyServicesCalled(MessageDto req, FunctionResponse resp)
        {
            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == $"CheckUserAppAccess called"),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            if (resp is ContinueResponse)
            {
                RolesMembershipsDSMock?.Verify(x => x.GetRoles(req.MembershipNumber), Times.Once);
                RolesDSMock?.Verify(x => x.Get(req.MembershipNumber));
                RolesAppsDSMock?.Verify(x => x.Get(It.IsAny<IEnumerable<string>>(), It.IsAny<string?>(), It.IsAny<string>()), Times.Once);
            }
        }
    }
}