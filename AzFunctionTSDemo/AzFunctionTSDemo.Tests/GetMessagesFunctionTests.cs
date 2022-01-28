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
    public class GetMessagesFunctionTests : BaseTest
    {        
        private Mock<ILogger<GetMessagesFunction>> LoggerMock;
        private readonly GetMessagesFunction _azFunction;

        public GetMessagesFunctionTests()
        {
            MockDependencies();
            LoggerMock = GetMockedILogger<GetMessagesFunction>();

            _azFunction = new GetMessagesFunction(CompanyDSMock!.Object, MessageDSMock!.Object, LoggerMock!.Object);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[]
            {
                "test"
            };
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public Task TestRunGetMessages(string testCase)
        {
            //var actual = await _azFunction.Run(req);

            //Assert.Equal(expectedResult.Value.GetType().ToString(), ((OkObjectResult)actual).Value.GetType().ToString());

            //if (expectedResult.Value is ContinueResponse expectedContinueResponse)
            //{
            //    var actualContinueResponse = (ContinueResponse)((OkObjectResult)actual).Value;

            //    Assert.Equal(expectedContinueResponse.UserMessage, actualContinueResponse.UserMessage);
            //    Assert.Equal(expectedContinueResponse.Version, actualContinueResponse.Version);
            //    Assert.Equal(expectedContinueResponse.Action, actualContinueResponse.Action);
            //}
            //else
            //{
            //    var expectedErrorResponse = (ErrorResponse)expectedResult.Value;
            //    var actualErrorResponse = (ErrorResponse)((OkObjectResult)actual).Value;

            //    Assert.Equal(expectedErrorResponse.UserMessage, actualErrorResponse.UserMessage);
            //    Assert.Equal(expectedErrorResponse.Version, actualErrorResponse.Version);
            //    Assert.Equal(expectedErrorResponse.Action, actualErrorResponse.Action);
            //    Assert.Equal(expectedErrorResponse.Status, actualErrorResponse.Status);
            //}

            //VerifyServicesCalled(req);
            return Task.CompletedTask;
        }

        private void VerifyServicesCalled(GetMessagesRequestDto req)
        {
            LoggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == $"CheckUserMembership called"),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }
    }
}
