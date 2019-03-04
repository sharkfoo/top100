//
// © Copyright 2017 Kevin Pearson
//

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Top100Common;

namespace Top100Import
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            var config = builder.Build();
            var mongoConnectionString = config["MONGO_CONNECTION_STRING"];

            var client = new Store(mongoConnectionString);

            await Billboard100.ImportCSV(client, "top100.csv");
        }
    }
}