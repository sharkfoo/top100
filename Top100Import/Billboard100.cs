﻿//
// © Copyright 2017 Kevin Pearson
//

using System;
using System.IO;
using System.Text;
using Top100Common;

namespace Top100Import
{
    public class Billboard100
    {
        private const string DELIM = "___COMMA_DELIM___";

        public static Song ParseCSVLine(string line)
        {
            var song = new Song();

            line = line.Replace(@"\,", DELIM);
            var split = line.Split(',');
            if (split.Length == 5)
            {
                song.Title = split[0].Replace(DELIM, ",").Trim();
                song.Artist = split[1].Replace(DELIM, ",").Trim();
                song.Year = int.Parse(split[2].Replace(DELIM, ",").Trim());
                song.Number = int.Parse(split[3].Replace(DELIM, ",").Trim());
                var own = int.Parse(split[4].Replace(DELIM, ",").Trim());
                switch (own)
                {
                    case 1:
                        song.Own = true;
                        break;
                    case 0:
                        song.Own = false;
                        break;
                    default:
                        Console.WriteLine($"ERROR: own={own}");
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Console.WriteLine($"ERROR:  line={line}");
                throw new ArgumentOutOfRangeException();
            }
            return song;
        }

        public static void ImportCSV(IStore client, string file)
        {
            var fileStream = new FileStream(file, FileMode.Open);
            using (var csvFile = new StreamReader(fileStream, Encoding.UTF8))
            {
                var line = "";
                while ((line = csvFile.ReadLine()) != null)
                {
                    try
                    {
                        var ret = client.CreateAsync(ParseCSVLine(line));
                        if (ret.Result != null)
                        {
                            Console.WriteLine($"Added song: {line}");
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"ERROR:  line={line}, ex={e}");
                        break;
                    }
                }
            }
        }
    }
}
