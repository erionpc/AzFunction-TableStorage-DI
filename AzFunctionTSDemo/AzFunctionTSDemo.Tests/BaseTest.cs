using AzFunctionTSDemo.Abstractions;
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
    public class BaseTest
    {
        protected static string AppSettingsFileName => "appsettings.test.json";

        protected static readonly List<Company> CompanyTestData = new()
        {
            new Company() { RowKey = "1", Name = "Company1", Active = true },
            new Company() { RowKey = "2", Name = "Company2", Active = true },
            new Company() { RowKey = "3", Name = "Role3", Active = false }
        };

        protected static readonly List<Message> MessagesTestData = new()
        {
            new Message() { CompanyId = "1", Content = "sample content 1", Timestamp = DateTimeOffset.Now.AddHours(-1) },
            new Message() { CompanyId = "1", Content = "sample content 2", Timestamp = DateTimeOffset.Now.AddHours(-2) },
            new Message() { CompanyId = "1", Content = "sample content 3", Timestamp = DateTimeOffset.Now.AddHours(-0.5) },
            new Message() { CompanyId = "2", Content = "sample content 4", Timestamp = DateTimeOffset.Now.AddHours(-0.4) },
            new Message() { CompanyId = "2", Content = "sample content 5", Timestamp = DateTimeOffset.Now.AddHours(-0.3) },
            new Message() { CompanyId = "2", Content = "sample content 6", Timestamp = DateTimeOffset.Now.AddHours(-0.2) },
            new Message() { CompanyId = "3", Content = "sample content 7", Timestamp = DateTimeOffset.Now.AddHours(-0.1) },
            new Message() { CompanyId = "3", Content = "sample content 8", Timestamp = DateTimeOffset.Now.AddHours(-0.7) },
            new Message() { CompanyId = "3", Content = "sample content 9", Timestamp = DateTimeOffset.Now.AddHours(-0.8) },
        };

        protected IConfigurationRoot Configuration { get; }

        protected Mock<ICompanyDataService>? CompanyDSMock;
        protected Mock<IMessageDataService>? MessageDSMock;

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

        protected void MockDependencies()
        {
            MockCompanyDataService();
            MockMessagesDataService();
        }

        protected void MockCompanyDataService()
        {
            CompanyDSMock = new Mock<ICompanyDataService>();
            foreach (var company in CompanyTestData.Where(r => r.Active == true))
            {
                CompanyDSMock.Setup(x => x.Get(company.RowKey))
                    .ReturnsAsync(company);
            }
        }

        protected void MockMessagesDataService()
        {
            MessageDSMock = new Mock<IMessageDataService>();
            
            MessageDSMock.Setup(x => 
                x.Get(It.IsAny<string?>(),
                      It.IsAny<DateTimeOffset?>(), 
                      It.IsAny<DateTimeOffset?>(), 
                      It.IsAny<bool?>()))
                    .ReturnsAsync((string? companyId, 
                                   DateTimeOffset? fromTime, 
                                   DateTimeOffset? toTime, 
                                   bool? processed) => 
                        MessagesTestData.Where(m => 
                            !string.IsNullOrWhiteSpace(companyId) ? m.CompanyId == companyId : true &&
                            fromTime.HasValue ? m.Timestamp >= fromTime : true &&
                            toTime.HasValue ? m.Timestamp <= toTime : true &&
                            processed.HasValue ? m.Processed == processed : true));
        }
    }
}
