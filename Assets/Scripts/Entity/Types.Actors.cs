using FieldEditorTool;
using System;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class ActorData : EntityData
    {
        public string Name;
        public int HP;
        public LayerMask Team;
        public Vector3 Position;
        public Vector3 Rotation;
        public float EnterDuration;
        public float ExitDuration;
    }

    [Serializable]
    public class ElementsData : ActorData
    {
        public string ExitSkill;
        public Vector3 SkillOffset;
        public Vector3 SkillDirection;
    }
}