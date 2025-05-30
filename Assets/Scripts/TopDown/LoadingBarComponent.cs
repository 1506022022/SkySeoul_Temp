using Microlight.MicroBar;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TopDown
{
    public class LoadingBarComponent : MonoBehaviour, ILoadManager
    {
        public event Action OnLoaded;
        [SerializeField] private MicroBar bar;
        [SerializeField] private TextMeshProUGUI loadingInfo;
        public string LoadTarget;
        [Min(3)] public float minDuration = 3.0f;

        private void Awake()
        {
            bar.Initialize(1);
            if (loadingInfo != null)
            {
                bar.OnCurrentValueChange += UpdateLoadingText;
            }
        }
        private void UpdateLoadingText(MicroBar bar)
        {
            loadingInfo.text = $"{(string.IsNullOrEmpty(LoadTarget) ? "Now loading" : LoadTarget)} ({(int)(bar.HPPercent * 100):D3}/100)";
            Debug.Log(loadingInfo.text);
        }
        public void Load()
        {
            StopAllCoroutines();
            if (bar == null) return;
            StartCoroutine(LoadRoutine());
        }
        public void Unload()
        {
            GameObject.Destroy(gameObject);
        }
        private IEnumerator LoadRoutine()
        {
            gameObject.SetActive(true);
            float t = 0;
            bar.UpdateBar(0, true);

            while (t < 1)
            {
                t = Mathf.Clamp01(t + (Time.deltaTime / minDuration));
                bar.UpdateBar(t);
                yield return null;
            }
            yield return new WaitForSeconds(minDuration);
            OnLoaded?.Invoke();
            gameObject.SetActive(false);
        }
    }

}
