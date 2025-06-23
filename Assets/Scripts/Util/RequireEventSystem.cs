using UnityEngine;
using UnityEngine.EventSystems;

public class RequireEventSystem : MonoBehaviour
{
    EventSystem es;
    StandaloneInputModule sim;

    void OnEnable()
    {
        RequireComponent(out sim);
        RequireComponent(out es);
    }
    void OnDisable()
    {
        ClearComponent(sim);
        ClearComponent(es);
    }

    void RequireComponent<T>(out T component) where T : Component
    {
        component = GetComponent<T>();
        if (component != null) return;
        if (FindAnyObjectByType<T>() != null) return;
        component = gameObject.AddComponent<T>();
    }

    void ClearComponent<T>(T component) where T : Component
    {
        if (component != null) Destroy(component);
    }
}