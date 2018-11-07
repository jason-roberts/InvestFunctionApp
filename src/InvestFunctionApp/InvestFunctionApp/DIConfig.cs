using Autofac;
using AzureFunctions.Autofac.Configuration;

namespace InvestFunctionApp
{
    public class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<NaiveInvestementAllocator>().As<IInvestementAllocator>(); // Naive

            }, functionName);
        }
    }
}
