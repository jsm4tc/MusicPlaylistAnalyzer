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
            var q1 = from Songs in SongsList where Songs.Plays > 200 select Songs;
            if (q1.Count() > 0)
            {
                foreach (var name in q1)
                {
                    report += "Name: " + name.Name + ", Artist: " + name.Artist + ", Album: " + name.Album + ", Genre: " + name.Genre + ", Size: " + name.Size + ", Time: " + name.Time + ", Year: " + name.Year + ", Plays: " + name.Plays + "\n";
                }
                report += "\n";
            }

            int altnum = 0;
            var q2 = from Songs in SongsList where Songs.Genre == "Alternative" select Songs;
            if (q2.Count() > 0)
            {
                foreach (var alt in q2)
                {
                    altnum++;
                }
            }
            report += "Number of Alternative Songs: " + altnum + "\n\n";

            int rapnum = 0;
            var q3 = from Songs in SongsList where Songs.Genre == "Hip-Hop/Rap" select Songs;
            if (q3.Count() > 0)
            {
                foreach (var rap in q3)
                {
                    rapnum++;
                }
            }
            report += "Number of Hip-Hop/Rap songs: " + rapnum + "\n\n";

            report += "Songs from the album Welcome to the Fishbowl: \n";
            var q4 = from Songs in SongsList where Songs.Album == "Welcome to the Fishbowl" select Songs;
            if (q4.Count() > 0)
            {
                foreach (var fish in q4)
                {
                    report += "Name: " + fish.Name + ", Artist: " + fish.Artist + ", Album: " + fish.Album + ", Genre: " + fish.Genre + ", Size: " + fish.Size + ", Time: " + fish.Time + ", Year: " + fish.Year + ", Plays: " + fish.Plays + "\n";
                }
                report += "\n";
            }

            report += "Songs from before 1970: \n";
            var q5 = from Songs in SongsList where Songs.Year < 1970 select Songs;
            if (q5.Count() > 0)
            {
                foreach (var prior in q5)
                {
                    report += "Name: " + prior.Name + ", Artist: " + prior.Artist + ", Album: " + prior.Album + ", Genre: " + prior.Genre + ", Size: " + prior.Size + ", Time: " + prior.Time + ", Year: " + prior.Year + ", Plays: " + prior.Plays + "\n";
                }
                report += "\n";
            }

            report += "Song names longer than 85 characters: \n";
            var q6 = from Songs in SongsList where Songs.Name.Length > 85 select Songs;
            if (q6.Count() > 0)
            {
                foreach (var big in q6)
                {
                    report += big.Name + " ";
                }
                report += "\n\n";
            }

            report += "Longest song: \n";
            int maxlen = 0;
            int newlen = 0;
            var q7 = from Songs in SongsList where Songs.Time > 0 select Songs;
            if (q7.Count() > 0)
            {
                foreach (var len in q7)
                {
                    newlen = len.Time;
                    if (newlen > maxlen)
                    {
                        maxlen = newlen;
                    }
                }
                foreach (var sort in q7)
                {
                    if (maxlen == sort.Time)
                    {
                        report += "Name: " + sort.Name + ", Artist: " + sort.Artist + ", Album: " + sort.Album + ", Genre: " + sort.Genre + ", Size: " + sort.Size + ", Time: " + sort.Time + ", Year: " + sort.Year + ", Plays: " + sort.Plays + "\n";
                    } 
                }
            }

            return report;
        }
    }
}
