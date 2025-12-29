using System;
using System.Diagnostics;

namespace EchoCity
{
        public static class Log
        {
                private const string MAIN_TAG = "D_LOG";
                private const string DEFAULT_COLOR = "orange";

                // --- LOG DEFAULT (D) ---
                [Conditional("UNITY_EDITOR")]
                [Conditional("DEVELOPMENT_BUILD")]
                public static void D(object message) => LogToUnity(message.ToString(), "Log", DEFAULT_COLOR, MAIN_TAG);

                [Conditional("UNITY_EDITOR")]
                [Conditional("DEVELOPMENT_BUILD")]
                public static void D(object message, string contextTag, string customColor = DEFAULT_COLOR)
                    => LogToUnity(message.ToString(), "Log", customColor, $"{MAIN_TAG}][{contextTag}");

                [Conditional("UNITY_EDITOR")]
                [Conditional("DEVELOPMENT_BUILD")]
                public static void DLazy(Func<string> messageFactory, string contextTag = null, string customColor = DEFAULT_COLOR)
                    => LogToUnity(messageFactory(), "Log", customColor, contextTag == null ? MAIN_TAG : $"{MAIN_TAG}][{contextTag}");

                // --- WARNING (W) ---
                [Conditional("UNITY_EDITOR")]
                [Conditional("DEVELOPMENT_BUILD")]
                public static void W(object message) => LogToUnity(message.ToString(), "Warning", DEFAULT_COLOR, "W_" + MAIN_TAG);

                [Conditional("UNITY_EDITOR")]
                [Conditional("DEVELOPMENT_BUILD")]
                public static void W(object message, string contextTag, string customColor = DEFAULT_COLOR)
                    => LogToUnity(message.ToString(), "Warning", customColor, $"W_{MAIN_TAG}][{contextTag}");

                [Conditional("UNITY_EDITOR")]
                [Conditional("DEVELOPMENT_BUILD")]
                public static void WLazy(Func<string> messageFactory, string contextTag = null, string customColor = DEFAULT_COLOR)
                    => LogToUnity(messageFactory(), "Warning", customColor, contextTag == null ? "W_" + MAIN_TAG : $"W_{MAIN_TAG}][{contextTag}");

                // --- ERROR (E) ---
                [Conditional("UNITY_EDITOR")]
                [Conditional("DEVELOPMENT_BUILD")]
                public static void E(object message) => LogToUnity(message.ToString(), "Error", DEFAULT_COLOR, "E_" + MAIN_TAG);

                [Conditional("UNITY_EDITOR")]
                [Conditional("DEVELOPMENT_BUILD")]
                public static void E(object message, string contextTag, string customColor = DEFAULT_COLOR)
                    => LogToUnity(message.ToString(), "Error", customColor, $"E_{MAIN_TAG}][{contextTag}");

                [Conditional("UNITY_EDITOR")]
                [Conditional("DEVELOPMENT_BUILD")]
                public static void ELazy(Func<string> messageFactory, string contextTag = null, string customColor = DEFAULT_COLOR)
                    => LogToUnity(messageFactory(), "Error", customColor, contextTag == null ? "E_" + MAIN_TAG : $"E_{MAIN_TAG}][{contextTag}");

                // --- CORE LOGIC ---
                private static void LogToUnity(string message, string type, string tagColor, string tagText)
                {
#if UNITY_EDITOR
                        // Formato: [D_LOG][CONTESTO]-Messaggio (con colori)
                        string formattedMsg = $"<color={tagColor}>[{tagText}]</color>-{message}";
#else
            // Formato: [D_LOG][CONTESTO]-Messaggio (senza colori per log file)
            string formattedMsg = $"[{tagText}]-{message}";
#endif

                        switch (type)
                        {
                                case "Warning": UnityEngine.Debug.LogWarning(formattedMsg); break;
                                case "Error": UnityEngine.Debug.LogError(formattedMsg); break;
                                default: UnityEngine.Debug.Log(formattedMsg); break;
                        }
                }
        }
}