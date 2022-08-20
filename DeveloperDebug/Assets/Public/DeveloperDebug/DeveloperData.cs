#if UNITY_EDITOR || DEVELOPER_DEBUG
namespace DeveloperDebug
{
    using UnityEngine;
    public static class DeveloperData
    {
        public static void DebugFunc1()
        {
            Debug.LogError("Func1");
        }
        
        public static void DebugFunc2()
        {
            Debug.LogError("Func2");
        }
        
        public static void DebugFunc3()
        {
            Debug.LogError("Func3");
        }
    }
}
#endif