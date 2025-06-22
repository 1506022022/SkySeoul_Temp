using System.Collections;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    [SerializeField] AttackBoxComponent attackContainer;
    bool initialized;
    [field: SerializeField] public Skill Skill { get; set; }
    public SkillController Controller { get; set; } = new();

    void OnEnable()
    {
        if (!initialized) Initialize(Skill, Controller);
        StartCoroutine(InvokeSkill());
    }

    public void Fire()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    IEnumerator InvokeSkill()
    {
        Controller.Fire();
        while (Controller.Alive)
        {
            Controller.Update();
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void Initialize(Skill skill, SkillController controller)
    {
        if (attackContainer == null) return;
        Controller.Initialize(Skill, attackContainer.transform, attackContainer.AttackBox);
        initialized = true;
    }
}