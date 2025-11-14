using System;
using System.Diagnostics; // needed for [Conditional]

// All calls to methods marked with [Conditional("UNITY_EDITOR")]
// or [Conditional("DEVELOPMENT_BUILD")] are compiled only if the code
// is running in the Editor or in a Build marked as "Developer Build".
// When not in the Unity Editor we avoid Unity rich-text color tags and use plain tags.

public static class Log
{
    private const string TAG = "D_LOG";
    private const string COLOR_INFO = "orange";

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void D(object message)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log($"<color={COLOR_INFO}>{TAG}</color>-" + message);
#else
        UnityEngine.Debug.Log($"{TAG}-{message}");
#endif
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void D(object message, string color, string coloredMessage)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log($"<color={COLOR_INFO}>{TAG}</color>-<color={color}>{coloredMessage}</color>:{message}");
#else
        UnityEngine.Debug.Log($"{TAG}-{coloredMessage}:{message}");
#endif
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void W(object message)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogWarning($"<color={COLOR_INFO}>W_{TAG}</color>-" + message);
#else
        UnityEngine.Debug.LogWarning($"W_{TAG}-{message}");
#endif
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void W(object message, string color, string coloredMessage)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogWarning($"<color={COLOR_INFO}>W_{TAG}</color>-<color={color}>{coloredMessage}</color>:{message}");
#else
        UnityEngine.Debug.LogWarning($"W_{TAG}-{coloredMessage}:{message}");
#endif
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void E(object message)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogError($"<color={COLOR_INFO}>E_{TAG}</color>-" + message);
#else
        UnityEngine.Debug.LogError($"E_{TAG}-{message}");
#endif
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void E(object message, string color, string coloredMessage)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogError($"<color={COLOR_INFO}>E_{TAG}</color>-<color={color}>{coloredMessage}</color>:{message}");
#else
        UnityEngine.Debug.LogError($"E_{TAG}-{coloredMessage}:{message}");
#endif
    }

    // Lazy log: the lambda isn't evaluated at all if logging is disabled
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void DLazy(Func<string> messageFactory)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log($"<color={COLOR_INFO}>{TAG}</color>-" + messageFactory());
#else
        UnityEngine.Debug.Log($"{TAG}-{messageFactory()}");
#endif
    }
}