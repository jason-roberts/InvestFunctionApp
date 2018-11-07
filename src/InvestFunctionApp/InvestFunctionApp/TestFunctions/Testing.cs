using System;
using Microsoft.Extensions.Logging;

namespace InvestFunctionApp.TestFunctions
{
    internal class Testing
    {
        internal const string TestFunctionRoute = "testing";
        internal const string TestEnvironmentConfigKey = "InvestFunctionApp.IsTestEnvironment";

        /// <summary>
        /// Testing functions should be disabled in Azure, this is an additional level of checking.
        /// </summary>
        internal static void AssertInTestEnvironment(ILogger log)
        {
            var value = Environment.GetEnvironmentVariable(TestEnvironmentConfigKey);

            var isTestingEnvonment = value != null && value == "true";

            if (!isTestingEnvonment)
            {
                log.LogError("This function should be disabled in non-testing environments but was called. Check that all testing functions are disabled in production.");
                throw new InvalidOperationException();
            }
        }
    }
}
