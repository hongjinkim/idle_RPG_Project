#if !ODIN_INSPECTOR
using System;
using System.Diagnostics;
using UnityEngine;

namespace Sirenix.OdinInspector
{
    // -------------------------------------------------------------------------
    // params object[]를 써서 어떤 인자가 들어와도 에러 없이 받아냅니다.
    // -------------------------------------------------------------------------
    [Conditional("UNITY_EDITOR")]
    public abstract class MockAttribute : Attribute
    {
        public MockAttribute(params object[] args) { }
    }

    // =========================================================================
    // 1. 그룹 (Groups) - 레이아웃 정리용
    // =========================================================================
    public class BoxGroupAttribute : MockAttribute { public BoxGroupAttribute(string group = "", bool showLabel = true, bool centerLabel = false, float order = 0) : base() { } }
    public class FoldoutGroupAttribute : MockAttribute { public FoldoutGroupAttribute(string group, int order = 0) : base() { } public FoldoutGroupAttribute(string group, bool expanded, int order = 0) : base() { } }
    public class TabGroupAttribute : MockAttribute { public TabGroupAttribute(string group, bool useFixedHeight = false, int order = 0) : base() { } }
    public class TitleGroupAttribute : MockAttribute { public TitleGroupAttribute(string title, string subtitle = null, TitleAlignments alignment = TitleAlignments.Left, bool horizontalLine = true, bool bold = true, bool indent = false, float order = 0) : base() { } }
    public class HorizontalGroupAttribute : MockAttribute { public HorizontalGroupAttribute(string group = "", float width = 0, int marginLeft = 0, int marginRight = 0, float order = 0) : base() { } }
    public class VerticalGroupAttribute : MockAttribute { public VerticalGroupAttribute(string group = "", float order = 0) : base() { } }
    public class ButtonGroupAttribute : MockAttribute { public ButtonGroupAttribute(string group = "") : base() { } }
    public class ToggleGroupAttribute : MockAttribute { public ToggleGroupAttribute(string toggleMemberName, float order = 0, string groupTitle = null) : base() { } }

    // =========================================================================
    // 2. 행동 및 버튼 (Actions & Buttons)
    // =========================================================================
    public class ButtonAttribute : MockAttribute 
    { 
        public ButtonAttribute() : base() { }
        public ButtonAttribute(ButtonSizes size) : base() { }
        public ButtonAttribute(string name) : base() { }
        public ButtonAttribute(string name, ButtonSizes size) : base() { }
        public string Name { get; set; }
        public ButtonSizes ButtonSize { get; set; }
        public ButtonStyle ButtonStyle { get; set; }
    }
    public class OnValueChangedAttribute : MockAttribute { public OnValueChangedAttribute(string action, bool includeChildren = false) : base() { } }
    public class OnInspectorInitAttribute : MockAttribute { public OnInspectorInitAttribute(string action) : base() { } }
    public class OnInspectorGUIAttribute : MockAttribute { public OnInspectorGUIAttribute() : base() { } }

    // =========================================================================
    // 3. 상태 제어 (State & Visibility)
    // =========================================================================
    public class ShowIfAttribute : MockAttribute { public ShowIfAttribute(string condition, object optionalValue = null) : base() { } }
    public class HideIfAttribute : MockAttribute { public HideIfAttribute(string condition, object optionalValue = null) : base() { } }
    public class EnableIfAttribute : MockAttribute { public EnableIfAttribute(string condition, object optionalValue = null) : base() { } }
    public class DisableIfAttribute : MockAttribute { public DisableIfAttribute(string condition, object optionalValue = null) : base() { } }
    public class ReadOnlyAttribute : MockAttribute { public ReadOnlyAttribute() : base() { } }
    public class ShowInInspectorAttribute : MockAttribute { public ShowInInspectorAttribute() : base() { } }
    public class HideInInspectorAttribute : MockAttribute { public HideInInspectorAttribute() : base() { } }
    public class HideInEditorModeAttribute : MockAttribute { public HideInEditorModeAttribute() : base() { } }
    public class HideInPlayModeAttribute : MockAttribute { public HideInPlayModeAttribute() : base() { } }

    // =========================================================================
    // 4. 시각적 표현 (Visuals)
    // =========================================================================
    public class LabelTextAttribute : MockAttribute { public LabelTextAttribute(string text) : base() { } }
    public class LabelWidthAttribute : MockAttribute { public LabelWidthAttribute(float width) : base() { } }
    public class TitleAttribute : MockAttribute { public TitleAttribute(string title, string subtitle = null, TitleAlignments titleAlignment = TitleAlignments.Left, bool horizontalLine = true, bool bold = true) : base() { } }
    public class InfoBoxAttribute : MockAttribute { public InfoBoxAttribute(string message, InfoMessageType infoMessageType = InfoMessageType.Info, string visibleIfMemberName = null) : base() { } }
    public class DisplayAsStringAttribute : MockAttribute { public DisplayAsStringAttribute(bool overflow = true) : base() { } }
    public class PropertySpaceAttribute : MockAttribute { public PropertySpaceAttribute(float spaceBefore = 0, float spaceAfter = 0) : base() { } }
    public class PropertyOrderAttribute : MockAttribute { public PropertyOrderAttribute(float order) : base() { } }
    public class GUIColorAttribute : MockAttribute { public GUIColorAttribute(float r, float g, float b, float a = 1f) : base() { } }
    public class ColorPaletteAttribute : MockAttribute { public ColorPaletteAttribute(string name = null) : base() { } }
    public class PreviewFieldAttribute : MockAttribute { public PreviewFieldAttribute(float height = 0, ObjectFieldAlignment alignment = ObjectFieldAlignment.Left) : base() { } }
    public class ProgressBarAttribute : MockAttribute { public ProgressBarAttribute(double min, string colorMemberName = null) : base() { } }

    // =========================================================================
    // 5. 값 및 유효성 (Values & Validation)
    // =========================================================================
    public class ValueDropdownAttribute : MockAttribute 
    { 
        public ValueDropdownAttribute(string memberName) : base() { }
        // 드롭다운은 옵션이 많으므로 필드도 몇 개 더미로 만들어둡니다.
        public bool AppendNextDrawer { get; set; }
        public bool DisableGUIInAppendedDrawer { get; set; }
        public bool DoubleClickToConfirm { get; set; }
        public bool DropdownHeight { get; set; }
    }
    public class ValidateInputAttribute : MockAttribute { public ValidateInputAttribute(string condition, string message = null, InfoMessageType messageType = InfoMessageType.Error) : base() { } }
    public class MinValueAttribute : MockAttribute { public MinValueAttribute(double minValue) : base() { } }
    public class MaxValueAttribute : MockAttribute { public MaxValueAttribute(double maxValue) : base() { } }
    public class RangeAttribute : MockAttribute { public RangeAttribute(double min, double max) : base() { } } // Unity Range와 겹칠 수 있으나 Odin 네임스페이스 안에 둠
    public class PropertyRangeAttribute : MockAttribute { public PropertyRangeAttribute(double min, double max) : base() { } }
    public class FilePathAttribute : MockAttribute { public FilePathAttribute() : base() { } }
    public class FolderPathAttribute : MockAttribute { public FolderPathAttribute() : base() { } }
    public class RequiredAttribute : MockAttribute { public RequiredAttribute(string errorMessage = null, InfoMessageType messageType = InfoMessageType.Error) : base() { } }
    public class AssetsOnlyAttribute : MockAttribute { public AssetsOnlyAttribute() : base() { } }
    public class SceneObjectsOnlyAttribute : MockAttribute { public SceneObjectsOnlyAttribute() : base() { } }

    // =========================================================================
    // 6. 리스트 (Collections)
    // =========================================================================
    public class TableListAttribute : MockAttribute 
    { 
        public TableListAttribute() : base() { }
        public int NumberOfItemsPerPage { get; set; }
        public bool ShowIndexLabels { get; set; }
        public bool DrawScrollView { get; set; }
        public int MinScrollViewHeight { get; set; }
        public int MaxScrollViewHeight { get; set; }
    }
    public class ListDrawerSettingsAttribute : MockAttribute 
    { 
        public ListDrawerSettingsAttribute() : base() { }
        public bool ShowIndexLabels { get; set; }
        public string OnTitleBarGUI { get; set; }
        public string ListElementLabelName { get; set; }
    }
    
    // =========================================================================
    // 7. 직렬화 (Serialization) - 주의: 데이터 저장 방식은 흉내낼 수 없음
    // =========================================================================
    // OdinSerialize는 시각적 요소가 아니라 실제 데이터 저장 로직을 바꾸므로,
    // 이 래퍼만으로는 데이터가 날아갈 수 있습니다. 
    // 하지만 컴파일 에러를 막기 위해 추가는 해둡니다.
    public class OdinSerializeAttribute : MockAttribute { public OdinSerializeAttribute() : base() { } }
    public class SerializedMonoBehaviour : MonoBehaviour { } // 상속 에러 방지용

    // =========================================================================
    // 8. 필수 Enums (껍데기)
    // =========================================================================
    public enum ButtonSizes { Small, Medium, Large, Gigantic }
    public enum ButtonStyle { Box, Foldout }
    public enum TitleAlignments { Left, Centered, Right, Split }
    public enum InfoMessageType { None, Info, Warning, Error }
    public enum ObjectFieldAlignment { Left, Center, Right }
}
#endif