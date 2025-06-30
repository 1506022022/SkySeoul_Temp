using UnityEngine;

namespace Character
{
    public sealed class LampComponent : PropBaseComponent, IDeathSkillOwner, IHP
    {
        [field: SerializeField] public SkillComponent DeathSkill { get; set; }
        [field: SerializeField] public Vector3 DeathSkillOffset { get; set; }
        [field: SerializeField] public Vector3 DeathSkillRotation { get; set; }

        Statistics IHP.HP { get; } = new Statistics(1);

        protected override void OnInitialize()
        {
        }

        protected override void OnDie()
        {
            if (DeathSkill == null) return;
            var exitSkillInstance = GameObject.Instantiate(DeathSkill);
            exitSkillInstance.transform.position = transform.position + DeathSkillOffset;
            exitSkillInstance.transform.eulerAngles = transform.eulerAngles + DeathSkillRotation;
            exitSkillInstance.Fire();
        }
    }
}