using System.Reflection;
using UnityEngine;

namespace DeveloperDebug.Editor
{
    using UnityEditor;
    [CustomEditor(typeof(DeveloperDebugSetting))]
    public class DeveloperDebugSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            return;
            var setting = (DeveloperDebugSetting) target;
            DrawEnableForBuild(setting);
            var methods = typeof(DeveloperData).GetMethods(BindingFlags.Static | BindingFlags.Public);
           
        }
        private void DrawEnableForBuild(DeveloperDebugSetting setting)
        {
            var lastEnable = setting.enableForBuild;
            setting.enableForBuild = EditorGUILayout.Toggle("Enable For Build", setting.enableForBuild);
            if (lastEnable != setting.enableForBuild)
            {
                
            }
        }
    }
}
