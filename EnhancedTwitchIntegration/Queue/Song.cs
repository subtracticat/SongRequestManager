using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongRequestManager.Queue
{
    public class Song
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int DurationSeconds { get; set; }
    }
}
