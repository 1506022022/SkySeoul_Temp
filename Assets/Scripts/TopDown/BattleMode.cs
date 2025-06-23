using Battle;
using FieldEditorTool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Util;
using Entity;

namespace TopDown
{
    internal class BattleMode : IGameMode
    {
        public event Action OnQuit;
        ILoad loader;
        ModeSet set;
        BattleController battle = new();
        CharacterComponent playable;
        List<Loader> loaders;
        FieldComponent current;
        readonly List<CharacterComponent> enemy = new();
        readonly StringPair CreateTypeToEvent;
        readonly Loader<GameObject, CharacterComponent> characterDB;
        readonly Loader<GameObject, CharacterComponent> enemyDB;

        public BattleMode()
        {
            battle.OnDead += OnDeadCharacter;
            CreateTypeToEvent = Resources.Load<StringPair>(nameof(CreateTypeToEvent));
            Container.Bind<FieldData>().To<FieldIniter>().AsSingle();
            Container.Bind<EntryPoint>().To<FieldIniter>().AsSingle();
            Container.Bind<ActorData>().To<ActorIniter>().AsSingle();
            Container.Bind<ElementsData>().To<BarricadeIniter>().AsSingle();

            characterDB = Loader<GameObject, CharacterComponent>.GetLoader(nameof(IPlayable));
            enemyDB = Loader<GameObject, CharacterComponent>.GetLoader(nameof(IEnemy));
        }
        public void Load(ModeSet set)
        {
            this.set = set;
            var name = MapType.Lobby.ToString() + "Loader";
            loader = LoaderFactory.Load<ILoad>(name, name);
            loaders = new List<Loader>
            {
                Loader<TextAsset, TextAsset>.GetLoader(nameof(FieldData)),
                Loader<GameObject, FieldComponent>.GetLoader(nameof(FieldComponent)),
                Loader<GameObject, CharacterComponent>.GetLoader(nameof(IEnemy)),
                Loader<GameObject, CharacterComponent>.GetLoader(nameof(IPlayable)),
                Loader<GameObject, SkillComponent>.GetLoader(nameof(Skill)),
                new SceneLoader(MapType.BattleMap.ToString())
            };
            loader.Initialize(loaders);
            loader.Load();
            loader.OnLoaded += OnLoaded;
        }
        void OnLoaded()
        {
            var fieldJsonDB = Loader<TextAsset, TextAsset>.GetLoader(nameof(FieldData)).LoadedResources.Values.ToList();
            for (int i = 0; i < fieldJsonDB.Count; i++)
            {
                string[] jsons = fieldJsonDB[i].text.Split('\n');
                if (jsons == null || jsons.Length == 0) continue;
                var Json = JsonUtility.FromJson<FieldData>(jsons[0]);
                if (Json == null || Json.HeaderType != nameof(FieldData)) continue;
                Enumerator.InvokeFor(jsons, InitializeEntity);
            }
        }
        void InitializeEntity(string json)
        {
            var entity = JsonUtility.FromJson<EntityData>(json);
            var data = (EntityData)JsonUtility.FromJson(json, FieldEditorTool.Types.FindTypeByName<EntityData>(entity.HeaderType));
            var instance = Container.Instantiate(entity.HeaderType)?.Initialize(data);

            if (CreateTypeToEvent == null) return;
            if (!CreateTypeToEvent.Map.TryGetValue(entity.HeaderType, out var @event)) return;
            MethodInfo method = GetType().GetMethod(@event, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            method?.Invoke(this, new[] { instance });
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
        void OnCreatedField(FieldComponent field)
        {
            var teh = field.gameObject.AddComponent<TriggerEventHandler>();
            teh.OnEnter.AddListener((c) => { if (c.GetComponent<IPlayable>() != null) OnEnterField(field); });
            teh.OnExit.AddListener((c) => { if (c.GetComponent<IPlayable>() != null) OnExitField(field); });
        }
        void OnCreatedEntryPoint(FieldComponent entry)
        {
            if (playable != null) DisposeCharacter(playable);
            var origin = characterDB.LoadedResources[set.PlayableCharacterID.ToString("D10")];
            var pos = entry.gameObject.transform.position;
            var rot = entry.gameObject.transform.eulerAngles;
            playable = GameObject.Instantiate(origin, pos, Quaternion.Euler(rot));
            OnBirthCharacter(playable);
        }
        void OnExitField(FieldComponent field)
        {

        }
        void OnEnterField(FieldComponent field)
        {
            current = field;
        }
        void OnBirthCharacter(CharacterComponent character)
        {
            character.Initialize();
            battle.JoinCharacter(character);
            if (character is IEnemy) OnBirthEnemy(character);
            else if (character is IPlayable) OnBirthPlayableCharacter(character);
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
            var delay = Mathf.Max(3, character.deathDuration);
            GameObject.Destroy(character.gameObject, delay);
        }
    }

    #region Initer
    public class BarricadeIniter : ActorIniter
    {
        public override MonoBehaviour Initialize(EntityData data)
        {
            var instance = base.Initialize(data);
            if (instance is not Barricade barricade) return instance;
            if (data is not ElementsData elements) return instance;
            barricade.ModelPath = elements.ModelPath;

            var skillDB = Loader<GameObject, SkillComponent>.GetLoader(nameof(Skill));
            if (!skillDB.LoadedResources.TryGetValue(elements.ExitSkill, out var skill)) return instance;
            barricade.exitSkill = GameObject.Instantiate(skill);
            barricade.exitSkill.Disable();
            barricade.SkillOffset = elements.SkillOffset;
            barricade.SkillRotation = elements.Rotation;
            return barricade;
        }
    }
    public class ActorIniter : IIniter
    {

        public virtual MonoBehaviour Initialize(EntityData data)
        {
            if (data is not ActorData actorData) return null;
            var enemy = Loader<GameObject, CharacterComponent>.GetLoader(nameof(IEnemy));
            var pc = Loader<GameObject, CharacterComponent>.GetLoader(nameof(IPlayable));

            CharacterComponent origin;
            if (enemy.LoadedResources.TryGetValue(actorData.Name, out origin)) { }
            else if (!pc.LoadedResources.TryGetValue(actorData.Name, out origin)) return null;

            var actor = GameObject.Instantiate(origin);
            actor.transform.position = actorData.Position;
            actor.transform.eulerAngles = actorData.Rotation;
            actor.HP.Initialize(actorData.HP, actorData.HP);
            actor.SetTeam(actorData.Team);
            actor.deathDuration = actorData.ExitDuration;

            return actor;
        }
    }
    public class FieldIniter : IIniter
    {

        public MonoBehaviour Initialize(EntityData data)
        {
            if (data is not FieldData fieldData) return null;
            var fieldPrefabDB = Loader<GameObject, FieldComponent>.GetLoader(nameof(FieldComponent));
            var field = GameObject.Instantiate(fieldPrefabDB.LoadedResources[fieldData.Name]);
            field.transform.position = fieldData.Position;
            field.transform.eulerAngles = fieldData.Rotation;
            field.Size = fieldData.Size;
            var box = field.gameObject.AddComponent<BoxCollider>();
            box.center = fieldData.Size / 2;
            box.size = fieldData.Size;
            box.isTrigger = true;

            return field;
        }
    }
    public interface IIniter
    {
        public MonoBehaviour Initialize(EntityData data);
    }
    public class Container
    {
        static readonly Dictionary<string, System.Type> transient = new();
        static readonly Dictionary<string, IIniter> single = new();

        System.Type bindBase;
        System.Type bindTarget;

        private Container()
        {

        }

        public static Container Bind<Base>() where Base : EntityData
        {
            return new Container() { bindBase = typeof(Base) };
        }

        public Container To<Initer>() where Initer : IIniter
        {
            bindTarget = typeof(Initer);
            return this;
        }

        public void AsSingle()
        {
            IIniter initer = (IIniter)Activator.CreateInstance(bindTarget);
            if (initer == null) return;
            if (!single.TryAdd(bindBase.Name, initer)) single[bindBase.Name] = initer;
        }

        public void AsTransient()
        {
            if (!transient.TryAdd(bindBase.Name, bindTarget)) transient[bindBase.Name] = bindTarget;
        }

        public static IIniter Instantiate(string type)
        {
            IIniter result = null;
            if (transient.TryGetValue(type, out var initer))
            {
                result = (IIniter)Activator.CreateInstance(initer);
            }
            else single.TryGetValue(type, out result);

            return result;
        }
    }
    #endregion
}