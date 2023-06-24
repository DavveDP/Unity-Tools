using Mono.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DavveDP.Tools
{
    public abstract class EnumBackedDatabase<TDatabase, TObject> : SingletonScriptableObject<TDatabase>
    where TDatabase : EnumBackedDatabase<TDatabase, TObject>
    where TObject : EnumBackedDatabaseObject
    {
        [SerializeField] protected List<TObject> _data;
        public ReadOnlyCollection<TObject> Data => _data as ReadOnlyCollection<TObject>;
    #if UNITY_EDITOR
        public List<TObject> e_Data => _data;
        public abstract string EnumFilePath { get; }
        public virtual void GenerateEnum()
        {
            string path = Path.Combine(Application.dataPath, EnumFilePath);
            if (_data.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine("public enum " + Path.GetFileNameWithoutExtension(EnumFilePath) + " {");
                    for (int i = 0; i < _data.Count; i++)
                    {
                        string name = _data[i].Name;
                        if (!string.IsNullOrEmpty(name))
                            writer.WriteLine("\t" + _data[i].Name + ",");
                    }
                    writer.WriteLine("}");
                }
            }
        }
    #endif
    }
}
