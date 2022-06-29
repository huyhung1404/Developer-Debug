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
        private static float m_LongestTimeHoldingTouch;

        private int m_LastTouchCount;
        private bool m_DebugMode;
        private float m_TimeCheckTouch;
        private float m_TimeHolding;
        private readonly int[] m_TempInput = new int[4];
        private string m_InputCode;
        private bool m_WaitCheckingDebugMode;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            var _setting = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting");
            m_TouchCodeData = _setting.GetTouchData();
            m_NumberOfTouchesRequiredToEnterDebugMode = _setting.numberOfTouchesRequiredToEnterDebugMode;
            m_LongestTimeWaitingForNextTouchCheck = _setting.longestTimeWaitingForNextTouchCheck;
            m_LongestTimeHoldingTouch = _setting.longestTimeHoldingTouch;
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
                        AddCode(_touchCount);
                        EndCheckTouchCode();
                        return;
                    }
                }
                
                if (m_LastTouchCount == _touchCount) return;
                
                if (m_LastTouchCount == 0)
                {
                    m_TimeHolding = 0;
                    m_TempInput[0] = m_TempInput[1] = m_TempInput[2] = m_TempInput[3] = 0;
                }else if (_touchCount == 0)
                {
                    m_TimeHolding = -1;
                    m_TimeCheckTouch = 0;
                    AddCode(GetCodeInput());
                }
                m_LastTouchCount = _touchCount;
                return;
            }

            if (!m_WaitCheckingDebugMode || _touchCount != 0) return;
            StartCheckDebug();
        }

        private int GetCodeInput()
        {
            var _max = 0;
            for (var i = 1; i < 4; i++)
            {
                if (m_TempInput[i] <= m_TempInput[_max]) continue;
                _max = i;
            }
            return _max;
        }

        private void StartCheckDebug()
        {
            m_WaitCheckingDebugMode = false;
            m_TimeCheckTouch = m_LastTouchCount = 0;
            m_TimeHolding = -1;
            m_InputCode = "";
        }

        private void AddCode(int touchCount)
        {
            m_InputCode += touchCount.ToString();
        }

        private void EndCheckTouchCode()
        {
            m_DebugMode = false;
            Debug.LogError(m_InputCode);
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