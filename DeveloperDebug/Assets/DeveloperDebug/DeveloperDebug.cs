using System;

namespace DeveloperDebug
{
#if UNITY_EDITOR || DEVELOPER_DEBUG
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using System.Linq;
    public static class DebugConfig
    {
        //Key Code Config
        public const float WAITING_TIME_FOR_EACH_PRESS = 1;
    
        //Touch Config
        public const float HOLD_TIME_TO_ENTER_DEBUG_MODE = 2f;
        public const float TIME_EACH_CHECKING = 0.2f;
    }

    public static class KeyCodeDebug
    {
        private static readonly Dictionary<string, Action> m_KeyCodeData;

        static KeyCodeDebug()
        {
            m_KeyCodeData = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting").GetKeyCodeData();
        }

        public static bool CheckKeyCodeActive()
        {
            return m_KeyCodeData.Count > 0;
        }

        public static List<string> GetKeyData()
        {
            return m_KeyCodeData.Select(pair => pair.Key).ToList();
        }

        public static Dictionary<string, Action> GetData()
        {
            return m_KeyCodeData;
        }

        public static void CheckStringInput(string input)
        {
            if (!m_KeyCodeData.ContainsKey(input)) return;
            m_KeyCodeData[input].Invoke();
        }

        public static void Register(string key, Action action)
        {
            if (key.Length < 5)
            {
                Debug.Log("The key is case sensitive and at least 5 characters");
                return;
            }

            if (m_KeyCodeData.ContainsKey(key))
            {
                Debug.Log("Key already exists");
                return;
            }

            m_KeyCodeData.Add(key, action);
            DeveloperDebug.CheckKeyCodeDebug = CheckKeyCodeActive();
        }

        public static void Unregister(string key)
        {
            if (!m_KeyCodeData.ContainsKey(key)) return;
            m_KeyCodeData.Remove(key);
            DeveloperDebug.CheckKeyCodeDebug = CheckKeyCodeActive();
        }
    }

    public static class TouchDebug
    {
        private static readonly Dictionary<string, Action> m_TouchData;

        static TouchDebug()
        {
            m_TouchData = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting").GetTouchData();
        }
        
        private static readonly StringBuilder m_InputString = new StringBuilder();
        private static readonly Regex m_KeyRegex = new Regex(@"^[UDRL]*$");

        public static bool CheckTouchActive()
        {
            return m_TouchData.Count > 0;
        }
    
        public static List<string> GetTouchData()
        {
            return m_TouchData.Select(pair => pair.Key).ToList();
        }
    
        public static Dictionary<string, Action> GetData()
        {
            return m_TouchData;
        }

        public static void CheckTouchInfo(List<Vector2> touchInfo)
        {
            var lastChar = '@';
            foreach (var touchData in touchInfo)
            {
                if (touchData == Vector2.zero)
                {
                    lastChar = '@';
                    continue;
                }

                HandleSwipeDirection(touchData.x, touchData.y, out var tempChar);
                if (lastChar.Equals(tempChar)) continue;
                lastChar = tempChar;
                m_InputString.Append(tempChar);
            }

            if (m_TouchData.ContainsKey(m_InputString.ToString()))
            {
                m_TouchData[m_InputString.ToString()].Invoke();
            }

            m_InputString.Clear();
        }

        private static void HandleSwipeDirection(float x, float y, out char tempChar)
        {
            if (Math.Abs(x) - Math.Abs(y) > 0)
            {
                tempChar = x >= 0 ? 'R' : 'L';
                return;
            }

            tempChar = y >= 0 ? 'U' : 'D';
        }

        public static void Register(string key, Action action)
        {
            if (!m_KeyRegex.IsMatch(key))
            {
                Debug.Log("Invalid data type. Only U D L R");
                return;
            }

            if (m_TouchData.ContainsKey(key))
            {
                Debug.Log("Key already exists");
                return;
            }

            m_TouchData.Add(key, action);
            DeveloperDebug.CheckTouchDebug = CheckTouchActive();
        }

        public static void Unregister(string key)
        {
            if (!m_TouchData.ContainsKey(key)) return;
            m_TouchData.Remove(key);
            DeveloperDebug.CheckTouchDebug = CheckTouchActive();
        }
    }

    public class DeveloperDebug : MonoBehaviour
    {
        public static bool CheckKeyCodeDebug;
        public static bool CheckTouchDebug;

        private float m_WaitTimeKeyCode;
        private StringBuilder m_CurrentInputKeyCode;

        private List<Vector2> m_CurrentDisplacementTouchInfo;
        private float m_TimeTouch = -1;
        private float m_LastCheckingTime;
        private float m_MinDistanceCheckTouch;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var instance = new GameObject().AddComponent<DeveloperDebug>();
            instance.name = instance.GetType().Name;
        }

        private void Awake()
        {
            m_CurrentInputKeyCode = new StringBuilder();
            m_CurrentDisplacementTouchInfo = new List<Vector2>();
            m_MinDistanceCheckTouch = Math.Min(Screen.width, Screen.height) / 5f;
            CheckKeyCodeDebug = KeyCodeDebug.CheckKeyCodeActive();
            CheckTouchDebug = TouchDebug.CheckTouchActive();
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
#if !UNITY_ANDROID && !UNITY_IOS || UNITY_EDITOR
            if (CheckKeyCodeDebug)
            {
                HandleCheckKeyCode();
            }
#endif
            if (CheckTouchDebug)
            {
                HandleCheckTouch();
            }
        }

        #region Check Key Code

        private void HandleCheckKeyCode()
        {
            if (m_WaitTimeKeyCode <= 0)
            {
                if (!Input.anyKeyDown) return;
                CheckingKeycode();
                m_WaitTimeKeyCode = DebugConfig.WAITING_TIME_FOR_EACH_PRESS;
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

        private void EndCheckingKeyCode()
        {
            m_WaitTimeKeyCode = 0;
            if (m_CurrentInputKeyCode.Length >= 5)
            {
                KeyCodeDebug.CheckStringInput(m_CurrentInputKeyCode.ToString());
            }
            m_CurrentInputKeyCode.Clear();
        }

        private void CheckingKeycode()
        {
            var inputString = Input.inputString;
            if (string.IsNullOrEmpty(inputString)) return;
            m_WaitTimeKeyCode = DebugConfig.WAITING_TIME_FOR_EACH_PRESS;
            m_CurrentInputKeyCode.Append(inputString);
        }

        #endregion

        #region Check Touch

        private void HandleCheckTouch()
        {
            if (m_TimeTouch < 0)
            {
                if (!Input.GetMouseButtonDown(0)) return;
                StartCheckTouchInput();
                return;
            }

            m_TimeTouch += Time.deltaTime;

            if (m_TimeTouch - m_LastCheckingTime >= DebugConfig.TIME_EACH_CHECKING)
            {
                CheckingTouch();
                m_LastCheckingTime = m_TimeTouch;
            }

            if (!Input.GetMouseButtonUp(0)) return;
            EndCheckTouchInput();
        }

        private void StartCheckTouchInput()
        {
            m_LastCheckingTime = DebugConfig.HOLD_TIME_TO_ENTER_DEBUG_MODE - DebugConfig.TIME_EACH_CHECKING;
            m_LastInputPosition = Input.mousePosition;
            m_TimeTouch = 0;
        }

        private void EndCheckTouchInput()
        {
            m_TimeTouch = -1;
            if (m_CurrentDisplacementTouchInfo.Count <= 0) return;
            TouchDebug.CheckTouchInfo(m_CurrentDisplacementTouchInfo);
            m_CurrentDisplacementTouchInfo.Clear();
        }

        private Vector2 m_LastInputPosition;
        private bool m_TouchWait;

        private void CheckingTouch()
        {
            Vector2 mousePosition = Input.mousePosition;
            var displacementPosition = mousePosition - m_LastInputPosition;
            if (Math.Abs(displacementPosition.x) < m_MinDistanceCheckTouch &&
                Math.Abs(displacementPosition.y) < m_MinDistanceCheckTouch)
            {
                if (m_TouchWait) return;
                m_CurrentDisplacementTouchInfo.Add(Vector2.zero);
                m_TouchWait = true;
                return;
            }

            m_CurrentDisplacementTouchInfo.Add(displacementPosition);
            m_LastInputPosition = mousePosition;
            m_TouchWait = false;
        }

        #endregion
    }

#endif

    public static class DeveloperDebugExtension
    {
        /// <summary>
        /// Register key code debug
        /// </summary>
        /// <param name="key">The key is case sensitive and at least 5 characters</param>
        /// <param name="action">Executable code</param>
        /// <param name="runOnEditorOnly">Only running in Unity Editor</param>
        public static void RegisterKeyCode(string key, Action action, bool runOnEditorOnly)
        {
            if (runOnEditorOnly)
            {
#if UNITY_EDITOR
                KeyCodeDebug.Register(key, action);
#endif
                return;
            }

#if UNITY_EDITOR || DEVELOPER_DEBUG
            KeyCodeDebug.Register(key, action);
#endif
        }

        public static void UnregisterKeyCode(string key, bool runOnEditorOnly)
        {
            if (runOnEditorOnly)
            {
#if UNITY_EDITOR
                KeyCodeDebug.Unregister(key);
#endif
                return;
            }
#if UNITY_EDITOR || DEVELOPER_DEBUG
            KeyCodeDebug.Unregister(key);
#endif
        }

        /// <summary>
        /// Register touch debug
        /// </summary>
        /// <param name="key">0nly 4 characters are valid 'U' | 'D' | 'L' | 'R'  means swipe 'UP' | 'DOWN' | 'LEFT' |'RIGHT' characters are capital letters</param>
        /// <param name="action">Executable code</param>
        /// /// <param name="runOnEditorOnly">Only running in Unity Editor</param>
        public static void RegisterTouchDebug(string key, Action action, bool runOnEditorOnly)
        {
            if (runOnEditorOnly)
            {
#if UNITY_EDITOR
                TouchDebug.Register(key.ToUpper(), action);
#endif
                return;
            }

#if UNITY_EDITOR || DEVELOPER_DEBUG
            TouchDebug.Register(key.ToUpper(), action);
#endif
        }

        public static void UnregisterTouchDebug(string key, bool runOnEditorOnly)
        {
            if (runOnEditorOnly)
            {
#if UNITY_EDITOR
                TouchDebug.Unregister(key.ToUpper());
#endif
                return;
            }

#if UNITY_EDITOR || DEVELOPER_DEBUG
            TouchDebug.Unregister(key.ToUpper());
#endif
        }
    }
}