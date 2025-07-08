using UnityEngine;

namespace Character
{
    public sealed class LampComponent : PropBaseComponent, ISkillOwner, ICaster
    {
        [field: SerializeField] public SkillComponent Skill { get; set; }
        [field: SerializeField] public Vector3 SkillOffset { get; set; }
        [field: SerializeField] public Vector3 SkillRotation { get; set; }

        SkillComponent SkillInstance;

        bool initialize => Skill != null && SkillInstance != null;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (initialize || Skill == null) return;
            SkillInstance = GameObject.Instantiate(Skill);
            SkillInstance.Disable();
        }

        void ICaster.Invoke()
        {
            if (!initialize) return;

            SkillInstance.transform.position = transform.position + SkillOffset;
            SkillInstance.transform.eulerAngles = transform.eulerAngles + SkillRotation;
            SkillInstance.Fire();
        }

        void ICaster.Cancel()
        {
            if (!initialize) return;

            SkillInstance.Disable();
        }
    }
}