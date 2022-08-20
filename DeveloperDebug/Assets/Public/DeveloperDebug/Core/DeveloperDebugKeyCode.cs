#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
namespace DeveloperDebug.Core
{
    using UnityEngine;
    using System.Text;
    using System;
    using System.Collections.Generic;

    public class DeveloperDebugKeyCode : MonoBehaviour
    {
        #region Core

        private static bool m_Enable;
        private static float m_WaitingTimeForEachPress;
        private static Dictionary<string, Action> m_KeyCodeData;
        private static int m_MinLengthKeyCode;

        private float m_WaitTimeKeyCode;
        private StringBuilder m_CurrentInputKeyCode;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var _setting = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting");
            m_WaitingTimeForEachPress = _setting.waitingTimeForEachPress;
            m_KeyCodeData = _setting.GetKeyCodeData();
            m_MinLengthKeyCode = _setting.useDefaultTouchCodeForKeyCode
                ? Math.Min(_setting.minLengthTouchCode, _setting.minLengthKeyCode)
                : _setting.minLengthKeyCode;
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
            var _inputString = Input.inputString;
            if (string.IsNullOrEmpty(_inputString)) return;
            m_WaitTimeKeyCode = m_WaitingTimeForEachPress;
            m_CurrentInputKeyCode.Append(_inputString);
        }

        private void EndCheckingKeyCode()
        {
            m_WaitTimeKeyCode = 0;
            if (m_CurrentInputKeyCode.Length < m_MinLengthKeyCode)
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

        public static void Register(string key, Action action)
        {
            if (key.Length < m_MinLengthKeyCode)
            {
                Debug.LogError("Invalid length");
                return;
            }

            if (m_KeyCodeData.ContainsKey(key))
            {
                Debug.LogError("Key already exists");
                return;
            }
            m_KeyCodeData.Add(key, action);
            m_Enable = m_KeyCodeData.Count > 0;
        }

        public static void Unregister(string key)
        {
            if(key.Length < m_MinLengthKeyCode) return;
            if (!m_KeyCodeData.ContainsKey(key)) return;
            m_KeyCodeData.Remove(key);
            m_Enable = m_KeyCodeData.Count > 0;
        }

        #endregion
    }
}
#endif