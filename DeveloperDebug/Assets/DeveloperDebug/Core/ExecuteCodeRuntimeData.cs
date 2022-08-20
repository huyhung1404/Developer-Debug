using System.Collections.Generic;
using UnityEngine;

namespace DeveloperDebug.Core
{
    public class ExecuteCodeRuntimeData : ScriptableObject
    {
        public List<string> ReferencedAssembliesPath;
        [TextArea] public string Namespace;
    }
}
