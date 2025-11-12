using System;
using System.Diagnostics; // serve per [Conditional]

// All calls to methods marked with [Conditional("UNITY_EDITOR")]
// or [Conditional("UNITY_EDITOR")] are compiled only if the code
// is running in the Editor or in a Build marked as "Developer Build".
// This means that the calls to the Log won't cause overhead in a "Standard" Build

public static class Log
{
    private const string TAG = "D_LOG";
    private const string COLOR_INFO = "orange";

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void D(object message)
    {
        UnityEngine.Debug.Log($"<color={COLOR_INFO}>{TAG}</color>-" + message);
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void D(object message, string color, string coloredMessage)
    {
        UnityEngine.Debug.Log($"<color={COLOR_INFO}>{TAG}</color>-<color={color}>{coloredMessage}</color>:{message}");
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void W(object message)
    {
        UnityEngine.Debug.LogWarning($"<color={COLOR_INFO}>W_{TAG}</color>-" + message);
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void W(object message, string color, string coloredMessage)
    {
        UnityEngine.Debug.LogWarning($"<color={COLOR_INFO}>W_{TAG}</color>-<color={color}>{coloredMessage}</color>:{message}");
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void E(object message)
    {
        UnityEngine.Debug.LogError($"<color={COLOR_INFO}>E_{TAG}</color>-" + message);
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void E(object message, string color, string coloredMessage)
    {
        UnityEngine.Debug.LogError($"<color={COLOR_INFO}>E_{TAG}</color>-<color={color}>{coloredMessage}</color>:{message}");
    }

    // Lazy log: the lambda isn't evaluated at all if logging is disabled
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void DLazy(Func<string> messageFactory)
    {
        UnityEngine.Debug.Log($"<color={COLOR_INFO}>{TAG}</color>-" + messageFactory());
    }
}
