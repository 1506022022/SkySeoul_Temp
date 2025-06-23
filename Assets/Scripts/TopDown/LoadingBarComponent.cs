using Cysharp.Threading.Tasks;
using Microlight.MicroBar;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TopDown
{
    public class LoadingBarComponent : MonoBehaviour, ILoad
    {
        public event Action OnLoaded;
        [SerializeField] MicroBar bar;
        [SerializeField] TextMeshProUGUI loadingInfo;
        [SerializeField, Range(3, 30)] float minDuration;
        readonly List<VisualLoader> resources = new();
        float lastLoadTime;
        public void Load()
        {
            if (resources.Count == 0)
            {
                OnLoaded?.Invoke();
                return;
            }
            resources.First().Load();
            lastLoadTime = Time.time;
        }
        public void Unload()
        {
            for (int i = 0; i < resources.Count; i++)
            {
                resources[i].Unload();
                resources[i].ClearCallback();
            }
            this.resources.Clear();
        }
        public void Initialize(List<Loader> resources)
        {
            Unload();
            for (int i = 0; i < resources.Count; i++)
            {
                var resource = resources[i];
                var loader = new VisualLoader();
                this.resources.Add(loader);
                loader.SetLoader(resource);
                loader.SetLoadingBar(bar);
                loader.SetInfoText(loadingInfo);
            }

            for (int i = 0; i < this.resources.Count; i++)
            {
                var loader = this.resources[i];
                if (i == resources.Count - 1) loader.CallbackSuccessLoad += OnSuccess;
                else loader.CallbackSuccessLoad += this.resources[i + 1].Load;
            }
        }
        async void OnSuccess()
        {
            for (int i = 0; i < resources.Count; i++) resources[i].ClearCallback();
            var waitting = (int)(Mathf.Max(lastLoadTime + minDuration - Time.time, 0) * 1000);
            await UniTask.Delay(waitting);
            OnLoaded?.Invoke();
            gameObject.SetActive(false);
        }
    }
}