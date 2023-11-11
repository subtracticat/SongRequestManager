using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if OLDVERSION
using TMPro;
#endif

using UnityEngine;
using UnityEngine.UI;
using SongCore;
using IPA.Utilities;
using SongRequestManager.UI;
using BeatSaberMarkupLanguage;
using System.Threading.Tasks;
using System.IO.Compression;
using HMUI;
using SongRequestManager.Config;
using SongRequestManager.Queue;

namespace SongRequestManager
{
    public partial class RequestBot : MonoBehaviour
    {
        public static RequestBot Instance;

        private static Button _requestButton;
        public static bool _refreshQueue = false;

        private static Queue<string> _botMessageQueue = new Queue<string>();

        private static RequestFlowCoordinator _flowCoordinator;
        private static bool _configChanged;

        internal static void SRMButtonPressed()
        {
            FlowCoordinator flowCoordinator;

            if (Plugin.gameMode == Plugin.GameMode.Solo)
            {
                flowCoordinator = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().First();
            }
            else
            {
                flowCoordinator = Resources.FindObjectsOfTypeAll<MultiplayerLevelSelectionFlowCoordinator>().First();
            }

            flowCoordinator.InvokeMethod<object, FlowCoordinator>("PresentFlowCoordinator", _flowCoordinator, null, ViewController.AnimationDirection.Horizontal, false, false);
        }

        internal static void SetTitle(string title)
        {
            _flowCoordinator.SetTitle(title);
        }

        public static void OnLoad()
        {
            try
            {
                var _levelListViewController = Resources.FindObjectsOfTypeAll<SelectLevelCategoryViewController>().Last();

                _levelListViewController.didActivateEvent += _levelListViewController_didActivateEvent;

                if (_levelListViewController)
                {
                    // move the icon control
                    var iconSegmentedControl = _levelListViewController.GetField<IconSegmentedControl, SelectLevelCategoryViewController>("_levelFilterCategoryIconSegmentedControl");
                    ((RectTransform)iconSegmentedControl.transform).anchoredPosition = new Vector2(0, 4.5f);

                    _requestButton = _levelListViewController.CreateUIButton("SRMButton", "PracticeButton", new Vector2(14, -4.5f), new Vector2(15f, 105f),
                        () =>
                        {
                            _requestButton.interactable = false;
                            SRMButtonPressed();
                            _requestButton.interactable = true;
                        },
                        "SRM");

                    _requestButton.ToggleWordWrapping(false);
                    _requestButton.SetButtonTextSize(5f);

                    UIHelper.AddHintText(_requestButton.transform as RectTransform, "Manage the current request queue");

                    UpdateRequestUI();
                    Plugin.Log("Created request button!");
                }
            }
            catch
            {
                Plugin.Log("Unable to create request button");
            }

            // check if flow coordinator has been setup yet
            if (_flowCoordinator == null)
            {
                _flowCoordinator = BeatSaberMarkupLanguage.BeatSaberUI.CreateFlowCoordinator<RequestFlowCoordinator>();
            }

            SongListUtils.Initialize();

            ChatHandler.instance.Init();

            // WriteQueueSummaryToFile();
            // WriteQueueStatusToFile(QueueMessage(QueueConfigManager.Instance.Config.RequestQueueOpen));

            if (Instance) return;
            new GameObject("SongRequestManager").AddComponent<RequestBot>();
        }

        private static void _levelListViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            UpdateRequestUI();
        }

        public static bool AddKeyboard(KEYBOARD keyboard, string keyboardname, float scale = 0.5f)
        {
            try
            {
                string fileContent = File.ReadAllText(Path.Combine(Plugin.DataPath, keyboardname));
                if (fileContent.Length > 0) keyboard.AddKeys(fileContent, scale);
                return true;
            }
            catch
            {
                return false;
                // This is a silent fail since custom keyboards are optional
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

            // Filter out history > 14d ago
            QueueManager.Instance.UpdateSettings(config => config.History.RemoveAll(request => request.RequestTimestamp.AddDays(14) < DateTime.Now));
            QueueConfigManager.Instance.OnChanged += OnConfigChangedEvent;
        }

        //public bool MyChatMessageHandler(TwitchMessage msg)
        //{
        //    string excludefilename = "chatexclude.users";
        //    return RequestBot.Instance && RequestBot.listcollection.contains(ref excludefilename, msg.user.DisplayName.ToLower(), RequestBot.ListFlags.Uncached);
        //}

        private void OnConfigChangedEvent(QueueConfig config)
        {
            _configChanged = true;
        }

        private void OnConfigChanged()
        {
            UpdateRequestUI();

            if (RequestBotListViewController.Instance.isActivated)
            {
                RequestBotListViewController.Instance.UpdateRequestUI(true);
                RequestBotListViewController.Instance.SetUIInteractivity();
            }

            _configChanged = false;
        }

        private void FixedUpdate()
        {
            if (_configChanged)
            {
                OnConfigChanged();
            }
        }

        public enum QueueInsertionStyle
        {
            FIFO, MoveToTop, RoundRobin
        }

        public static async void Play(SongRequest request)
        {
            Plugin.Log($"Processing song request {request.Song.Name}");

            string songId = request.Song.ID;
            string songHash = request.Song.Versions[0].Hash.ToUpper();

            string songName = request.Song.Metadata.SongName;
            string songFolderName = StringNormalizer.RemoveDirectorySymbols($"{songId} ({request.Song.Metadata.SongName} - {request.Song.Metadata.LevelAuthorName})");
            string currentSongDirectory = Path.Combine(Environment.CurrentDirectory, "Beat Saber_Data\\CustomLevels", songFolderName);

            Plugin.Log($"Evaluating levelIDsForHash: {songHash}");
            var rat = SongCore.Collections.levelIDsForHash(songHash);
            bool mapexists = (rat.Count > 0) && (rat[0] != "");

            if (!SongCore.Loader.CustomLevels.ContainsKey(currentSongDirectory) && !mapexists)
            {
                EmptyDirectory(".requestcache", false);

                if (Directory.Exists(currentSongDirectory))
                {
                    EmptyDirectory(currentSongDirectory, true);
                    Plugin.Log($"Deleting {currentSongDirectory}");
                }

                byte[] songZip = null;

                if (!string.IsNullOrEmpty(QueueConfigManager.Instance.Config.offlinepath))
                {
                    // build cache name to check
                    var cacheName = $"{songId}_{songHash}.zip";
                    Plugin.Log($"{QueueConfigManager.Instance.Config.offlinepath} - {cacheName}");
                    var cachePath = Path.Combine(QueueConfigManager.Instance.Config.offlinepath, cacheName);

                    // check if a local cache exists, if so, copy it
                    if (File.Exists(cachePath))
                    {
                        Plugin.Log($"{songId} found in offline cache");
                        using (var stream = File.Open(cachePath, FileMode.Open))
                        {
                            songZip = new byte[stream.Length];
                            await stream.ReadAsync(songZip, 0, (int)stream.Length, System.Threading.CancellationToken.None);
                        }
                    }
                }

                if (songZip == null)
                {
                    var downloadUrl = request.Song.Versions[0].DownloadURL;
                    Plugin.Log($"Downloading song {songId} from {downloadUrl}");
                    songZip = await Plugin.WebClient.DownloadSong(downloadUrl, System.Threading.CancellationToken.None);
                }

                Stream zipStream = new MemoryStream(songZip);
                try
                {
                    // open zip archive from memory stream
                    ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
                    archive.ExtractToDirectory(currentSongDirectory);
                    archive.Dispose();
                }
                catch (Exception e)
                {
                    Plugin.Log($"Unable to extract ZIP! Exception: {e}");
                    return;
                }
                zipStream.Close();

                await Task.Run(async () =>
                {
                    while (!SongCore.Loader.AreSongsLoaded && SongCore.Loader.AreSongsLoading)
                    {
                        await Task.Delay(25);
                    }
                });

                Loader.Instance.RefreshSongs();

                await Task.Run(async () =>
                {
                    while (!SongCore.Loader.AreSongsLoaded && SongCore.Loader.AreSongsLoading)
                    {
                        await Task.Delay(25);
                    }
                });

                EmptyDirectory(".requestcache", true);
            }
            else
            {
                Plugin.Log($"Song {songName} already exists!");
            }

            // Dismiss the song request viewcontroller now
            _flowCoordinator.Dismiss();

            bool success = false;
            Dispatcher.RunCoroutine(SongListUtils.ScrollToLevel(songHash, (s) => success = s, false));

            if (QueueConfigManager.Instance.Config.SendNextSongBeingPlayedtoChat)
            {
                ChatHandler.Send($"{request.Song.Name} ({songId}) requested by {request.RequestedBy} is next!");
            }
        }

        public static void UpdateRequestUI(bool writeSummary = true)
        {
            try
            {
                //if (writeSummary)
                //{
                //    WriteQueueSummaryToFile(); // Write out queue status to file, do it first
                //}

                if (_requestButton != null)
                {
                    var enabled = Plugin.gameMode == Plugin.GameMode.Solo;
                    if (Plugin.gameMode == Plugin.GameMode.Online)
                    {
                        var mpFlowCoordinator = Resources.FindObjectsOfTypeAll<MultiplayerLevelSelectionFlowCoordinator>().First();
                        enabled = mpFlowCoordinator.GetProperty<bool, MultiplayerLevelSelectionFlowCoordinator>("enableCustomLevels");
                    }
                    _requestButton.enabled = enabled;

                    _requestButton.interactable = enabled;

                    if (QueueManager.Instance.Config.Requests.Count == 0)
                    {
                        _requestButton.SetButtonUnderlineColor(Color.red);
                    }
                    else
                    {
                        _requestButton.SetButtonUnderlineColor(Color.green);
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log(ex.ToString());
            }
        }
    }
}