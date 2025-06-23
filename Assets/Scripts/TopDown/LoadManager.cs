using System;
using System.Collections.Generic;

namespace TopDown
{
    public interface ILoad
    {
        public event Action OnLoaded;
        public void Initialize(List<Loader> resources);
        public void Load();
        public void Unload();
    }
}
