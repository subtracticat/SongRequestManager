using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongRequestManager.Queue
{
    public class RequestHistory
    {
        private static RequestHistory instance;

        public static RequestHistory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RequestHistory();
                }

                return instance;
            }
        }

        public List<SongRequest> Requests { get; private set; } = new List<SongRequest>();
    }
}
