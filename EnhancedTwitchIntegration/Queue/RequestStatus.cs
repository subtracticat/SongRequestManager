namespace SongRequestManager.Queue
{
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
