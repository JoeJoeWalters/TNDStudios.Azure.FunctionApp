using Microsoft.Extensions.Logging;
using TNDStudios.Azure.FunctionApp.Mocking.Mocks;

namespace TNDStudios.Azure.FunctionApp.Mocking.Factories
{
    public static class TestLoggerFactory
    {
        public static ILogger CreateLogger()
            => new TestLogger();
    }
}
