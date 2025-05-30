using System;

namespace TopDown
{
    public interface ILoadManager
    {
        public event Action OnLoaded;
        public void Load();
        public void Unload();
    }

}
