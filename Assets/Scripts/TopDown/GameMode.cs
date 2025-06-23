using System;

namespace TopDown
{
    public interface IGameMode
    {
        public void Load(ModeSet set);
        public event Action OnQuit;
    }
}