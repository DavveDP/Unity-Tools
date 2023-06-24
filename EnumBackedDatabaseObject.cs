using UnityEngine;

namespace DavveDP.Tools
{
    [System.Serializable]
    public class EnumBackedDatabaseObject
    {
        [SerializeField] protected string name;
        public string Name => new System.Globalization.CultureInfo("en-US").TextInfo.ToTitleCase(name);
    #if UNITY_EDITOR
        public string e_Name { set { name = value; } }
        public EnumBackedDatabaseObject() { }
    #endif
    }
}
