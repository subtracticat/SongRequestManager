using System;

namespace SongRequestManager.Queue
{
    [Flags]
    public enum RequestStatus
    {
        Invalid,
        Queued,
        Blacklisted,
        Skipped,
        Played,
        Wrongsong,
        SongSearch,
        Deleted,
    }
}
