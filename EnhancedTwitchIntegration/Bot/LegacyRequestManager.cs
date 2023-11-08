using System;
using System.Collections.Generic;
using System.IO;
using SongRequestManager.SimpleJSON;

namespace SongRequestManager
{
    public class LegacyRequestManager
    {
        public static List<LegacySongRequest> Read(string path)
        {
            List<LegacySongRequest> songs = new List<LegacySongRequest>();
            if (File.Exists(path))
            {
                JSONNode json = JSON.Parse(File.ReadAllText(path));
                if (!json.IsNull)
                {
                    foreach (JSONObject j in json.AsArray)
                    {
                        songs.Add(new LegacySongRequest().FromJson(j));
                    }
                }
            }
            return songs;
        }

        public static void Write(string path, ref List<LegacySongRequest> songs)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            JSONArray arr = new JSONArray();
            foreach (LegacySongRequest song in songs)
            {
                try
                {
                    var songData = song.ToJson();
                    arr.Add(songData);
                }
                catch (Exception)
                {
                    // silent ignore
                }
            }

            File.WriteAllText(path, arr.ToString());
        }
    }

    public class LegacyRequestQueue
    {
        public static List<LegacySongRequest> Songs = new List<LegacySongRequest>();
        private static string requestsPath = Path.Combine(Plugin.DataPath, "SongRequestQueue.dat");
        public static void Read()
        {
            try
            {
                Songs = LegacyRequestManager.Read(requestsPath);
            }
            catch
            {
                RequestBot.Instance.QueueChatMessage("There was an error reading the request queue.");
            }
        }

        public static void Write()
        {
            LegacyRequestManager.Write(requestsPath, ref Songs);
        }
    }

    public class RequestHistory
    {
        public static List<LegacySongRequest> Songs = new List<LegacySongRequest>();
        private static string historyPath = Path.Combine(Plugin.DataPath, "SongRequestHistory.dat");
        public static void Read()
        {
            try
            {
                Songs = LegacyRequestManager.Read(historyPath);
            }
            catch
            {
                RequestBot.Instance.QueueChatMessage("There was an error reading the request history.");
            }
        }

        public static void Write()
        {
            LegacyRequestManager.Write(historyPath, ref Songs);
        }
    }
}