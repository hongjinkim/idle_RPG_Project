#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEngine;
using UnityEditor;
using System.Numerics;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

// BigInteger 타입을 만날 때마다 이 코드가 대신 화면을 그립니다.
public class BigIntegerDrawer : OdinValueDrawer<BigInteger>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        // 1. 현재 값 가져오기
        BigInteger value = this.ValueEntry.SmartValue;

        // 2. 가로 배치 시작 (입력창 + 포맷된 단위 표시)
        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            // [라벨] 그리기
            if (label != null)
            {
                GUILayout.Label(label, GUILayout.Width(EditorGUIUtility.labelWidth));
            }

            // [입력창] 문자열로 변환하여 보여주고, 수정하면 다시 BigInteger로 파싱
            string strValue = value.ToString();
            string newValueStr = EditorGUILayout.TextField(strValue);

            // 값이 바뀌었고, 숫자로 변환 가능하다면 값 업데이트
            if (strValue != newValueStr && BigInteger.TryParse(newValueStr, out BigInteger result))
            {
                this.ValueEntry.SmartValue = result;
            }

            // [단위 표시] 옆에 "1.25A" 처럼 보기 좋게 표시 (읽기 전용)
            // 아까 만든 BigIntegerExtension이 있다면 활용
            string currencyText = "0";
            try
            {
                // 확장 메서드 호출 (없으면 이 부분 주석 처리)
                currencyText = value.ToFormattedString(ENumberFormatType.Standard);
            }
            catch { }

            // 회색 박스로 단위 보여주기
            GUILayout.Space(5);
            SirenixEditorGUI.BeginBox();
            GUILayout.Label(currencyText, EditorStyles.miniLabel, GUILayout.Width(40));
            SirenixEditorGUI.EndBox();
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}
#endif