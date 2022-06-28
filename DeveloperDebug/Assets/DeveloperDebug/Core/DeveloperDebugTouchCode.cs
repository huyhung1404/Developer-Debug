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

        private static bool m_Enable;
        private static Dictionary<int, Action> m_TouchCodeData;
        private static int m_NumberOfTouchesRequiredToEnterDebugMode;
        private static float m_LongestTimeWaitingForNextTouchCheck;
        private static float m_TimeHoldingTouch;

        private int m_LastTouchCount;
        private float m_TimeChangeTouchCount;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            var _setting = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting");
            m_TouchCodeData = _setting.GetTouchData();
            m_NumberOfTouchesRequiredToEnterDebugMode = _setting.numberOfTouchesRequiredToEnterDebugMode;
            m_LongestTimeWaitingForNextTouchCheck = _setting.longestTimeWaitingForNextTouchCheck;
            m_TimeHoldingTouch = _setting.timeHoldingTouch;
            var _instance = new GameObject().AddComponent<DeveloperDebugTouchCode>();
            _instance.name = _instance.GetType().Name;
        }

        private void Awake()
        {
            m_Enable = m_TouchCodeData.Count > 0;
            DontDestroyOnLoad(gameObject);
        }

        private string test;

        private void Update()
        {
            if (!m_Enable) return;
            var _touchCount = Input.touchCount;
            var _deltaTime = Time.deltaTime;
            // if (_touchCount == m_NumberOfTouchesRequiredToEnterDebugMode)
            // {
                
            // }
            m_TimeChangeTouchCount += _deltaTime;
            if (m_LastTouchCount != _touchCount)
            {
                m_TimeChangeTouchCount = 0;
                if (m_TimeChangeTouchCount >= m_TimeHoldingTouch)
                {
                    test += _touchCount.ToString();
                    Debug.Log(test);
                }
            }
            m_LastTouchCount = _touchCount;
        }

        private void CheckTouchCount(int lastTouchCount,int currentTouchCount)
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