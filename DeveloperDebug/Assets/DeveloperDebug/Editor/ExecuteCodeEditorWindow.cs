using System;
using System.CodeDom.Compiler;
using System.Text;
using DeveloperDebug.Core;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

namespace DeveloperDebug.Editor
{
    public class ExecuteCodeEditorWindow : EditorWindow
    {
        private static ExecuteCodeEditorWindow m_Window;
        private SerializedObject m_SerializedObject;
        private ExecuteCodeRuntimeData m_Data;
        private Vector2 m_ScrollPosLib;
        private Vector2 m_ScrollPosCode;
        private StringBuilder m_CodeExecute;

        [MenuItem("Window/Developer Debug/Execute Code Runtime")]
        public static void OpenPopupWindow()
        {
            m_Window = GetWindow<ExecuteCodeEditorWindow>("Execute Code Runtime");
            m_Window.Init();
        }

        private void Init()
        {
            m_Data = Resources.Load<ExecuteCodeRuntimeData>("ExecuteCodeRuntimeSetting");
            m_SerializedObject = new SerializedObject(m_Data);
            m_CodeExecute = new StringBuilder();
        }

        [Obsolete("Obsolete")]
        private void OnGUI()
        {
            m_SerializedObject.Update();

            EditorGUILayout.PropertyField(m_SerializedObject.FindProperty("ReferencedAssembliesPath"));

            EditorGUILayout.LabelField("Namespace", GUICustomStyle.CenteredBigLabel);

            m_ScrollPosLib = EditorGUILayout.BeginScrollView(m_ScrollPosLib, GUILayout.Height(50));
            var Lib = GUILayout.TextArea(m_SerializedObject.FindProperty("Namespace").stringValue,
                GUILayout.ExpandHeight(true));
            m_Data.Namespace = Lib;
            EditorGUILayout.EndScrollView();

            m_SerializedObject.ApplyModifiedProperties();

            EditorGUILayout.LabelField("Code", GUICustomStyle.CenteredBigLabel);

            m_ScrollPosCode = EditorGUILayout.BeginScrollView(m_ScrollPosCode, GUILayout.ExpandHeight(true));
            var code = GUILayout.TextArea(EditorPrefs.GetString("cache_code_runtime_debug", string.Empty),
                GUILayout.ExpandHeight(true));
            EditorPrefs.SetString("cache_code_runtime_debug", code);
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Execute"))
            {
                Execute(code);
            }
        }

        [Obsolete("Obsolete")]
        private void Execute(string stringCode)
        {
            var cpCompilerParameters = new CompilerParameters();

            foreach (var referencedAssemblies in m_Data.ReferencedAssembliesPath)
            {
                cpCompilerParameters.ReferencedAssemblies.Add(referencedAssemblies);
            }

            cpCompilerParameters.CompilerOptions = "/t:library";
            cpCompilerParameters.GenerateInMemory = true;

            m_CodeExecute.Clear();
            m_CodeExecute.Append(m_Data.Namespace);
            m_CodeExecute.Append(
                "namespace DeveloperDebug.Editor{ public class ExecuteDebugCodeRuntime{ public void ExecuteFunc(){");
            m_CodeExecute.Append(stringCode);
            m_CodeExecute.Append("}}}");

            var compilerResults = new CSharpCodeProvider().CreateCompiler()
                .CompileAssemblyFromSource(cpCompilerParameters, m_CodeExecute.ToString());
            if (compilerResults.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in compilerResults.Errors)
                {
                    Debug.LogError(
                        $"Line number {CompErr.Line}, Error Number: {CompErr.ErrorNumber}, '{CompErr.ErrorText}'");
                }

                return;
            }

            var instance =
                compilerResults.CompiledAssembly.CreateInstance("DeveloperDebug.Editor.ExecuteDebugCodeRuntime");
            instance?.GetType().GetMethod("ExecuteFunc")?.Invoke(instance, null);
        }
    }
}