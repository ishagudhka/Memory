using UnityEditor;
using UnityEngine;

namespace Memory.Sound.Editor
{
    [CustomPropertyDrawer(typeof(GameSoundAttribute))]
    public class GameSoundDrawer : PropertyDrawer
    {
        private bool needInit = true;
        private int index = -1;
        private string[] clips;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (needInit)
            {                
                var gs = GameObject.FindObjectOfType<GameSound>();
                if (gs == null) return;

                clips = gs.GetClips();
                if (clips == null) return;
                needInit = false;

                index = 0;
                string val = property.stringValue;
                for (int i = 0; i < clips.Length; i++)
                {
                    if (clips[i] == val)
                    {
                        index = i;
                        break;                        
                    }
                }
            }

            if (index < 0) return;

            EditorGUI.BeginProperty(position, label, property);
            if (property.propertyType == SerializedPropertyType.String)
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                index = EditorGUI.Popup(position, index, clips);
                property.stringValue = clips[index];
            }
            EditorGUI.EndProperty();
        }
    }
}