﻿using System;

namespace SongRequestManager
{
    public class RequestUserTracker
    {
        public int numRequests = 0;
        public DateTime resetTime = DateTime.Now;
    }
}
