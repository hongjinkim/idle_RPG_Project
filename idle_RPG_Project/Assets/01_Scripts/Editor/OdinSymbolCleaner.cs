using UnityEngine;
using UnityEditor;
using System.Linq;

[InitializeOnLoad]
public class OdinSymbolCleaner
{
    static OdinSymbolCleaner()
    {
        EditorApplication.delayCall += RemoveOdinSymbols;
    }
    private static void RemoveOdinSymbols()
    {
        bool odinInstalled = UnityEditor.AssetDatabase.FindAssets("OdinInspector").Length > 0;
        if (odinInstalled)
        {
            return;
        }

        var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var symbolsList = currentSymbols.Split(';').ToList();
        bool symbolsChanged = false;
        string[] odinSymbols = new string[]
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
        foreach (var symbol in odinSymbols)
        {
            if (symbolsList.Contains(symbol))
            {
                symbolsList.Remove(symbol);
                symbolsChanged = true;
            }
        }
        if (symbolsChanged)
        {
            string newSymbols = string.Join(";", symbolsList);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newSymbols);
            Debug.Log("Odin Inspector symbols removed from scripting define symbols.");
        }
    }

}
