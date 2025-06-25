using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using static UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus;

namespace TopDown
{
    public abstract class Loader
    {
        public event Action<float> OnProgress;
        public event Action OnFailure;
        public event Action OnSuccess;
        public string Locator { get; protected set; }

        public abstract void Load();
        public abstract void Unload();
        protected void InvokeOnProgress(float progress) => OnProgress?.Invoke(progress);
        protected void InvokeOnFailure() => OnFailure?.Invoke();
        protected void InvokeOnSuccess() => OnSuccess?.Invoke();
    }

    public class Loader<FileType, LoadType> : Loader
    where FileType : UnityEngine.Object
    where LoadType : class
    {
        static readonly Dictionary<string, Loader<FileType, LoadType>> loaders = new();
        public Dictionary<string, LoadType> LoadedResources { get; private set; } = new();
        AsyncOperationHandle<IList<FileType>> handle;

        private Loader(string locator) { this.Locator = locator; }
        public static Loader<FileType, LoadType> GetLoader(string locator)
        {
            if (!loaders.TryGetValue(locator, out var loader))
            {
                loader = new Loader<FileType, LoadType>(locator);
                loaders.Add(locator, loader);
            }
            return loader;
        }
        public override void Unload()
        {
            if (!handle.IsValid()) return;

            if (!handle.IsDone)
            {
                Debug.LogWarning("로드가 진행중이라 언로드를 실행하지 못했습니다.");
                return;
            }

            Addressables.Release(handle);
            LoadedResources.Clear();
        }
        public async override void Load()
        {
            LoadedResources.Clear();
            if (!handle.IsDone) { InvokeOnFailure(); return; }
            if (handle.IsValid()) Addressables.Release(handle);

            var locationsHandle = Addressables.LoadResourceLocationsAsync(Locator);
            await locationsHandle.ToUniTask();

            bool exists = locationsHandle.Status == Succeeded
                          && locationsHandle.Result != null
                          && locationsHandle.Result.Count > 0;

            Addressables.Release(locationsHandle);
            if (!exists) { InvokeOnFailure(); return; }

            handle = Addressables.LoadAssetsAsync<FileType>(Locator, null);
            while (!handle.IsDone)
            {
                InvokeOnProgress(handle.PercentComplete);
                await UniTask.Yield();
            }
            InvokeOnProgress(1f);

            if (handle.Status is Succeeded)
            {
                if (typeof(FileType).IsAssignableFrom(typeof(GameObject)))
                    LoadedResources = handle.Result.ToDictionary(x => x.name, x => x.GetComponent<LoadType>());
                else
                    LoadedResources = handle.Result.ToDictionary(x => x.name, x => x as LoadType);
                InvokeOnSuccess();
            }
            else
            {
                InvokeOnFailure();
            }
        }

    }
    public class SceneLoader : Loader
    {
        public SceneLoader(string locator)
        {
            Locator = locator;
        }

        public async override void Load()
        {
            var asyncOp = SceneManager.LoadSceneAsync(Locator);
            asyncOp.allowSceneActivation = false;
            while (asyncOp.progress < 0.9f)
            {
                InvokeOnProgress(asyncOp.progress);
                await UniTask.Yield();
            }

            InvokeOnProgress(1f);
            asyncOp.allowSceneActivation = true;
            await UniTask.WaitUntil(() => asyncOp.isDone);
            InvokeOnSuccess();
        }

        public override void Unload()
        {
        }
    }
}