using Battle;
using Cysharp.Threading.Tasks;
using FieldEditorTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace TopDown
{
    internal class BattleMode : IGameMode
    {
        public event Action OnQuit;
        ILoad loader;
        ModeSet set;
        BattleController battle = new();
        readonly List<CharacterComponent> enemy = new();
        readonly Dictionary<FieldComponent, string> fieldInfos = new();
        CharacterComponent playable;
        List<Loader> loaders;
        FieldComponent current;

        public BattleMode()
        {
            battle.OnDead += OnDeadCharacter;
        }
        void ExitMode()
        {
            battle.Clear();
            battle = null;
            LoaderFactory.Unload();
            while (loaders.Count > 0)
            {
                loaders[0].Unload();
                loaders.RemoveAt(0);
            }
            set.MapType = MapType.Lobby;
            OnQuit?.Invoke();
        }
        void OnExitField(FieldComponent field)
        {
            while (field.enemys.Count > 0)
            {
                DisposeCharacter(field.enemys[0]);
                field.Remove(field.enemys[0]);
            }
        }
        async void OnEnterField(FieldComponent field)
        {
            var reader = new StringReader(fieldInfos[field]);
            var enemyDB = Loader<GameObject, CharacterComponent>.GetLoader(nameof(IEnemy));
            reader.ReadLine();
            string json;
            while ((json = reader.ReadLine()) != null)
            {
                var type = JsonUtility.FromJson<EntityData>(json).HeaderType;
                var instance = JsonUtility.FromJson(json, FieldEditorTool.Types.FindTypeByName<EntityData>(type));
                if (instance is ActorData actorData)
                {
                    var actor = GameObject.Instantiate(enemyDB.LoadedResources[actorData.Name]);
                    actor.transform.position = field.transform.position + actorData.Position;
                    actor.transform.eulerAngles = actorData.Rotation;
                    if (actor is IEnemy)
                    {
                        var agent = actor.GetComponent<NavMeshAgent>();
                        field.Add((MonsterComponent)actor);
                    }
                    OnBirthCharacter(actor);
                }
                await UniTask.Yield();
            }

            current = field;
            field.Initialize();
        }
        void OnBirthCharacter(CharacterComponent character)
        {
            character.Initialize();
            battle.JoinCharacter(character);
        }
        void OnBirthEnemy(CharacterComponent enemy)
        {
        }
        void OnBirthPlayableCharacter(CharacterComponent pc)
        {
        }
        void OnDeadCharacter(CharacterComponent character)
        {
            DisposeCharacter(character);
            if (character is IPlayable) OnDeadPlayableCharacter(character);
            else if (character is IEnemy) OnDeadEnemy(character);
        }
        void OnDeadPlayableCharacter(CharacterComponent character)
        {
            ExitMode();
        }
        void OnDeadEnemy(CharacterComponent character)
        {
            current.Remove(character as MonsterComponent);
        }
        void DisposeCharacter(CharacterComponent character)
        {
            battle.DisposeCharacter(character);
            GameObject.Destroy(character.gameObject, 3f);
        }
        void OnLoaded()
        {
            InitializeFields();
            InitializePlayableCharacter();
        }
        public void Load(ModeSet set)
        {
            this.set = set;
            var name = MapType.Lobby.ToString() + "Loader";
            loader = LoaderFactory.Load<ILoad>(name, name);
            loaders = new List<Loader>();
            loaders.Add(new SceneLoader(MapType.BattleMap.ToString()));
            loaders.Add(Loader<TextAsset, TextAsset>.GetLoader(nameof(FieldData)));
            loaders.Add(Loader<GameObject, FieldComponent>.GetLoader(nameof(FieldComponent)));
            loaders.Add(Loader<GameObject, CharacterComponent>.GetLoader(nameof(IEnemy)));
            loaders.Add(Loader<GameObject, CharacterComponent>.GetLoader(nameof(IPlayable)));
            loader.Initialize(loaders);
            loader.Load();
            loader.OnLoaded += OnLoaded;
        }
        void InitializeFields()
        {
            fieldInfos.Clear();

            var fieldPrefabDB = Loader<GameObject, FieldComponent>.GetLoader(nameof(FieldComponent));
            var fieldJsonDB = Loader<TextAsset, TextAsset>.GetLoader(nameof(FieldData)).LoadedResources.Values.ToList();
            for (int i = 0; i < fieldJsonDB.Count; i++)
            {
                var file = fieldJsonDB[i];
                var json = new StringReader(file.text).ReadLine();
                var fieldData = JsonUtility.FromJson<FieldData>(json);
                if (fieldData.HeaderType != nameof(FieldData)) continue;
                var field = GameObject.Instantiate(fieldPrefabDB.LoadedResources[fieldData.Name]);
                field.transform.position = fieldData.Position;
                field.transform.eulerAngles = fieldData.Rotation;
                field.Size = fieldData.Size;
                var box = field.AddComponent<BoxCollider>();
                box.center = fieldData.Size / 2;
                box.size = fieldData.Size;
                box.isTrigger = true;
                var teh = field.AddComponent<TriggerEventHandler>();
                teh.OnEnter.AddListener((c) => { if (c.GetComponent<IPlayable>() != null) OnEnterField(field); });
                teh.OnExit.AddListener((c) => { if (c.GetComponent<IPlayable>() != null) OnExitField(field); });
                fieldInfos.Add(field, fieldJsonDB[i].text);
                if (fieldData.Name == "EntryPoint") entry = fieldData;
            }
        }
        FieldData entry;
        void InitializePlayableCharacter()
        {
            if (playable != null) DisposeCharacter(playable);
            var characterDB = Loader<GameObject, CharacterComponent>.GetLoader(nameof(IPlayable));
            var origin = characterDB.LoadedResources[set.PlayableCharacterID.ToString("D10")];
            var pos = entry?.Position ?? Vector3.zero;
            var rot = entry?.Rotation ?? Vector3.zero;
            playable = GameObject.Instantiate(origin, pos, Quaternion.Euler(rot));
            OnBirthCharacter(playable);
        }
    }
}