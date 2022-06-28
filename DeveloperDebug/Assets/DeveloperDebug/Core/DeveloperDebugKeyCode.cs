#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
namespace DeveloperDebug.Core
{
    using UnityEngine;
    using System.Text;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DeveloperDebugKeyCode : MonoBehaviour
    {
        #region Core

        private static bool m_Enable;
        private static float m_WaitingTimeForEachPress;
        private static Dictionary<string, Action> m_KeyCodeData;

        private float m_WaitTimeKeyCode;
        private StringBuilder m_CurrentInputKeyCode;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            var _setting = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting");
            m_WaitingTimeForEachPress = _setting.waitingTimeForEachPress;
            m_KeyCodeData = _setting.GetKeyCodeData();
            var _instance = new GameObject().AddComponent<DeveloperDebugKeyCode>();
            _instance.name = _instance.GetType().Name;
        }

        private void Awake()
        {
            m_CurrentInputKeyCode = new StringBuilder();
            m_Enable = m_KeyCodeData.Count > 0;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!m_Enable) return;
            if (m_WaitTimeKeyCode <= 0)
            {
                if (!Input.anyKeyDown) return;
                CheckingKeycode();
                m_WaitTimeKeyCode = m_WaitingTimeForEachPress;
                return;
            }

            m_WaitTimeKeyCode -= Time.deltaTime;
            if (m_WaitTimeKeyCode <= 0)
            {
                EndCheckingKeyCode();
                return;
            }

            CheckingKeycode();
        }

        private void CheckingKeycode()
        {
            var inputString = Input.inputString;
            if (string.IsNullOrEmpty(inputString)) return;
            m_WaitTimeKeyCode = m_WaitingTimeForEachPress;
            m_CurrentInputKeyCode.Append(inputString);
        }

        private void EndCheckingKeyCode()
        {
            m_WaitTimeKeyCode = 0;
            if (m_CurrentInputKeyCode.Length < 5)
            {
                m_CurrentInputKeyCode.Clear();
                return;
            }

            var _input = m_CurrentInputKeyCode.ToString();
            if (m_KeyCodeData.ContainsKey(_input))
            {
                m_KeyCodeData[_input].Invoke();
            }

            m_CurrentInputKeyCode.Clear();
        }

        #endregion

        #region Public Method

        public static List<string> GetKeyData()
        {
            return m_KeyCodeData.Select(pair => pair.Key).ToList();
        }
        
        public static Dictionary<string,Action> GetData()
        {
            return m_KeyCodeData;
        }

        public static void Register(string key, Action action)
        {
            if (key.Length < 5) return;
            if (m_KeyCodeData.ContainsKey(key)) return;
            m_KeyCodeData.Add(key, action);
            m_Enable = m_KeyCodeData.Count > 0;
        }

        public static void Unregister(string key)
        {
            if (!m_KeyCodeData.ContainsKey(key)) return;
            m_KeyCodeData.Remove(key);
            m_Enable = m_KeyCodeData.Count > 0;
        }

        #endregion
    }
}
#endif