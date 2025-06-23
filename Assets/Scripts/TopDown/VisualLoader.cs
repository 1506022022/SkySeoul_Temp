using Microlight.MicroBar;
using System;
using TMPro;
using UnityEngine;

namespace TopDown
{
    public class VisualLoader
    {
        public event Action CallbackSuccessLoad;
        public event Action CallbackFailureLoad;
        MicroBar bar;
        Loader loader;
        TextMeshProUGUI loadingInfo;

        public void SetLoader(Loader loader)
        {
            if (this.loader == loader) return;
            if (this.loader != null) ClearEvents();
            this.loader = loader; AddEvents();
        }
        public void SetLoadingBar(MicroBar bar)
        {
            if (this.bar == bar) return;
            this.bar = bar;
            this.bar.Initialize(0f);
        }
        public void SetInfoText(TextMeshProUGUI loadingInfo)
        {
            this.loadingInfo = loadingInfo;
        }
        public void ClearCallback()
        {
            CallbackSuccessLoad = null;
            CallbackFailureLoad = null;
        }
        public void Load() => loader?.Load();
        public void Unload() => loader?.Unload();
        void UpdateLoadingText()
        {
            if (loadingInfo == null) return;
            loadingInfo.text = $"{loader.Locator} ({(int)(bar.HPPercent * 100):D3}/100)";
            Debug.Log(loadingInfo.text);
        }
        void OnProgress(float progress)
        {
            bar.UpdateBar(progress);
            this.UpdateLoadingText();
        }
        void AddEvents()
        {
            loader.OnProgress += OnProgress;
            loader.OnFailure += OnFailure;
            loader.OnFailure += ClearEvents;
            loader.OnSuccess += OnSuccess;
            loader.OnSuccess += ClearEvents;
        }
        void ClearEvents()
        {
            loader.OnProgress -= OnProgress;
            loader.OnFailure -= OnFailure;
            loader.OnFailure -= ClearEvents;
            loader.OnSuccess -= OnSuccess;
            loader.OnSuccess -= ClearEvents;
        }
        void OnSuccess()
        {
            CallbackSuccessLoad?.Invoke();
        }
        void OnFailure()
        {
            loadingInfo.text = $"{loader.Locator} failed.";
            CallbackFailureLoad?.Invoke();
        }
    }

}