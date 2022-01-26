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
    public class CheckUserMembershipFunctionTests : BaseTest
    {        
        private Mock<ILogger<CheckUserMembershipFunction>> _loggerMock;
        private readonly CheckUserMembershipFunction _azFunction;

        public CheckUserMembershipFunctionTests()
        {
            MockDependencies();
            _azFunction = new CheckUserMembershipFunction(MembershipDetailsDSMock!.Object, _loggerMock!.Object);
        }

        protected override void MockDependencies()
        {
            base.MockMembershipDetailsDataService();

            _loggerMock = GetMockedILogger<CheckUserMembershipFunction>();
        }

        public static IEnumerable<object[]> GetTestData()
        {
            string errorText = "Login failed: Please check your details and try again, if problem persists please contact our support team via email at sign_in_support@londonlibrary.co.uk";

            yield return new object[]
            {
                "test membership found",
                new GetMessagesRequestDto() { MembershipNumber = MembershipDetailsTestData[0].MembershipNumber, Email = MembershipDetailsTestData[0].Email },
                new OkObjectResult(new ContinueResponse())
            };

            yield return new object[]
            {
                "test membership not found",
                new GetMessagesRequestDto() { MembershipNumber = MembershipDetailsTestData[0].MembershipNumber, Email = MembershipDetailsTestData[1].Email },
                new OkObjectResult(new ErrorResponse(errorText))
            };

            yield return new object[]
            {
                "test invalid request",
                new GetMessagesRequestDto() { MembershipNumber = MembershipDetailsTestData[0].MembershipNumber, Email = MembershipDetailsTestData[1].Email },
                new OkObjectResult(new ErrorResponse(errorText))
            };
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task TestRunCheckUserMembership(string testCase, GetMessagesRequestDto req, OkObjectResult expectedResult)
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

            VerifyServicesCalled(req);
        }

        private void VerifyServicesCalled(GetMessagesRequestDto req)
        {
            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == $"CheckUserMembership called"),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            MembershipDetailsDSMock?.Verify(x => x.Get(req.MembershipNumber, req.Email), Times.Once);
        }
    }
}
