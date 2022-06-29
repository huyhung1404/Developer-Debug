#if ((DEVELOPER_DEBUG && UNITY_ANDROID) || (DEVELOPER_DEBUG && UNITY_IOS)) //&& !UNITY_EDITOR
namespace DeveloperDebug.Core
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class DeveloperDebugTouchCode : MonoBehaviour
    {
        #region Core

        private enum StateDebug
        {
            CheckDebug,
            DebugMode
        }
        
        private static bool m_Enable;
        private static Dictionary<int, Action> m_TouchCodeData;
        private static int m_NumberOfTouchesRequiredToEnterDebugMode;
        private static float m_LongestTimeWaitingForNextTouchCheck;

        private int m_LastTouchCount;
        private StateDebug m_State;
        private bool m_DebugMode;
        private float m_TimeCheckTouch;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            var _setting = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting");
            m_TouchCodeData = _setting.GetTouchData();
            m_NumberOfTouchesRequiredToEnterDebugMode = _setting.numberOfTouchesRequiredToEnterDebugMode;
            m_LongestTimeWaitingForNextTouchCheck = _setting.longestTimeWaitingForNextTouchCheck;
            var _instance = new GameObject().AddComponent<DeveloperDebugTouchCode>();
            _instance.name = _instance.GetType().Name;
        }

        private void Awake()
        {
            m_Enable = m_TouchCodeData.Count > 0;
            DontDestroyOnLoad(gameObject);
        }


        private void Update()
        {
            if (!m_Enable) return;
            var _touchCount = Input.touchCount;

            if (!m_DebugMode)
            {
                if (_touchCount != m_NumberOfTouchesRequiredToEnterDebugMode) return;
                m_State = StateDebug.CheckDebug;
                m_DebugMode = true;
                return;
            }

            switch (m_State)
            {
                case StateDebug.CheckDebug:
                    if (_touchCount == 0)
                    {
                        m_State = StateDebug.DebugMode;
                        m_TimeCheckTouch = 0;
                        m_LastTouchCount = 0;
                        return;
                    }
                    m_DebugMode = _touchCount == m_NumberOfTouchesRequiredToEnterDebugMode;
                    return;
                case StateDebug.DebugMode:
                    var _time = Time.deltaTime;
                    m_TimeCheckTouch += _time;
                    if (m_TimeCheckTouch >= m_LongestTimeWaitingForNextTouchCheck)
                    {
                        EndCheckTouchCode();
                        m_DebugMode = false;
                        return;
                    }

                    if (m_LastTouchCount != _touchCount)
                    {
                        if (m_LastTouchCount == 0)
                        {
                            
                        }   
                        
                        m_LastTouchCount = _touchCount;
                    }
                    return;
            }
        }
        
        

        private void EndCheckTouchCode()
        {
            
        }

        #endregion

        #region Public Method
        
        private static readonly Regex m_KeyRegex = new Regex(@"^[1234]*$");

        public static void Register(int key, Action action)
        {
            if (!m_KeyRegex.IsMatch(key.ToString())) return;
            if (m_TouchCodeData.ContainsKey(key)) return;
            m_TouchCodeData.Add(key, action);
            m_Enable = m_TouchCodeData.Count > 0;
        }

        public static void Unregister(int key)
        {
            if (!m_TouchCodeData.ContainsKey(key)) return;
            m_TouchCodeData.Remove(key);
            m_Enable = m_TouchCodeData.Count > 0;
        }

        #endregion
    }
}
#endif