//
// © Copyright 2017 Kevin Pearson
//

using Microsoft.Extensions.Configuration;
using Top100Common;

namespace Top100Import
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            var config = builder.Build();
            var mongoConnectionString = config["MONGO_CONNECTION_STRING"];

            var client = new Store(mongoConnectionString);

            Billboard100.ImportCSV(client, "top100.csv");
        }
    }
}