using Zenject;

namespace SongRequestManager.SongData
{
    internal class BeatmapCollector : IInitializable, ILateDisposable
    {
        private readonly BeatmapLevel _beatmap;

        public BeatmapCollector(GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            _beatmap = gameplayCoreSceneSetupData.beatmapLevel;
        }

        public void Initialize()
        {
            Plugin.Log($"New current song: {_beatmap.songName}");
            SongDataManager.CurrentBeatmap = _beatmap;
        }

        public void LateDispose()
        {
            Plugin.Log($"Moving song to previous: {_beatmap.songName}");
            SongDataManager.PreviousBeatmap = _beatmap;
            SongDataManager.CurrentBeatmap = null;
        }
    }
}
