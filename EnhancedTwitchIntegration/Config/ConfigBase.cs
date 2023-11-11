using System;
using System.IO;
using Newtonsoft.Json;

namespace SongRequestManager.Config
{

    public abstract class ConfigBase<T> where T : new()
    {
        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public T Config { get; private set; }
        public event Action<T> OnChanged;

        protected abstract string FilePath { get; }
        protected virtual string LegacyFilePath { get; } = string.Empty;

        private readonly FileSystemWatcher configWatcher = new FileSystemWatcher();

        protected ConfigBase()
        {
            if (File.Exists(FilePath))
            {
                this.LoadData();
            }
            else if (File.Exists(LegacyFilePath))
            {
                this.Config = new T();
                ConfigSerializer.LoadConfig(this.Config, LegacyFilePath);
                this.Save();
            }
            else
            {
                this.Config = new T();
                this.Save();
            }

            this.configWatcher.Path = Path.GetDirectoryName(FilePath);
            this.configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            this.configWatcher.Filter = Path.GetFileName(FilePath);
            this.configWatcher.EnableRaisingEvents = true;

            this.configWatcher.Changed += this.OnFileChanged;
        }

        ~ConfigBase()
        {
            this.configWatcher.Changed -= this.OnFileChanged;
        }

        public virtual void UpdateSettings(Action<T> updateFunc)
        {
            updateFunc(this.Config);
            this.Save();
        }

        private void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this.Config, SerializerSettings));
        }

        private void LoadData()
        {
            this.Config = JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            this.LoadData();
            OnChanged?.Invoke(this.Config);
        }
    }
}
