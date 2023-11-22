using System;
using SongRequestManager.Config;

namespace SongRequestManagerUT.Mocks
{
    public class MockStateProvider<T> : IStateProvider<T> where T : new()
    {
        public T Data { get; set; } = new T();

        public event Action<T> OnChanged;

        public void Update(Action<T> updateFunc)
        {
            updateFunc(this.Data);
        }

        public void NotifyChange()
        {
            this.OnChanged?.Invoke(this.Data);
        }
    }
}
