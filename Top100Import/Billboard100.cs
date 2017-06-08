//
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
            string[] split = line.Split(',');
            if (split.Length == 5)
            {
                song.Title = split[0].Replace(DELIM, ",").Trim();
                song.Artist = split[1].Replace(DELIM, ",").Trim();
                song.Year = Int32.Parse(split[2].Replace(DELIM, ",").Trim());
                song.Number = Int32.Parse(split[3].Replace(DELIM, ",").Trim());
                int own = Int32.Parse(split[4].Replace(DELIM, ",").Trim());
                if (own == 1)
                {
                    song.Own = true;
                }
                else if (own == 0)
                {
                    song.Own = false;
                }
                else
                {
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
                string line = "";
                while ((line = csvFile.ReadLine()) != null)
                {
                    try
                    {
                        var ret = client.InsertAsync(ParseCSVLine(line));
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
