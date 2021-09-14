using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Word_Replace
{
    class Program
    {
        public static string dirPath = AppDomain.CurrentDomain.BaseDirectory;
        static void Main(string[] args)
        {
            try
            {
                List<Words> words = ReadCSV();

                var watch = Stopwatch.StartNew();

                string inputFile = File.ReadAllText(dirPath + AppSettings.GetAppSetting<string>("InputFile"));
                string content = string.Empty;
                words.ForEach(word =>
                {
                    word.Count = Regex.Matches(inputFile, word.English.ToString()).Count;
                    inputFile = inputFile.Replace(word.English, word.French);

                    //Find the Upper case words and replace the assoicate french word to upper.
                    word.Count += Regex.Matches(inputFile, UppercaseFirst(word.English)).Count;

                    inputFile = inputFile.Replace(UppercaseFirst(word.English), UppercaseFirst(word.French));
                });
                File.WriteAllText(dirPath + AppSettings.GetAppSetting<string>("OutputFile"), inputFile);

                watch.Stop();

                //Process.txt
                var memory = 0.0;
                Process proc = Process.GetCurrentProcess();
                memory = proc.PrivateMemorySize64 / (1024 * 1024);
                proc.Dispose();

                var processtxtPath = dirPath + AppSettings.GetAppSetting<string>("Output");
                File.WriteAllText(processtxtPath + "performance.txt", String.Empty);
                // Append text to an existing file named "WriteLines.txt".
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(processtxtPath, "performance.txt"), true))
                {
                    outputFile.WriteLine(string.Format("Time to process: {0} minutes {1} seconds", watch.Elapsed.Minutes, watch.Elapsed.Seconds));
                    outputFile.WriteLine(string.Format("Memory used: {0} mb", memory));
                }

                //Frequency.csv file geneate
                WriteFiles(words);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static List<Words> ReadCSV()
        {
            List<Words> words = new List<Words>();
            try
            {
                var csvPath = string.Format("{0}{1}", dirPath, AppSettings.GetAppSetting<string>("CsvReadPath"));
                string directoryName = Path.GetDirectoryName(csvPath);

                var files = Directory.GetFiles(directoryName).Where(p => p.EndsWith(".csv")).ToArray();


                foreach (var file in files)
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ",",
                        Encoding = Encoding.UTF8,
                        HasHeaderRecord = false
                    };

                    FileStream fStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using (TextReader reader = new StreamReader(fStream))
                    {
                        var csv = new CsvReader(reader, config);
                        words = csv.GetRecords<Words>().ToList();
                        reader.Close();
                        reader.Dispose();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return words;
        }

        public static void WriteFiles(List<Words> words)
        {
            List<WordsCsv> wordsCsvs = new List<WordsCsv>();
            words.ForEach(x =>
            {
                var wordsCsv = new WordsCsv
                {
                    English = x.English,
                    French = x.French,
                    Count = x.Count
                };
                wordsCsvs.Add(wordsCsv);
            });
            var csvPath = string.Format("{0}{1}", dirPath, AppSettings.GetAppSetting<string>("CsvWritePath"));
            string directoryName = Path.GetDirectoryName(csvPath);

            try
            {
                using (var writer = new StreamWriter(csvPath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<WordsCsv>();
                    csv.NextRecord();
                    foreach (var word in wordsCsvs)
                    {
                        csv.WriteRecord(word);
                        csv.NextRecord();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string UppercaseFirst(string value)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
}
