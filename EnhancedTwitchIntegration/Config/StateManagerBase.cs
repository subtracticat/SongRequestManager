using System;

namespace SongRequestManager.Config
{
    public abstract class StateManagerBase<T> where T : new()
    {
        public event Action<T> OnChanged;

        protected abstract string FileName { get; }
        protected virtual string LegacyFileName { get; } = string.Empty;

        private IStateProvider<T> stateProvider;

        public void Initialize()
        {
            this.Initialize(new PersistedStateProvider<T>(this.FileName, this.LegacyFileName));
        }

        public void Initialize(IStateProvider<T> stateProvider)
        {
            this.stateProvider = stateProvider ?? throw new ArgumentNullException("stateProvider");
            this.stateProvider.OnChanged += OnStateProviderChanged;
        }

        public T Data
        {
            get
            {
                this.EnsureInitialized();
                return this.stateProvider.Data;
            }
        }

        public virtual void Update(Action<T> updateFunc)
        {
            this.EnsureInitialized();
            this.stateProvider.Update(updateFunc);
        }

        private void OnStateProviderChanged(T obj)
        {
            this.OnChanged?.Invoke(obj);
        }

        private void EnsureInitialized()
        {
            if (this.stateProvider == null)
            {
                throw new InvalidOperationException("State manager must be initialized prior to use.");
            }
        }
    }
}
