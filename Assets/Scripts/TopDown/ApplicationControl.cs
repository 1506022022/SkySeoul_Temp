#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class ApplicationControl
{
    public static void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // 에디터 실행 종료
#else
        Application.Quit(); // 정식 빌드 종료
#endif
    }

}
