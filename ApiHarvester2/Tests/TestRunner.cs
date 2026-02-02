using System;
using System.Threading.Tasks;
namespace ApiHarvester.Tests
{
    public static class TestRunner
    {
        public static async Task RunAllAsync()
        {
            Console.WriteLine("Running MultiSourceProcessorTests..."); MultiSourceProcessorTests.Run(); Console.WriteLine("MultiSourceProcessorTests passed.");

            Console.WriteLine("Running SpaceXSourceTests...");
            await SpaceXSourceTests.RunAsync();
            Console.WriteLine("SpaceXSourceTests passed.");

            Console.WriteLine("Running OpenMeteoSourceTests...");
            await OpenMeteoSourceTests.RunAsync();
            Console.WriteLine("OpenMeteoSourceTests passed.");

            Console.WriteLine("Running CoinGeckoSourceTests...");
            await CoinGeckoSourceTests.RunAsync();
            Console.WriteLine("CoinGeckoSourceTests passed.");

            Console.WriteLine("All tests passed.");
        }
    }
}