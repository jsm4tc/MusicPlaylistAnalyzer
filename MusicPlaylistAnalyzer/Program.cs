using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MusicPlaylistAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            { 
                Console.WriteLine("MusicPlaylistAnalyzer <music_playlist_file_path> <report_file_path>");
                Environment.Exit(1);
            }

            string musicDataFilePath = args[0];
            string reportFilePath = args[1];

            List<Songs> SongsList = null;
            try
            {
                SongsList = MusicStatsLoader.Load(musicDataFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(2);
            }

            var report = MusicStatsReport.GenerateText(SongsList);

            try
            {
                System.IO.File.WriteAllText(reportFilePath, report);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(3);
            }
        }
    }
    public class Songs
    {
        public string Name;
        public string Artist;
        public string Album;
        public string Genre;
        public int Size;
        public int Time;
        public int Year;
        public int Plays;

        public Songs(string name, string artist, string album, string genre, int size, int time, int year, int plays)
        {
            Name = name;
            Artist = artist;
            Album = album;
            Genre = genre;
            Size = size;
            Time = time;
            Year = year;
            Plays = plays;
        }
    }
    public static class MusicStatsLoader
    {
        private static int NumItemsInRow = 8;

        public static List<Songs> Load(string musicDataFilePath) {
            List<Songs> SongsList = new List<Songs>();

            try
            {
                using (var reader = new StreamReader(musicDataFilePath))
                {
                    int lineNumber = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        lineNumber++;
                        if (lineNumber == 1) continue;

                        var values = line.Split('\t');

                        if (values.Length != NumItemsInRow)
                        {
                            throw new Exception($"Row {lineNumber} contains {values.Length} values. It should contain {NumItemsInRow}.");
                        }
                        try
                        {
                            string name = values[0];
                            string artist = values[1];
                            string album = values[2];
                            string genre = values[3];
                            int size = Int32.Parse(values[4]);
                            int time = Int32.Parse(values[5]);
                            int year = Int32.Parse(values[6]);
                            int plays = Int32.Parse(values[7]);
                            Songs songs = new Songs(name, artist, album, genre, size, time, year, plays);
                            SongsList.Add(songs);
                        }
                        catch (FormatException e)
                        {
                            throw new Exception($"Row {lineNumber} contains invalid data. ({e.Message})");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to open {musicDataFilePath} ({e.Message}).");
            }

            return SongsList;
        }
    }
    public static class MusicStatsReport
    {
        public static string GenerateText(List<Songs> SongsList)
        {
            string report = "Music Analyzer Report\n\n";

            if (SongsList.Count() < 1)
            {
                report += "No data is available.\n";

                return report;
            }

            report += "Songs that received 200 or more plays: \n";
            var q1 = from Songs in SongsList where Songs.Plays > 200 select Songs.Name;
            if (q1.Count() > 0)
            {
                foreach (var name in q1)
                {
                    report += name + ",";
                }
                report.TrimEnd(',');
                report += "\n";
            }

            return report;
        }
    }
}
