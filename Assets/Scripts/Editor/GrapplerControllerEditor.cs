using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GrapplerCharacterController))]
public class GrapplerControllerEditor : Editor
{

    protected virtual void OnSceneGUI()
    {
        GrapplerCharacterController t = (GrapplerCharacterController)target;
        for(var i = 0; i < t.movementPointsRelativeToCar.Length; i++)
        {
            var pointWorldSpace = t.car.transform.TransformPoint(t.movementPointsRelativeToCar[i]);
            var newPointWorldSpace = Handles.PositionHandle(pointWorldSpace, Quaternion.identity);
            t.movementPointsRelativeToCar[i] = t.car.transform.InverseTransformPoint(newPointWorldSpace);

            var nextPointWorldSpace = t.car.transform.TransformPoint(t.movementPointsRelativeToCar[(i+1) % t.movementPointsRelativeToCar.Length]);
            Handles.color = Color.blue;
            Handles.DrawLine(newPointWorldSpace, nextPointWorldSpace);
        }
    }
}
