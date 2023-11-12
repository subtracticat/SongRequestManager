using System;
using System.IO;
using Newtonsoft.Json;

namespace SongRequestManager.Config
{

    public abstract class PersistedStateManager<T> where T : new()
    {
        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public T Data { get; private set; }
        public event Action<T> OnChanged;

        protected abstract string FileName { get; }
        protected virtual string LegacyFileName { get; } = string.Empty;

        private readonly FileSystemWatcher configWatcher = new FileSystemWatcher();

        private string FilePath => Path.Combine(Plugin.DataPath, this.FileName);
        private string LegacyFilePath => string.IsNullOrEmpty(this.LegacyFileName) ? string.Empty : Path.Combine(Plugin.DataPath, this.LegacyFileName);

        protected PersistedStateManager()
        {
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
            this.configWatcher.Filter = this.FileName;
            this.configWatcher.EnableRaisingEvents = true;

            this.configWatcher.Changed += this.OnFileChanged;
        }

        ~PersistedStateManager()
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
            File.WriteAllText(this.FilePath, JsonConvert.SerializeObject(this.Data, SerializerSettings));
        }

        private void LoadData()
        {
            this.Data = JsonConvert.DeserializeObject<T>(File.ReadAllText(this.FilePath));
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            this.LoadData();
            OnChanged?.Invoke(this.Data);
        }
    }
}
