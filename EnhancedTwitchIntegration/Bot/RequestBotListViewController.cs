using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using SongRequestManager.UI;
using SongRequestManager.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace SongRequestManager
{

    public class RequestBotListViewController : ViewController, TableView.IDataSource
    {
        public static RequestBotListViewController Instance;

        private bool confirmDialogActive = false;

        // ui elements
        private Button _pageUpButton;
        private Button _pageDownButton;
        private Button _playButton;
        private Button _skipButton;
        private Button _pingButton;
        private Button _blacklistButton;
        private Button _historyButton;
        private Button _queueButton;
        private Button _websocketConnectButton;

        private TableView _songListTableView;
        private LevelListTableCell _requestListTableCellInstance;

        private TextMeshProUGUI _CurrentSongName;
        private TextMeshProUGUI _CurrentSongName2;

        private HoverHint _historyHintText;

        private SongPreviewPlayer _songPreviewPlayer;

        private int _requestRow = 0;
        private int _historyRow = 0;
        private int _lastSelection = -1;

        private bool _isShowingHistory = false;

        private int _selectedRow
        {
            get => _isShowingHistory ? _historyRow : _requestRow;
            set
            {
                if (_isShowingHistory)
                {
                    _historyRow = value;
                }
                else
                {
                    _requestRow = value;
                }
            }
        }

        private KEYBOARD CenterKeys;

        string SONGLISTKEY = @"
[blacklist last]/0'!block/current%CR%'

[fun +]/25'!fun/selected/toggle%CR%' [hard +]/25'!hard/selected/toggle%CR%'
[dance +]/25'!dance/selected/toggle%CR%' [chill +]/25'!chill/selected/toggle%CR%'
[brutal +]/25'!brutal/selected/toggle%CR%' 

[Random song!]/0'!decklist draw%CR%'";

        public void Awake()
        {
            Instance = this;
        }

        public void ColorDeckButtons(KEYBOARD kb, Color basecolor, Color Present, bool setSprite = false)
        {
            if (RequestQueue.Current.Data.History.Count == 0)
            {
                return;
            }

            //foreach (KEYBOARD.KEY key in kb.keys)
            //{
            //    foreach (var item in RequestBot.deck)
            //    {
            //        string search = $"!{item.Key}/selected/toggle";
            //        if (key.value.StartsWith(search))
            //        {
            //            string deckname = item.Key.ToLower() + ".deck";
            //            Color color = (RequestBot.listcollection.contains(ref deckname, CurrentlySelectedSong().song["id"].Value)) ? Present : basecolor;

            //            key.mybutton.HighlightDeckButton(color);
            //        }
            //    }
            //}
        }

        static public Song currentsong = null;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {

            if (firstActivation)
            {
                if (!SongCore.Loader.AreSongsLoaded)
                {
                    SongCore.Loader.SongsLoadedEvent += SongLoader_SongsLoadedEvent;
                }

                // get table cell instance
                _requestListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First((LevelListTableCell x) => x.name == "LevelListTableCell");

                // initialize Yes/No modal
                YesNoModal.instance.Setup();

                _songPreviewPlayer = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().FirstOrDefault();

                RectTransform container = new GameObject("RequestBotContainer", typeof(RectTransform)).transform as RectTransform;
                container.SetParent(rectTransform, false);

                #region TableView Setup and Initialization
                var go = new GameObject("SongRequestTableView", typeof(RectTransform));
                go.SetActive(false);

                go.AddComponent<ScrollRect>();
                go.AddComponent<Touchable>();
                go.AddComponent<EventSystemListener>();

                ScrollView scrollView = go.AddComponent<ScrollView>();

                _songListTableView = go.AddComponent<TableView>();
                go.AddComponent<RectMask2D>();
                _songListTableView.transform.SetParent(container, false);

                _songListTableView.SetField("_preallocatedCells", new TableView.CellsGroup[0]);
                _songListTableView.SetField("_isInitialized", false);
                _songListTableView.SetField("_scrollView", scrollView);

                var viewport = new GameObject("Viewport").AddComponent<RectTransform>();
                viewport.SetParent(go.GetComponent<RectTransform>(), false);
                go.GetComponent<ScrollRect>().viewport = viewport;
                (viewport.transform as RectTransform).sizeDelta = new Vector2(70f, 70f);

                RectTransform content = new GameObject("Content").AddComponent<RectTransform>();
                content.SetParent(viewport, false);

                scrollView.SetField("_contentRectTransform", content);
                scrollView.SetField("_viewport", viewport);

                _songListTableView.SetDataSource(this, false);

                _songListTableView.LazyInit();

                go.SetActive(true);

                (_songListTableView.transform as RectTransform).sizeDelta = new Vector2(70f, 70f);
                (_songListTableView.transform as RectTransform).anchoredPosition = new Vector2(3f, 0f);

                _songListTableView.didSelectCellWithIdxEvent += DidSelectRow;

                _pageUpButton = UIHelper.CreateUIButton("SRMPageUpButton",
                    container,
                    "PracticeButton",
                    new Vector2(0f, 38.5f),
                    new Vector2(15f, 7f),
                    () => { scrollView.PageUpButtonPressed(); },
                    "˄");
                Destroy(_pageUpButton.GetComponentsInChildren<ImageView>().FirstOrDefault(x => x.name == "Underline"));

                _pageDownButton = UIHelper.CreateUIButton("SRMPageDownButton",
                    container,
                    "PracticeButton",
                    new Vector2(0f, -38.5f),
                    new Vector2(15f, 7f),
                    () => { scrollView.PageDownButtonPressed(); },
                    "˅");
                Destroy(_pageDownButton.GetComponentsInChildren<ImageView>().FirstOrDefault(x => x.name == "Underline"));
                #endregion

                CenterKeys = new KEYBOARD(container, "", false, -15, 15);

                // BUG: Need additional modes disabling one shot buttons
                // BUG: Need to make sure the buttons are usable on older headsets

                _CurrentSongName = BeatSaberUI.CreateText(container, "", new Vector2(-35, 37f));
                _CurrentSongName.fontSize = 3f;
                _CurrentSongName.color = Color.cyan;
                _CurrentSongName.alignment = TextAlignmentOptions.Left;
                _CurrentSongName.enableWordWrapping = false;
                _CurrentSongName.text = "";

                _CurrentSongName2 = BeatSaberUI.CreateText(container, "", new Vector2(-35, 34f));
                _CurrentSongName2.fontSize = 3f;
                _CurrentSongName2.color = Color.cyan;
                _CurrentSongName2.alignment = TextAlignmentOptions.Left;
                _CurrentSongName2.enableWordWrapping = false;
                _CurrentSongName2.text = "";

                //CenterKeys.AddKeys(SONGLISTKEY);
                if (!RequestBot.AddKeyboard(CenterKeys, "mainpanel.kbd"))
                {
                    CenterKeys.AddKeys(SONGLISTKEY);
                }

                ColorDeckButtons(CenterKeys, Color.white, Color.magenta);

                RequestBot.AddKeyboard(CenterKeys, "CenterPanel.kbd");

                CenterKeys.DefaultActions();

                #region History button
                // History button
                _historyButton = UIHelper.CreateUIButton("SRMHistory", container, "PracticeButton", new Vector2(53f, 30f),
                    new Vector2(25f, 15f),
                    () =>
                    {
                        _isShowingHistory = !_isShowingHistory;
                        RequestBot.SetTitle(_isShowingHistory ? "Song Request History" : "Song Request Queue");
                        if (NumberOfCells() > 0)
                        {
                            _songListTableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, false);
                            _songListTableView.SelectCellWithIdx(0);
                            _selectedRow = 0;
                        }
                        else
                        {
                            _selectedRow = -1;
                        }
                        UpdateRequestUI(true);
                        SetUIInteractivity();
                        _lastSelection = -1;
                    }, "History");

                _historyButton.ToggleWordWrapping(false);
                _historyHintText = UIHelper.AddHintText(_historyButton.transform as RectTransform, "");
                #endregion

                #region Ping button
                // Blacklist button
                _pingButton = UIHelper.CreateUIButton("SRMPing", container, "PracticeButton", new Vector2(53f, 20f),
                    new Vector2(25f, 15f),
                    () =>
                    {
                        if (NumberOfCells() > 0)
                        {
                            var request = GetRequest(_selectedRow, _isShowingHistory);

                            if (request != null)
                            {
                                ChatHandler.Send($"Hey @{request.RequestedBy} - you still here? 🤔 Let us know!");
                            }
                        }
                    }, "Ping");

                _pingButton.ToggleWordWrapping(false);
                UIHelper.AddHintText(_pingButton.transform as RectTransform, "Hey, hey you! Still here?");
                #endregion

                #region Blacklist button
                // Blacklist button
                _blacklistButton = UIHelper.CreateUIButton("SRMBlacklist", container, "PracticeButton", new Vector2(53f, 10f),
                    new Vector2(25f, 15f),
                    () =>
                    {
                        if (NumberOfCells() > 0)
                        {
                            void _onConfirm()
                            {
                                var request = GetRequest(_selectedRow, _isShowingHistory);
                                SongModerationSettings.Current.Update(config => config.Bans.Add(request.Song.ID));
                                
                                if (!_isShowingHistory)
                                {
                                    RequestQueue.Current.Remove(request.Song.ID, RequestStatus.Blacklisted);
                                }

                                ChatHandler.Send($"{request.Song.ID} blocked.");

                                if (_selectedRow > 0)
                                    _selectedRow--;
                                confirmDialogActive = false;
                            }

                            // get song
                            var song = GetRequest(_selectedRow, _isShowingHistory)?.Song;

                            if (song != null)
                            {
                                // indicate dialog is active
                                confirmDialogActive = true;

                                // show dialog
                                YesNoModal.instance.ShowDialog("Block Song Warning", $"Blocking {song.Metadata.SongName} by {song.Metadata.LevelAuthorName}\r\nDo you want to continue?", _onConfirm, () => { confirmDialogActive = false; });
                            }
                        }
                    }, "Block");

                _blacklistButton.ToggleWordWrapping(false);
                UIHelper.AddHintText(_blacklistButton.transform as RectTransform, "Block the selected request from being queued in the future.");
                #endregion

                #region Skip button
                // Skip button
                _skipButton = UIHelper.CreateUIButton("SRMSkip", container, "PracticeButton", new Vector2(53f, 0f),
                    new Vector2(25f, 15f),
                    () =>
                    {
                        if (NumberOfCells() > 0)
                        {
                            // get song
                            var song = GetRequest(_selectedRow, _isShowingHistory)?.Song;

                            Action onConfirm = () =>
                            {
                                // skip it
                                RequestQueue.Current.Remove(song.ID, RequestStatus.Skipped);

                                // select previous song if not first song
                                if (_selectedRow > 0)
                                {
                                    _selectedRow--;
                                }

                                // get new selected song
                                currentsong = GetRequest(_selectedRow, _isShowingHistory)?.Song;

                                // indicate dialog is no longer active
                                confirmDialogActive = false;
                            };

                            // indicate dialog is active
                            confirmDialogActive = true;

                            // show dialog
                            YesNoModal.instance.ShowDialog("Skip Song Warning", $"Skipping {song.Metadata.SongName} by {song.Metadata.LevelAuthorName}\r\nDo you want to continue?", onConfirm, () => { confirmDialogActive = false; });
                        }
                    }, "Skip");

                _skipButton.ToggleWordWrapping(false);
                UIHelper.AddHintText(_skipButton.transform as RectTransform, "Remove the selected request from the queue.");
                #endregion

                #region Play button
                // Play button
                _playButton = UIHelper.CreateUIButton("SRMPlay", container, "ActionButton", new Vector2(53f, -10f),
                    new Vector2(25f, 15f),
                    () =>
                    {
                        if (NumberOfCells() > 0)
                        {
                            var request = GetRequest(_selectedRow, _isShowingHistory);
                            if (!_isShowingHistory)
                            {
                                RequestQueue.Current.Remove(request.Song.ID, RequestStatus.Played);
                            }

                            SetUIInteractivity(false);
                            RequestBot.Play(request);
                            _selectedRow = -1;
                        }
                    }, "Play");

                ((RectTransform)_playButton.transform).localScale = Vector3.one;
                _playButton.GetComponent<NoTransitionsButton>().enabled = true;

                _playButton.ToggleWordWrapping(false);
                _playButton.interactable = ((_isShowingHistory && RequestQueue.Current.Data.Requests.Count > 0) || (!_isShowingHistory && RequestQueue.Current.Data.History.Count > 0));
                UIHelper.AddHintText(_playButton.transform as RectTransform, "Download and scroll to the currently selected request.");
                #endregion

                #region Queue button
                // Queue button
                _queueButton = UIHelper.CreateUIButton("SRMQueue", container, "PracticeButton", new Vector2(53f, -30f),
                    new Vector2(25f, 15f),
                    () =>
                    {
                        bool newState = !RequestBotSettings.Current.Data.RequestQueueOpen;
                        RequestBotSettings.Current.Update(config => config.RequestQueueOpen = newState);
                        ChatHandler.Send($"Queue is now {(newState ? "open!" : "closed.")}");
                        //RequestBot.WriteQueueStatusToFile(QueueConfigManager.Instance.Config.RequestQueueOpen ? "Queue is open." : "Queue is closed.");
                        //RequestBot.Instance.QueueChatMessage(QueueConfigManager.Instance.Config.RequestQueueOpen ? "Queue is open." : "Queue is closed.");
                        UpdateRequestUI();
                    }, RequestBotSettings.Current.Data.RequestQueueOpen ? "Queue Open" : "Queue Closed");

                _queueButton.ToggleWordWrapping(true);
                _queueButton.SetButtonUnderlineColor(RequestBotSettings.Current.Data.RequestQueueOpen ? Color.green : Color.red);
                _queueButton.SetButtonTextSize(3.5f);
                UIHelper.AddHintText(_queueButton.transform as RectTransform, "Open/Close the queue.");
                #endregion

                #region Websocket Connect Button
                // Websocket Connect button
                //_websocketConnectButton = UIHelper.CreateUIButton("WSConnect", container, "PracticeButton",
                //    new Vector2(53f, -20f),
                //    new Vector2(25f, 15f),
                //    () =>
                //    {
                //        ChatHandler.WebsocketHandlerConnect();
                //    }, "Connect WS");

                //_websocketConnectButton.ToggleWordWrapping(true);
                //_websocketConnectButton.SetButtonUnderlineColor(Color.red);
                //_websocketConnectButton.SetButtonTextSize(3.5f);
                //UIHelper.AddHintText(_websocketConnectButton.transform as RectTransform, "Connects the Websocket");

                #endregion

                // Set default RequestFlowCoordinator title
                RequestBot.SetTitle(_isShowingHistory ? "Song Request History" : "Song Request Queue");
            }



            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

            if (addedToHierarchy)
            {
                _selectedRow = -1;
                _songListTableView.ClearSelection();
            }

            UpdateRequestUI();
            SetUIInteractivity(true);
        }

        protected override void DidDeactivate(bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidDeactivate(addedToHierarchy, screenSystemEnabling);
            if (!confirmDialogActive)
            {
                _isShowingHistory = false;
            }
        }

        public SongRequest CurrentlySelectedSong()
        {
            var selected = RequestQueue.Current.Data.History[0];

            if (_selectedRow != -1 && NumberOfCells() > _selectedRow)
            {
                selected = GetRequest(_selectedRow, _isShowingHistory);
            }
            return selected;
        }

        public void UpdateSelectSongInfo()
        {
            if (RequestQueue.Current.Data.History.Count > 0)
            {
                var selected = CurrentlySelectedSong();

                _CurrentSongName.text = selected.Song.Metadata.SongName;
                _CurrentSongName2.text = $"{selected.Song.Metadata.SongAuthorName} ({selected.Song.ID})";

                ColorDeckButtons(CenterKeys, Color.white, Color.magenta);
            }
        }

        public void UpdateRequestUI(bool selectRowCallback = false)
        {
            _playButton.interactable = ((_isShowingHistory && RequestQueue.Current.Data.History.Count > 0) || (!_isShowingHistory && RequestQueue.Current.Data.Requests.Count > 0));

            _queueButton.SetButtonText(RequestBotSettings.Current.Data.RequestQueueOpen ? "Queue Open" : "Queue Closed");
            _queueButton.SetButtonUnderlineColor(RequestBotSettings.Current.Data.RequestQueueOpen ? Color.green : Color.red);

            _historyHintText.text = _isShowingHistory ? "Go back to your current song request queue." : "View the history of song requests from the current session.";
            _historyButton.SetButtonText(_isShowingHistory ? "Requests" : "History");
            _playButton.SetButtonText(_isShowingHistory ? "Replay" : "Play");

            //_websocketConnectButton.gameObject.SetActive(!ChatHandler.WebsocketHandlerConnected() && RequestQueueConfigManager.Instance.Config.WebsocketEnabled);

            UpdateSelectSongInfo();

            _songListTableView.ReloadData();

            if (_selectedRow == -1)
            {
                return;
            }

            if (NumberOfCells() > _selectedRow)
            {
                _songListTableView.SelectCellWithIdx(_selectedRow, selectRowCallback);
                _songListTableView.ScrollToCellWithIdx(_selectedRow, TableView.ScrollPositionType.Beginning, true);
            }
        }

        public void UpdateWebsocketConnectButton(bool active)
        {
            _websocketConnectButton.gameObject.SetActive(active);
        }

        private void DidSelectRow(TableView table, int row)
        {
            _selectedRow = row;
            if (row != _lastSelection)
            {
                _lastSelection = row;
            }

            UpdateSelectSongInfo();
            SetUIInteractivity();
        }

        private void SongLoader_SongsLoadedEvent(SongCore.Loader arg1, ConcurrentDictionary<string, CustomPreviewBeatmapLevel> arg2)
        {
            _songListTableView?.ReloadData();
        }

        private List<SongRequest> Songs => _isShowingHistory ? RequestQueue.Current.Data.History : RequestQueue.Current.Data.Requests;

        /// <summary>
        /// Alter the state of the buttons based on selection
        /// </summary>
        /// <param name="interactive">Set to false to force disable all buttons, true to auto enable buttons based on states</param>
        public void SetUIInteractivity(bool interactive = true)
        {
            var toggled = interactive;

            if (_selectedRow >= Songs.Count())
            {
                _selectedRow = -1;
            }

            if (NumberOfCells() == 0 || _selectedRow == -1 || _selectedRow >= Songs.Count())
            {
                Plugin.Log("Nothing selected, or empty list, buttons should be off");
                toggled = false;
            }

            _playButton.interactable = toggled;
            _skipButton.interactable = toggled && !_isShowingHistory;
            _blacklistButton.interactable = toggled;
            _pingButton.interactable = toggled;

            // history button can be enabled even if others are disabled
            _historyButton.interactable = true;
        }

        private CustomPreviewBeatmapLevel CustomLevelForRow(int row)
        {
            // get level id from hash
            var request = GetRequest(row, _isShowingHistory);
            var hash = request.Song.Versions[0].Hash;

            var levelIds = SongCore.Collections.levelIDsForHash(hash);
            if (levelIds.Count == 0)
            {
                return null;
            }

            // lookup song from level id
            return SongCore.Loader.CustomLevels.FirstOrDefault(s => string.Equals(s.Value.levelID, levelIds.First(), StringComparison.OrdinalIgnoreCase)).Value ?? null;
        }

        private void PlayPreview(CustomPreviewBeatmapLevel level)
        {
            //_songPreviewPlayer.CrossfadeTo(level.previewAudioClip, level.previewStartTime, level.previewDuration);
        }

        private static Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();

        #region TableView.IDataSource interface
        public float CellSize() { return 10f; }

        public int NumberOfCells()
        {
            return Songs.Count;
        }

        public TableCell CellForIdx(TableView tableView, int row)
        {
            LevelListTableCell _tableCell = Instantiate(_requestListTableCellInstance);
            _tableCell.reuseIdentifier = "RequestBotSongCell";
            _tableCell.SetField("_notOwned", false);

            SongRequest request = GetRequest(row, _isShowingHistory);
            SetDataFromLevelAsync(request, _tableCell, row);

            return _tableCell;
        }
        #endregion

        private async void SetDataFromLevelAsync(SongRequest request, LevelListTableCell _tableCell, int row)
        {
            var favouritesBadge = _tableCell.GetField<Image, LevelListTableCell>("_favoritesBadgeImage");
            favouritesBadge.enabled = false;

            List<string> tags = new List<string>();

            bool isPrio = request.PriorityValue >= RequestBotSettings.Current.Data.MinimumPriorityRequestValue;
            if (isPrio)
            {
                tags.Add("PRIO");
            }

            bool hasComment = !string.IsNullOrEmpty(request.Comment);
            if (hasComment)
            {
                tags.Add("MSG");
            }

            tags.Add(request.Song.ID);

            var songDurationText = _tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songDurationText");
            songDurationText.text = StringUtils.GetDurationString((int)request.Song.Metadata.Duration);

            var songBpm = _tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songBpmText");
            //if (!request.requestor.IsModerator && !request.requestor.IsVip)
            (songBpm.transform as RectTransform).anchoredPosition = new Vector2(-2.5f, -1.8f);
            (songBpm.transform as RectTransform).sizeDelta += new Vector2(15f, 0f);
            songBpm.text = string.Join(" - ", tags);

            var songBpmIcon = _tableCell.GetComponentsInChildren<Image>().LastOrDefault(c => string.Equals(c.name, "BpmIcon", StringComparison.OrdinalIgnoreCase));
            if (songBpmIcon != null)
            {
                //songBpmIcon.color = request.requestor.IsModerator ? Color.green : request.requestor.IsVip ? Color.magenta : Color.white;
                //if (!request.requestor.IsModerator && !request.requestor.IsVip)
                    Destroy(songBpmIcon);
            }

            var songName = _tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songNameText");
            songName.richText = true;
            songName.text = $"{request.Song.Metadata.SongName} <size=50%>{(int)(request.Song.Stats.Score * 100)}% <color=#3fff3f>{(request.Song.Ranked ? "[RANKED]" : string.Empty)}</color></size>";

            var author = _tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songAuthorText");
            author.richText = true;
            author.text = $"{request.Song.Metadata.SongAuthorName} [{request.Song.Metadata.LevelAuthorName}]";

            var image = _tableCell.GetField<Image, LevelListTableCell>("_coverImage");
            var imageSet = false;

            if (SongCore.Loader.AreSongsLoaded)
            {
                var level = CustomLevelForRow(row);
                if (level != null)
                {
                    // set image from song's cover image
                    var sprite = await level.GetCoverImageAsync(System.Threading.CancellationToken.None);
                    image.sprite = sprite;
                    imageSet = true;
                }
            }

            if (!imageSet)
            {
                var url = request.Song.Versions[0].CoverURL;

                if (!_cachedTextures.TryGetValue(url, out var tex))
                {
                    var b = await Plugin.WebClient.DownloadImage(url, System.Threading.CancellationToken.None);

                    tex = new Texture2D(2, 2);
                    tex.LoadImage(b);

                    try
                    {
                        _cachedTextures.Add(url, tex);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                image.sprite = Base64Sprites.Texture2DToSprite(tex);
            }

            List<string> hoverSegments = new List<string>();
            hoverSegments.Add($"Requested by: {request.RequestedBy}");
            hoverSegments.Add($"At: {request.RequestTimestamp.ToString("hh:mm tt")}");
            hoverSegments.Add($"Status: {request.Status}");
            if (request.Status == RequestStatus.Played)
            {
                hoverSegments.Add($"At: {request.PlayedTimestamp.ToString("hh:mm:ss")}");
            }

            if (isPrio)
            {
                hoverSegments.Add($"Prio: ${request.PriorityValue:0.00}");
            }

            if (hasComment)
            {
                hoverSegments.Add($"Comment: {request.Comment}");
            }

            UIHelper.AddHintText(_tableCell.transform as RectTransform, string.Join("\n", hoverSegments));
        }

        private SongRequest GetRequest(int index, bool fromHistory)
        {
            var source = fromHistory ? RequestQueue.Current.Data.History : RequestQueue.Current.Data.Requests;

            if (index >= 0 && index < source.Count)
            {
                return source[index];
            }

            return null;
        }
    }
}
