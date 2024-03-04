using System;
using System.IO;
using Newtonsoft.Json;

namespace SongRequestManager.Config
{
    public interface IStateProvider<T>
    {
        T Data { get; }
        event Action<T> OnChanged;
        void Update(Action<T> updateFunc);
    }

    public class PersistedStateProvider<T> : IStateProvider<T> where T : new()
    {
        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public T Data { get; private set; }
        public event Action<T> OnChanged;

        private readonly string fileName;
        private readonly string legacyFileName;

        private readonly FileSystemWatcher configWatcher = new FileSystemWatcher();

        private readonly object fileSyncLock = new object();

        private string FilePath => Path.Combine(Plugin.DataPath, this.fileName);
        private string LegacyFilePath => string.IsNullOrEmpty(this.legacyFileName) ? string.Empty : Path.Combine(Plugin.DataPath, this.legacyFileName);

        public PersistedStateProvider(string fileName, string legacyFileName)
        {
            this.fileName = fileName;
            this.legacyFileName = legacyFileName;

            if (File.Exists(FilePath))
            {
                this.LoadData();
            }
            else if (File.Exists(LegacyFilePath))
            {
                this.Data = new T();
                ConfigSerializer.LoadConfig(this.Data, LegacyFilePath);
                this.Save();
            }
            else
            {
                this.Data = new T();
                this.Save();
            }

            this.configWatcher.Path = Plugin.DataPath;
            this.configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            this.configWatcher.Filter = this.fileName;
            this.configWatcher.EnableRaisingEvents = true;

            this.configWatcher.Changed += this.OnFileChanged;
        }

        ~PersistedStateProvider()
        {
            this.configWatcher.Changed -= this.OnFileChanged;
        }

        public virtual void Update(Action<T> updateFunc)
        {
            updateFunc(this.Data);
            this.Save();
        }

        private void Save()
        {
            lock (this.fileSyncLock)
            {
                File.WriteAllText(this.FilePath, JsonConvert.SerializeObject(this.Data, SerializerSettings));
            }
        }

        private void LoadData()
        {
            lock (this.fileSyncLock)
            {
                this.Data = JsonConvert.DeserializeObject<T>(File.ReadAllText(this.FilePath));
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            this.LoadData();
            OnChanged?.Invoke(this.Data);
        }
    }
}
