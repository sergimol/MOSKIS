using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

//public class DropdownDictionaryTest : MonoBehaviour
//{
//    public string[] keys;
//    public int selectedIndex = 0;

//    private void Start()
//    {
//        keys = new string[GetComponent<TrackerConfig>().eventsTracked.Count];
//        GetComponent<TrackerConfig>().eventsTracked.Keys.CopyTo(keys, 0);
//    }
//}

//[CustomEditor(typeof(DropdownDictionaryTest))]
//public class DropdownDictionaryTestEditor : Editor
//{
//    private DropdownDictionaryTest dropdownDictionaryTest;

//    private void OnEnable()
//    {
//        dropdownDictionaryTest = (DropdownDictionaryTest)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        // Draw default fields
//        DrawDefaultInspector();

//        // Draw dropdown menu
//        dropdownDictionaryTest.selectedIndex = EditorGUILayout.Popup("Options", dropdownDictionaryTest.selectedIndex, dropdownDictionaryTest.keys);

//        serializedObject.ApplyModifiedProperties();
//    }
//}


