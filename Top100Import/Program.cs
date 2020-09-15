//
// © Copyright 2017,2020 Kevin Pearson
//

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Top100Common;

namespace Top100Import
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("");
                Console.WriteLine($"\tUsage: {args[0]} <csv file>");
                Console.WriteLine("");
                return;
            }
            var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            var config = builder.Build();
            var mongoConnectionString = config["MONGO_CONNECTION_STRING"];

            var client = new Store(mongoConnectionString);

            await Billboard100.ImportCSV(client, args[0]);
        }
    }
}