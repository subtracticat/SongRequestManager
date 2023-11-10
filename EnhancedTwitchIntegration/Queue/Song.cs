using System.Collections.Generic;
using Newtonsoft.Json;

namespace SongRequestManager.Queue
{
    public class Song
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Ranked { get; set; }
        public SongMetadata Metadata { get; set; }
        public List<SongVersion> Versions { get; set; }
        public SongStats Stats { get; set; }
    }

    public class SongMetadata
    {
        public float BPM { get; set; }
        public float Duration { get; set; }
        public string SongName { get; set; }
        public string SongSubName { get; set; }
        public string SongAuthorName { get; set; }
        public string LevelAuthorName { get; set; }
    }

    public class SongVersion
    {
        public string Hash { get; set; }
        public string DownloadURL { get; set; }
        public string CoverURL { get; set; }
        public string PreviewURL { get; set; }
    }

    public class SongStats
    {
        public int Plays { get; set; }
        public int Downloads { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public int Reviews { get; set; }
        public float Score { get; set; }
    }
}
