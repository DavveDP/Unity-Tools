using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DavveDP.Tools
{
    public abstract class EnumBackedDatabaseEditorWindow<TDatabase, TObject> : EditorWindow
        where TDatabase : EnumBackedDatabase<TDatabase, TObject>
        where TObject : EnumBackedDatabaseObject, new()
    {
        protected SerializedObject _databaseObject;

        protected string selectedPropertyPath = string.Empty;
        protected SerializedProperty selectedProperty;

        protected TDatabase _database;
        public Action<TDatabase> DatabaseUpdated;
        /// <summary>
        /// Should be set to false if the amount of elements in <c>_database</c> or their ordering changed
        /// </summary>
        private Vector2 sidebarScrollPos = Vector2.zero;
        private Vector2 propertiesScrollPos = Vector2.zero;

        //protected static void Open()
        //{
        //    EnumBackedDatabaseEditorWindow<TDatabase, TObject> window = GetWindow<EnumBackedDatabaseEditorWindow<TDatabase, TObject>>();
        //    TDatabase database = EnumBackedDatabase<TDatabase, TObject>.Instance;
        //    window._database = database;
        //    window._databaseObject = new SerializedObject(database);
        //    window.DatabaseUpdated += (newDatabase) =>
        //    {
        //        window._databaseObject = new SerializedObject(newDatabase);
        //    };
        //}

        protected virtual void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(200));
            DrawSideBar(_databaseObject.FindProperty("_data"), 20);
            if (GUILayout.Button("Add new Entry"))
            {
                NameEntryWindow.Open((window) =>
                {
                    GUILayout.Label("Name Entry:");
                    window._text = GUILayout.TextField(window._text);
                    if (GUILayout.Button("Confirm"))
                    {
                        if (!string.IsNullOrEmpty(window._text) && Regex.IsMatch(window._text, @"^[a-zA-Z]+$"))
                        {
                            _database.e_Data.Add(new TObject() { e_Name = window._text });
                            DatabaseUpdated?.Invoke(_database);
                        }
                        window.Close();
                    }
                });
                GUIUtility.ExitGUI(); // fixes issue with closing layout groups that aren't open
            }
            GUILayout.EndVertical();
            if (selectedProperty != null)
                DrawProperties(selectedProperty, true);

            GUILayout.EndHorizontal();
            Apply();
        }

        protected void DrawSideBar(SerializedProperty prop, int buttonHeight, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            sidebarScrollPos = GUILayout.BeginScrollView(sidebarScrollPos);
            foreach (SerializedProperty p in prop)
            {
                if (GUILayout.Button(p.displayName, GUILayout.Height(buttonHeight)))
                {
                    selectedProperty = p;
                    selectedPropertyPath = p.propertyPath;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            if (!string.IsNullOrEmpty(selectedPropertyPath))
            {
                selectedProperty = _databaseObject.FindProperty(selectedPropertyPath);
            }
        }
        protected void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            GUILayout.BeginVertical();
            propertiesScrollPos = GUILayout.BeginScrollView(propertiesScrollPos);
            string lastPropPath = string.Empty;
            foreach (SerializedProperty p in prop)
            {
                if (p.isAnimated && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        Debug.Log("Drawing children");
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }


        //protected void DrawField(string propName, bool relative)
        //{
        //    if (relative && currentProperty != null)
        //    {
        //        EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propName), true);
        //    }
        //    else if (currentObject != null)
        //    {
        //        EditorGUILayout.PropertyField(currentObject.FindProperty(propName), true);
        //    }
        //}

        protected void DrawLabelStylised(string text, Color fontColor, int fontSize = 16, FontStyle fontStyle = FontStyle.Normal)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = fontColor;
            style.fontSize = fontSize;
            style.fontStyle = fontStyle;
            EditorGUILayout.LabelField(text, style);
        }

        protected void DrawModelPreviewField(SerializedProperty prop)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(prop);
            var obj = prop.objectReferenceValue;
            if (obj != null)
            {
                Texture2D texture = AssetPreview.GetAssetPreview(prop.objectReferenceValue);
                GUILayout.Label("", GUILayout.Width(100), GUILayout.Height(100));
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
            }
            GUILayout.EndHorizontal();
        }

        private void OnDestroy()
        {
            _database.GenerateEnum();
        }

        protected void Apply()
        {
            _databaseObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_database);
        }

    }
    public class NameEntryWindow : EditorWindow
    {
        public string _text;
        private Action<NameEntryWindow> GUIAction;
        public static void Open(Action<NameEntryWindow> GUIAction)
        {
            NameEntryWindow window = EditorWindow.GetWindow<NameEntryWindow>();
            window.GUIAction = GUIAction;
            window.ShowModalUtility();
        }

        private void OnGUI()
        {
            GUIAction?.Invoke(this);
        }
    }
}