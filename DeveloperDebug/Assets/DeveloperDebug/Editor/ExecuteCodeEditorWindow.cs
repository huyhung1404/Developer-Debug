using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
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
        }

        private void OnGUI()
        {
            m_SerializedObject.Update();

            EditorGUILayout.PropertyField(m_SerializedObject.FindProperty("ReferencedAssembliesPath"));

            EditorGUILayout.LabelField("Namespace", GUICustomStyle.CenteredBigLabel);
            
            m_ScrollPosLib = EditorGUILayout.BeginScrollView(m_ScrollPosLib,GUILayout.Height(50));
            var Lib = GUILayout.TextArea(m_SerializedObject.FindProperty("Namespace").stringValue,GUILayout.ExpandHeight(true));
            m_Data.Namespace = Lib;
            EditorGUILayout.EndScrollView();

            m_SerializedObject.ApplyModifiedProperties();

            EditorGUILayout.LabelField("Code", GUICustomStyle.CenteredBigLabel);

            m_ScrollPosCode = EditorGUILayout.BeginScrollView(m_ScrollPosCode,GUILayout.ExpandHeight(true));
            var code = GUILayout.TextArea(EditorPrefs.GetString("cache_code_runtime_debug", string.Empty),GUILayout.ExpandHeight(true));
            EditorPrefs.SetString("cache_code_runtime_debug",code);
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Execute"))
            {
                Execute(code);
            }
        }
        
        [Obsolete("Obsolete")]
        private void Execute(string stringCode)
        {
            // CSharpCodeProvider c = new CSharpCodeProvider();
            // ICodeCompiler icc = c.CreateCompiler();
            // CompilerParameters cp = new CompilerParameters();
            //
            // foreach (var referencedAssemblies in ((ExecuteCodeRuntimeData)m_Setting.targetObject).ReferencedAssembliesPath)
            // {
            //     cp.ReferencedAssemblies.Add(referencedAssemblies);
            // }
            //
            // cp.CompilerOptions = "/t:library";
            // cp.GenerateInMemory = true;
            //
            // StringBuilder sb = new StringBuilder("");
            // sb.Append("using System;\n" );
            // sb.Append("using UnityEngine;\n" );
            //
            // sb.Append("namespace CSCodeEvaler{ \n");
            // sb.Append("public class CSCodeEvaler{ \n");
            // sb.Append("public object EvalCode(){\n");
            // sb.Append(sCSCode+"; \n");
            // sb.Append("return null; \n");
            // sb.Append("} \n");
            // sb.Append("} \n");
            // sb.Append("}\n");
            //
            // CompilerResults cr = icc.CompileAssemblyFromSource(cp, sb.ToString());
            // if( cr.Errors.Count > 0 ){
            //     foreach (CompilerError CompErr in cr.Errors)
            //     {
            //         Debug.LogError($"Line number {CompErr.Line}, Error Number: {CompErr.ErrorNumber}, '{CompErr.ErrorText}'");
            //     }
            //     return null;
            // }
            //
            // System.Reflection.Assembly a = cr.CompiledAssembly;
            // object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");
            //
            // Type t = o.GetType();
            // MethodInfo mi = t.GetMethod("EvalCode");
            //
            // object s = mi.Invoke(o, null);
            // return s;
        }
    }
}
