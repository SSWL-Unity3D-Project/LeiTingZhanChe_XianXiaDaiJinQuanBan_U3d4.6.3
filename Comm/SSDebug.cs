using UnityEngine;

public class SSDebug
{
    static string m_DebugHeadInfo = "Unity: ";
    public static void Log(string info)
    {
        Debug.Log(m_DebugHeadInfo + info);
    }

    public static void LogWarning(string info)
    {
        Debug.LogWarning(m_DebugHeadInfo + info);
    }

    public static void LogError(string info)
    {
        Debug.LogError(m_DebugHeadInfo + info);
    }
}
