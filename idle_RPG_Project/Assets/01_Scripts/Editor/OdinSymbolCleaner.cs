using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

[InitializeOnLoad]
public class OdinSymbolCleaner
{
    static OdinSymbolCleaner()
    {
        EditorApplication.delayCall += RemoveOdinSymbols;
    }

    private static void RemoveOdinSymbols()
    {
        bool odinInstalled = AssetDatabase.FindAssets("OdinInspector").Length > 0;
        if (odinInstalled)
            return;

        string[] odinSymbols =
        {
            "ODIN_INSPECTOR",
            "ODIN_INSPECTOR_3",
            "ODIN_INSPECTOR_4",
            "ODIN_INSPECTOR_5",
            "ODIN_INSPECTOR_6",
            "ODIN_INSPECTOR_7",
            "ODIN_INSPECTOR_8",
            "ODIN_INSPECTOR_9",
            "ODIN_INSPECTOR_10",
            "ODIN_INSPECTOR_11",
            "ODIN_INSPECTOR_12",
            "ODIN_INSPECTOR_13",
            "ODIN_INSPECTOR_14"
        };


        var allTargets = System.Enum.GetValues(typeof(NamedBuildTarget)).Cast<NamedBuildTarget>();

        foreach (var target in allTargets)
        {
            try
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbols(target);
                if (string.IsNullOrEmpty(symbols))
                    continue;

                var list = symbols.Split(';').ToList();
                bool changed = false;

                foreach (string os in odinSymbols)
                {
                    if (list.Remove(os))
                        changed = true;
                }

                if (changed)
                {
                    string newSymbols = string.Join(";", list);
                    PlayerSettings.SetScriptingDefineSymbols(target, newSymbols);
                    Debug.Log($"[OdinSymbolCleaner] Removed Odin symbols from {target.TargetName}");
                }
            }
            catch
            {
                // 지원되지 않는 플랫폼은 무시
            }
        }
    }
}
