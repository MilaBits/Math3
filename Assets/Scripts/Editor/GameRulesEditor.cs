//using UnityEditor;
//using UnityEngine;
//
//
//[CustomEditor(typeof(GameRules))]
//public class GameRulesEditor : Editor {
//    
//    public override void OnInspectorGUI() {
//        DrawDefaultInspector();
//
//        GameRules gameRules = (GameRules) target;
//        if (GUILayout.Button("Re-generate values")) {
//            gameRules.NextAnswer();
//        }
//    }
//}