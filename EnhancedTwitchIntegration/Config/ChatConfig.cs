using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IPA.Utilities;

namespace SongRequestManager.Config
{
    public class ChatConfig
    {
        private static readonly string FileName = "SRMChatConfig.ini";
        private static readonly string FilePath = Path.Combine(Plugin.DataPath, FileName);

        public string ChatUsername = "";
        public string ChatToken = "";
        public string ChatChannel = "";

        public event Action<ChatConfig> OnChanged;

        private readonly FileSystemWatcher _configWatcher;
        private bool _saving;

        private static ChatConfig _instance = null;
        public static ChatConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChatConfig();
                }

                return _instance;
            }

            private set => _instance = value;
        }

        public ChatConfig()
        {
            Instance = this;

            _configWatcher = new FileSystemWatcher();

            Task.Run(() =>
            {
                while (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                {
                    Thread.Sleep(100);
                }

                Plugin.Log("FilePath exists! Continuing initialization!");

                if (File.Exists(FilePath))
                {
                    Load();
                }
                Save();

                _configWatcher.Path = Path.GetDirectoryName(FilePath);
                _configWatcher.NotifyFilter = NotifyFilters.LastWrite;
                _configWatcher.Filter = FileName;
                _configWatcher.EnableRaisingEvents = true;

                _configWatcher.Changed += ConfigWatcherOnChanged;
            });
        }

        ~ChatConfig()
        {
            _configWatcher.Changed -= ConfigWatcherOnChanged;
        }

        public void Load()
        {
            ConfigSerializer.LoadConfig(this, FilePath);
        }

        public void Save(bool callback = false)
        {
            if (!callback)
            {
                _saving = true;
            }

            ConfigSerializer.SaveConfig(this, FilePath);
        }

        private void ConfigWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (_saving)
            {
                _saving = false;
                return;
            }

            Load();

            OnChanged?.Invoke(this);
        }
    }
}
