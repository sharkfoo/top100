//
// © Copyright 2017 Kevin Pearson
//

using Microsoft.Extensions.Configuration;

namespace Top100Import
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            var config = builder.Build();
            string mongoConnectionString = config["MONGO_CONNECTION_STRING"];

            var client = new Top100DBClient(mongoConnectionString);

            Billboard100.ImportCSV(client, "top100.csv");
        }
    }
}