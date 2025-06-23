using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace TopDown
{
    public class MapSelecter : MonoBehaviour, ISelect
    {
        public MapView MapViewPrefab;
        public uint SelectedValue { get; private set; }
        public event Action<ISelect> OnSelect;
        public UnityEvent OnFocusEvent;
        public UnityEvent OffFocusEvent;
        public UnityEvent OnSelectEvent;
        public Vector2 margin;
        private Vector3 scrollVelocityX;
        private Vector3 scrollVelocityY;
        private Vector3 focusVelocity;

        public float smoothTime = 0.2f;

        private readonly List<MapView> clearedMapInstances = new();
        private readonly List<MapView> routeMapInstances = new();

        private Transform clearedMaps;
        private Transform routeMaps;

        public Transform FocusTransform;

        private int clearedMapIndex = 0;
        private int routeMapIndex = 0;

        public bool IsFocused;
        private void Awake()
        {
            Initialize();
            PositionMaps();
        }
        public void Select()
        {
            SelectedValue = GetFocusedMapView().MapData.ID;
            OnSelect?.Invoke(this);
            OnSelectEvent?.Invoke();
        }
        private void OnFocus()
        {
            IsFocused = true;
            GetFocusedMapView().SetOrderInLayer(1000);
            OnFocusEvent.Invoke();
        }
        private void OffFocus()
        {
            IsFocused = false;
            PositionMaps();
            GetFocusedMapView().SetOrderInLayer(0);
            OffFocusEvent.Invoke();
        }
        private void Initialize()
        {
            if (clearedMaps != null)
                Destroy(clearedMaps.gameObject);

            clearedMaps = new GameObject(nameof(clearedMaps)).transform;
            clearedMaps.SetParent(transform);
            clearedMaps.localPosition = Vector3.zero;
            clearedMaps.localRotation = Quaternion.identity;
            clearedMaps.localScale = Vector3.one;

            if (routeMaps != null)
                Destroy(routeMaps.gameObject);

            routeMaps = new GameObject(nameof(routeMaps)).transform;
            routeMaps.SetParent(clearedMaps);
            routeMaps.localPosition = Vector3.zero;
            routeMaps.localRotation = Quaternion.identity;
            routeMaps.localScale = Vector3.one;

            clearedMapInstances.Clear();
            foreach (var clearedMapID in MapManager.ClearedMaps)
            {
                var clearedMap = MapManager.Maps[clearedMapID];
                var mapView = Instantiate(MapViewPrefab, clearedMaps);
                mapView.SetMapData(clearedMap);
                clearedMapInstances.Add(mapView);
            }

            routeMapInstances.Clear();
            foreach (var map in MapManager.Maps.Values)
            {
                if (clearedMapInstances.Any(x => x.MapData.Equals(map))) continue;

                var mapView = Instantiate(MapViewPrefab, routeMaps);
                mapView.SetMapData(map);
                routeMapInstances.Add(mapView);
            }
        }
        private void PositionMaps()
        {
            var reversedClearedMaps = clearedMapInstances.ToList();
            reversedClearedMaps.Reverse();

            for (int i = 0; i < reversedClearedMaps.Count; i++)
            {
                var mapView = reversedClearedMaps[i];
                mapView.transform.localPosition = Vector3.left * (mapView.MapPrecutWidth + margin.x) * (i + 1);
                mapView.transform.localRotation = Quaternion.identity;
            }

            for (int i = 0; i < routeMapInstances.Count; i++)
            {
                var mapView = routeMapInstances[i];
                mapView.transform.localPosition = Vector3.down * (mapView.MapPrecutHeight + margin.y) * i;
                mapView.transform.localRotation = Quaternion.identity;
            }
        }
        private void Update()
        {
            SmoothHorizontalScroll();
            SmoothVerticalScroll();
            HandleFocus();

            if (!IsFocused)
            {
                HandleInput();
            }

            if (IsFocused && Input.GetKeyDown(KeyCode.A))
            {
                Select();
            }

            if (!IsFocused && Input.GetKeyDown(KeyCode.A))
            {
                OnFocus();
            }
        }


        public float y = 180f;
        public float yDuration = 0.5f;
        float yt = 0f;
        private void HandleFocus()
        {
            if (!IsFocused) { yt = 0f; return; }
            if (Input.GetKeyDown(KeyCode.Escape)) { OffFocus(); return; }

            var focusedMap = GetFocusedMapView();
            focusedMap.transform.position = Vector3.SmoothDamp(
                focusedMap.transform.position,
                FocusTransform.position,
                ref focusVelocity,
                yDuration
            );
            focusedMap.transform.rotation = Quaternion.Lerp(
                Quaternion.identity,
                Quaternion.AngleAxis(y, Vector3.up),
                yt
            );
            yt += (Time.deltaTime / yDuration);
        }
        private MapView GetFocusedMapView()
        {
            if (clearedMapIndex == 0)
                return routeMaps.GetChild(routeMapIndex).GetComponent<MapView>();
            else
                return clearedMaps.GetChild(clearedMaps.childCount - clearedMapIndex).GetComponent<MapView>();
        }
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                clearedMapIndex = Mathf.Clamp(clearedMapIndex + 1, 0, clearedMapInstances.Count);
                if (clearedMapIndex == 0) routeMapIndex = 0;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                clearedMapIndex = Mathf.Clamp(clearedMapIndex - 1, 0, clearedMapInstances.Count - 1);
                if (clearedMapIndex == 0) routeMapIndex = 0;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (clearedMapIndex != 0)
                {
                    clearedMapIndex = 0;
                    routeMapIndex = 0;
                    return;
                }
                routeMapIndex = Mathf.Clamp(routeMapIndex - 1, 0, routeMapInstances.Count - 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (clearedMapIndex != 0)
                {
                    clearedMapIndex = 0;
                    routeMapIndex = 0;
                    return;
                }
                routeMapIndex = Mathf.Clamp(routeMapIndex + 1, 0, routeMapInstances.Count - 1);
            }
        }
        private void SmoothHorizontalScroll()
        {
            if (clearedMapInstances.Count == 0) return;

            float itemWidth = clearedMapInstances[0].MapPrecutWidth + margin.x;
            float targetX = itemWidth * clearedMapIndex;
            Vector3 targetPos = new Vector3(targetX, clearedMaps.localPosition.y, clearedMaps.localPosition.z);
            clearedMaps.localPosition = Vector3.SmoothDamp(clearedMaps.localPosition, targetPos, ref scrollVelocityX, smoothTime);
        }
        private void SmoothVerticalScroll()
        {
            if (routeMapInstances.Count == 0) return;

            float itemHeight = routeMapInstances[0].MapPrecutHeight + margin.y;
            float targetY = itemHeight * routeMapIndex;
            Vector3 targetPos = new Vector3(routeMaps.localPosition.x, targetY, routeMaps.localPosition.z);
            routeMaps.localPosition = Vector3.SmoothDamp(routeMaps.localPosition, targetPos, ref scrollVelocityY, smoothTime);
        }

        [MenuItem("Func/Test")]
        public static void Test()
        {
            CreateMapData();
        }

        private static void CreateMapData()
        {
            var json = new MapJson();
            for (uint i = 0; i < 10; i++)
            {
                json.Maps.Add(new MapData
                {
                    ID = i,
                    Name = i.ToString("D5"),
                    LevelOfStory = i,
                    PrecutPath = i.ToString("D5")
                });

                if (UnityEngine.Random.Range(0, 3) % 2 == 0)
                {
                    json.ClearedMaps.Add(i);
                }
            }

            var jsonString = JsonUtility.ToJson(json);
            File.WriteAllText(MapManager.mapDataPath, jsonString);
        }
    }
}
