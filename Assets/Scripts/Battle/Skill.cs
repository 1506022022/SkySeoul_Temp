using Battle;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;

[Serializable]
public struct SkillFrame
{
    public uint ms;
    public uint duration;
    public Vector3 Offset;
    public Vector3 Rotation;
    public Vector3 Scale;
}

[Serializable]
public class Skill
{
    [SerializeField] List<SkillFrame> frames = new();
    public List<SkillFrame> Frames => frames.ToList();
    [NonSerialized] public Transform Transform;
    [NonSerialized] public AttackBox AttackBox;
}

public class SkillController : IController
{
    Skill skill;
    float firedTime;
    int pos;
    uint ms;

    List<SkillFrame> frames => skill?.Frames ?? new();
    public bool Alive => IsInitialized && pos != -2;
    public bool IsInitialized => skill.Transform != null && skill.AttackBox != null && skill.Frames.Count > 0;
    public void Initialize(Skill skill, Transform transform, AttackBox attackBox)
    {
        this.skill = skill;
        this.skill.Transform = transform;
        this.skill.AttackBox = attackBox;
    }
    public void Fire()
    {
        firedTime = Time.time;
        pos = -1;
        ms = uint.MaxValue;
    }
    public void Update()
    {
        if (!IsInitialized) return;
        if (!UpdateMS()) return;

        if (pos == frames.Count - 1 && frames.Last().ms + frames.Last().duration <= ms)
        {
            OnDeadSkill();
            return;
        }

        if (UpdatePos())
        {
            OnUpdateSkillFrame();
        }
    }
    bool UpdateMS()
    {
        uint ms = (uint)((Time.time - firedTime) * 1000);
        bool updated = this.ms != ms;
        this.ms = ms;
        return updated;
    }
    bool UpdatePos()
    {
        int prePos = pos;
        for (int i = Mathf.Max(0, pos); i < frames.Count; i++)
        {
            if (ms < frames[i].ms) break; else pos = i;
        }
        bool updated = prePos != pos;
        return updated;
    }
    void OnDeadSkill()
    {
        pos = -2;
    }
    void OnUpdateSkillFrame()
    {
        var frame = frames[pos];
        skill.Transform.localPosition = frame.Offset;
        skill.Transform.localEulerAngles = frame.Rotation;
        skill.Transform.localScale = frame.Scale;
        skill.AttackBox.SetAttackWindow(frame.duration / 1000f);
        skill.AttackBox.OpenAttackWindow();
    }
}

//[CustomPropertyDrawer(typeof(Skill))]
public class SkillDrawer : PropertyDrawer
{
    SerializedProperty property;
    int ms;
    int maxMs = int.MaxValue;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        this.property = property;
        DrawHeader(ref position, property);
        DrawAttackGraph(ref position, property);
        DrawGraphSlider(ref position, property);
        DrawEdit(ref position, property);
    }
    void DrawHeader(ref Rect position, SerializedProperty property)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        GUIStyle lable = new(GUI.skin.label) { fontStyle = FontStyle.Bold };
        GUIContent con = new GUIContent() { text = nameof(Skill) };
        EditorGUI.LabelField(position, con, lable);
        position.y += EditorGUIUtility.singleLineHeight;
    }
    static void DrawAttackGraph(ref Rect position, SerializedProperty property)
    {
        position.height = 30;
        AnimationCurve curve = new();

        EditorGUI.CurveField(position, curve);
        position.y += position.height;
        position.height = EditorGUIUtility.singleLineHeight;
    }
    void DrawGraphSlider(ref Rect position, SerializedProperty property)
    {
        EditorGUI.GradientField(position, new Gradient());
        ms = EditorGUI.IntSlider(position, ms, 0, maxMs - 100);
        position.y += EditorGUIUtility.singleLineHeight;
    }
    void DrawEdit(ref Rect position, SerializedProperty property)
    {
        var prob = property.FindPropertyRelative("frames");
        if (prob.arraySize == 0) property.InsertArrayElementAtIndex(0);
        var first = prob.GetArrayElementAtIndex(0);
        EditorGUI.PropertyField(position, first, true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 12;
    }
}