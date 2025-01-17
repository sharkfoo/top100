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
            var input_file = "";

            if (args.Length != 1)
            {
                input_file = "top100.csv";
                //Console.WriteLine("");
                //Console.WriteLine($"\tUsage: {args[0]} <csv file>");
                //Console.WriteLine("");
            }
            else
            {
                input_file = args[0];
            }
            var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            var config = builder.Build();
            var mongoConnectionString = config["MONGO_CONNECTION_STRING"];

            var client = new Store(mongoConnectionString);

            await Billboard100.ImportCSV(client, args[0]);
        }
    }
}