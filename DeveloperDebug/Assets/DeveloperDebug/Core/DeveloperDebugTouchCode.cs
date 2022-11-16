#if ((DEVELOPER_DEBUG && UNITY_ANDROID) || (DEVELOPER_DEBUG && UNITY_IOS)) && !UNITY_EDITOR
namespace DeveloperDebug.Core
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class DeveloperDebugTouchCode : MonoBehaviour
    {
        #region Core

        private static bool m_Enable;
        private static Dictionary<int, Action> m_TouchCodeData;
        private static int m_NumberOfTouchesRequiredToEnterDebugMode;
        private static float m_LongestTimeWaitingForNextTouchCheck;
        private static float m_LongestTimeHoldingTouch;
        private static int m_MinValueTouchCode;

        private int m_LastTouchCount;
        private bool m_DebugMode;
        private float m_TimeCheckTouch;
        private float m_TimeHolding;
        private readonly int[] m_TempInput = new int[4];
        private int m_InputCode;
        private bool m_WaitCheckingDebugMode;

        public static void Initialization()
        {
            var _setting = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting");
            m_TouchCodeData = _setting.GetTouchCodeData();
            m_NumberOfTouchesRequiredToEnterDebugMode = _setting.numberOfTouchesRequiredToEnterDebugMode;
            m_LongestTimeWaitingForNextTouchCheck = _setting.longestTimeWaitingForNextTouchCheck;
            m_LongestTimeHoldingTouch = _setting.longestTimeHoldingTouch;
            m_MinValueTouchCode = (int) Math.Pow(10, _setting.minLengthTouchCode - 1);
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
                m_WaitCheckingDebugMode = m_DebugMode = true;
                return;
            }

            if (!m_WaitCheckingDebugMode)
            {
                if (_touchCount > 4)
                {
                    EndCheckTouchCode();
                    return;
                }

                var _time = Time.deltaTime;
                m_TimeCheckTouch += _time;
                if (m_TimeCheckTouch >= m_LongestTimeWaitingForNextTouchCheck)
                {
                    EndCheckTouchCode();
                    return;
                }

                if (m_TimeHolding >= 0)
                {
                    m_TimeHolding += _time;
                    if (_touchCount != 0) m_TempInput[_touchCount - 1]++;
                    if (m_TimeHolding >= m_LongestTimeHoldingTouch)
                    {
                        m_InputCode = m_InputCode * 10 + _touchCount;
                        EndCheckTouchCode();
                        return;
                    }
                }

                if (m_LastTouchCount == _touchCount) return;

                if (m_LastTouchCount == 0)
                {
                    m_TimeHolding = m_TempInput[0] = m_TempInput[1] = m_TempInput[2] = m_TempInput[3] = 0;
                }
                else if (_touchCount == 0)
                {
                    m_TimeHolding = -1;
                    m_TimeCheckTouch = 0;
                    m_InputCode = m_InputCode * 10 + GetCodeInput();
                }

                m_LastTouchCount = _touchCount;
                return;
            }

            if (!m_WaitCheckingDebugMode || _touchCount != 0) return;
            m_WaitCheckingDebugMode = false;
            m_TimeCheckTouch = m_LastTouchCount = m_InputCode = 0;
            m_TimeHolding = -1;
        }

        private int GetCodeInput()
        {
            var _max = 0;
            for (var i = 1; i < 4; i++)
            {
                if (m_TempInput[i] <= m_TempInput[_max]) continue;
                _max = i;
            }

            return _max + 1;
        }

        private void EndCheckTouchCode()
        {
            m_DebugMode = false;
            if (m_InputCode < m_MinValueTouchCode) return;
            if (!m_TouchCodeData.ContainsKey(m_InputCode)) return;
            m_TouchCodeData[m_InputCode].Invoke();
        }

        #endregion

        #region Public Method

        public static void Register(int key, Action action)
        {
            if (key < m_MinValueTouchCode)
            {
                Debug.LogError("Invalid length");
                return;
            }

            if (m_TouchCodeData.ContainsKey(key))
            {
                Debug.LogError("Key already exists");
                return;
            }
            m_TouchCodeData.Add(key, action);
            m_Enable = m_TouchCodeData.Count > 0;
        }

        public static void Unregister(int key)
        {
            if (key < m_MinValueTouchCode) return;
            if (!m_TouchCodeData.ContainsKey(key)) return;
            m_TouchCodeData.Remove(key);
            m_Enable = m_TouchCodeData.Count > 0;
        }

        #endregion
    }
}
#endif