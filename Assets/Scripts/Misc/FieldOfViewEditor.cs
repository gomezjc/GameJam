using System;
using Characters;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Misc
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fieldOfView = (FieldOfView) target;
            Handles.color = Color.white;
            Handles.DrawWireDisc(fieldOfView.transform.position, fieldOfView.transform.forward,
                fieldOfView.viewRadius);
            Vector3 viewAngleA = fieldOfView.DirFromAngle(-fieldOfView.viewAngle / 2,false);
            Vector3 viewAngleB = fieldOfView.DirFromAngle(fieldOfView.viewAngle / 2,false);
            
            Vector3 position = new Vector3(fieldOfView.transform.position.x,fieldOfView.transform.position.y,0);
            
            Handles.DrawLine(position,position + viewAngleA * fieldOfView.viewRadius);
            Handles.DrawLine(position,position + viewAngleB * fieldOfView.viewRadius);
            
            foreach (Transform visibleTarget in fieldOfView.visibleTargets) {
                Handles.DrawLine (fieldOfView.transform.position, visibleTarget.position);
            }
        }
    }
}