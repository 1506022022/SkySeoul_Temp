using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public interface IGameMode
    {
        public void Initialize(ModeSet set);
        public event Action OnQuit;
    }
    
}
