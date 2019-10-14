using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[CreateAssetMenu(fileName = "New AIcuñaMap", menuName = "AIcuña Map", order = 51)]
public class AIcuñaMap : ScriptableObject
{
    [SerializeField]
    string name;
    //[SerializeField]
    //List<BaseNode> nodes;

}
