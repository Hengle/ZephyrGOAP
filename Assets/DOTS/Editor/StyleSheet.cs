using UnityEngine;

namespace DOTS.Editor
{
    [CreateAssetMenu]
    public class StyleSheet : ScriptableObject
    {
        public string Name;
        
        private static StyleSheet _styleSheet;

        private static StyleSheet styleSheet =>
            _styleSheet ? _styleSheet : _styleSheet = Resources.Load<StyleSheet>("StyleSheet");
                // UnityEditor.EditorGUIUtility.isProSkin ? "StyleSheet/StyleSheetDark" : "StyleSheet/StyleSheetLight");

        [ContextMenu("Lock")] void Lock() { hideFlags = HideFlags.NotEditable; }
        [ContextMenu("UnLock")] void UnLock() { hideFlags = HideFlags.None; }
                
        
        [UnityEditor.InitializeOnLoadMethod]
        static void Load() {
            _styleSheet = styleSheet;
        }
        
        void OnValidate() {
            hideFlags = HideFlags.NotEditable;
        }

        [System.Serializable]
        public class Icons
        {
            [Header("Fixed")]
            public Texture2D canvasIcon;
        }
        
        public Icons icons;

        public static Texture2D CanvasIcon => styleSheet.icons.canvasIcon;
    }
}
