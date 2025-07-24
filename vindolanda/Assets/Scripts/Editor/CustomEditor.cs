using UnityEngine;

namespace Vindolanda.Editor
{
    public class CustomEditor<T> : UnityEditor.Editor where T : Object
    {
        protected T Target => (T)target;
    }
}