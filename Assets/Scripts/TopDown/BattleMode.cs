using Battle;
using FieldEditorTool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Util;
using Entity;
using Cysharp.Threading.Tasks;
using Character;
using Unity.VisualScripting;

namespace TopDown
{
    internal class BattleMode : IGameMode
    {
        public event Action OnQuit;
        ILoad resourceLoader;
        ModeSet modeSet;
        readonly BattleController battle = new();
        IPlayable playerCharacter;
        List<Loader> loaders;
        FieldComponent current, rootField;
        readonly Dictionary<FieldComponent, bool> fieldClear = new();
        readonly StringPair CreateTypeToEvent;
        readonly Loader<GameObject, IPlayable> characterDB;
        readonly Loader<GameObject, IEnemy> enemyDB;
        readonly Loader<GameObject, IProp> propDB;

        public BattleMode()
        {
            battle.OnDead += OnDeadCharacter;
            CreateTypeToEvent = Resources.Load<StringPair>(nameof(CreateTypeToEvent));

            Container.Bind<FieldData>().To<FieldIniter>().AsSingle();
            Container.Bind<EntryPoint>().To<FieldIniter>().AsSingle();
            Container.Bind<ActorData>().To<ActorIniter>().AsSingle();
            Container.Bind<PropData>().To<PropIniter>().AsSingle();

            characterDB = Loader<GameObject, IPlayable>.GetLoader(nameof(IPlayable));
            enemyDB = Loader<GameObject, IEnemy>.GetLoader(nameof(IEnemy));
            propDB = Loader<GameObject, IProp>.GetLoader(nameof(IProp));
        }
        public void Load(ModeSet set)
        {
            this.modeSet = set;
            var name = MapType.Lobby.ToString() + "Loader";
            resourceLoader = LoaderFactory.Load<ILoad>(name, name);
            loaders = new List<Loader>
            {
                Loader<TextAsset, TextAsset>.GetLoader(nameof(FieldData)),
                Loader<GameObject, FieldComponent>.GetLoader(nameof(FieldComponent)),
                Loader<GameObject, IEnemy>.GetLoader(nameof(IEnemy)),
                Loader<GameObject, IPlayable>.GetLoader(nameof(IPlayable)),
                Loader<GameObject, IProp>.GetLoader(nameof(IProp)),
                Loader<GameObject, SkillComponent>.GetLoader(nameof(Skill)),
                new SceneLoader(MapType.BattleMap.ToString())
            };
            resourceLoader.Initialize(loaders);
            resourceLoader.Load();
            resourceLoader.OnLoaded += OnLoaded;
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
            IActor instance = Container.Instantiate(entity.HeaderType)?.Initialize(data);

            if (CreateTypeToEvent == null) return;
            if (!CreateTypeToEvent.Map.TryGetValue(entity.HeaderType, out var @event)) return;
            MethodInfo method = GetType().GetMethod(@event, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            method?.Invoke(this, new[] { instance });
        }
        void ExitMode()
        {
            battle.Clear();
            LoaderFactory.Unload();
            while (loaders.Count > 0)
            {
                loaders[0].Unload();
                loaders.RemoveAt(0);
            }
            modeSet.MapType = MapType.Lobby;
            OnQuit?.Invoke();
        }

        async void OnClearBattle()
        {
            if (playerCharacter is IControlable controlable && playerCharacter is IActor actor)
                controlable.SetController(new EmptyJoycon(actor));

            await UniTask.WaitForSeconds(3f);
            ExitMode();
        }
        void OnCreatedEntryPoint(FieldComponent entry)
        {
            if (playerCharacter is IActor oldActor) DisposeActor(oldActor);
            var cache = characterDB.LoadedResources[modeSet.PlayableCharacterID.ToString("D10")];
            if (cache == null)
            {
                Debug.LogWarning($"load fail: {modeSet.PlayableCharacterID.ToString("D10")}");
                return;
            }
            if (cache is ITransform transform)
            {
                var origin = transform.transform.gameObject;
                var pos = entry.gameObject.transform.position;
                var rot = entry.gameObject.transform.eulerAngles;
                playerCharacter = (GameObject.Instantiate(origin, pos, Quaternion.Euler(rot))).GetComponent<IPlayable>();
                if (playerCharacter is IActor newActor) OnBirthActor(newActor);
            }
            else
            {
                Debug.LogWarning($"create fail: {modeSet.PlayableCharacterID.ToString("D10")}");
            }
        }
        void OnCreatedField(FieldComponent field)
        {
            rootField = field;
            var teh = field.gameObject.AddComponent<TriggerEventHandler>();
            teh.OnEnter.AddListener((c) => { if (c.transform.GetComponentInParent<IPlayable>() != null) OnEnterField(field); });
            teh.OnExit.AddListener((c) => { if (c.transform.GetComponentInParent<IPlayable>() != null) OnExitField(field); });
            fieldClear.Add(field, false);
            field.Dispose();
        }
        void OnEnterField(FieldComponent field)
        {
            current = field;
            field.Initialize();
        }
        void OnClearField(FieldComponent field)
        {
            fieldClear[field] = true;
            if (fieldClear.Values.All(clear => clear)) OnClearBattle();
        }
        void OnExitField(FieldComponent field)
        {
            field.Dispose();
        }
        void OnBirthActor(IActor actor)
        {
            battle.JoinCharacter(actor);
            if (actor is IEnemy enemy) OnBirthEnemy(enemy);
            else if (actor is IPlayable playableCharacter) OnBirthPlayableCharacter(playableCharacter);
            else if (actor is IProp prop) OnBirthProp(prop);
        }
        void OnBirthEnemy(IEnemy enemy)
        {
            rootField.Add(enemy);
        }
        void OnBirthPlayableCharacter(IPlayable pc)
        {
            if (pc is IInitializable initializable) initializable.Initialize();
        }
        void OnBirthProp(IProp prop)
        {
        }
        void OnDeadCharacter(IActor actor)
        {
            if (actor is IPlayable playable) OnDeadPlayableCharacter(playable);
            else if (actor is IEnemy enemy) OnDeadEnemy(enemy);
            else if (actor is IProp prop) OnDestroyProp(prop);
            DisposeActor(actor);
        }
        void OnDeadPlayableCharacter(IPlayable character)
        {
            ExitMode();
        }
        void OnDeadEnemy(IEnemy enemy)
        {
            if (enemy is not MonsterComponent monster) return;
            if (current == null)
            {
                Debug.LogWarning("not found field");
                return;
            }
            current.Remove(monster);

            if (current.enemys.Count == 0 && !fieldClear[current]) OnClearField(current);
        }
        void OnDestroyProp(IProp prop)
        {

        }
        void DisposeActor(IActor actor)
        {
            battle.DisposeCharacter(actor);
            if (actor is ITransform tActor)
            {
                float delay = actor is IDeathable deathable ? Mathf.Max(3, deathable.DeathDuration) : 0f;
                GameObject.Destroy(tActor.transform.gameObject, delay);
            }
        }
    }

    #region Initer
    public class PropIniter : ActorIniter
    {
        public override IActor Initialize(EntityData data)
        {
            var instance = base.Initialize(data);
            if (instance is not IProp prop) return instance;
            if (data is not PropData elements) return instance;

            if (instance is not ISkillOwner death) return instance;
            var skillDB = Loader<GameObject, SkillComponent>.GetLoader(nameof(Skill));
            if (!skillDB.LoadedResources.TryGetValue(elements.ExitSkill, out var skill)) return instance;
            death.Skill = GameObject.Instantiate(skill);
            death.Skill.Disable();
            death.SkillOffset = elements.SkillOffset;
            death.SkillRotation = elements.Rotation;
            return instance;
        }
    }
    public class ActorIniter : IIniter
    {

        public virtual IActor Initialize(EntityData data)
        {
            if (data is not ActorData actorData) return null;
            var enemyDB = Loader<GameObject, IEnemy>.GetLoader(nameof(IEnemy));
            var pcDB = Loader<GameObject, IPlayable>.GetLoader(nameof(IPlayable));
            var propDB = Loader<GameObject, IProp>.GetLoader(nameof(IProp));

            GameObject cache = null;
            if (enemyDB.LoadedResources.TryGetValue(actorData.Name, out var enemy))
            {
                cache = (enemy as ITransform)?.transform.gameObject;
            }
            else if (pcDB.LoadedResources.TryGetValue(actorData.Name, out var pc))
            {
                cache = (pc as ITransform)?.transform.gameObject;
            }
            else if (propDB.LoadedResources.TryGetValue(actorData.Name, out var prop))
            {
                cache = (prop as ITransform)?.transform.gameObject;
            }
            if (cache == null)
            {
                Debug.LogWarning($"Create failed: {actorData.Name}");
                return null;
            }
            var actor = GameObject.Instantiate(cache).GetComponent<IActor>();
            if (actor == null)
            {
                return null;
            }

            if (actor is ITransform transform)
            {
                transform.transform.position = actorData.Position;
                transform.transform.eulerAngles = actorData.Rotation;
            }
            if (actor is IHP health)
            {
                health.HP.Initialize(actorData.HP, actorData.HP);
            }
            if (actor is IDeathable death)
            {
                death.DeathDuration = actorData.ExitDuration;
            }
            //actor.SetTeam(actorData.Team);

            return actor;
        }
    }
    public class FieldIniter : IIniter
    {

        public IActor Initialize(EntityData data)
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
        public IActor Initialize(EntityData data);
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