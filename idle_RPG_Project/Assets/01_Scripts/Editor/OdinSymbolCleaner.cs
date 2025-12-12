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
        DefineSymbolManager.RemoveDefineSymbol(odinSymbols);
    }
}