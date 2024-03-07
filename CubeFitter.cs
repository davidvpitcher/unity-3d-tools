using UnityEditor;
using UnityEngine;

public class CubeFitterWindow : EditorWindow
{
    private Transform cubeToScale;
    private Transform leftNeighbor;
    private Transform rightNeighbor;
    private string selectedAxis = "x";

    [MenuItem("Tools/Cube Fitter")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CubeFitterWindow), false, "Cube Fitter");
    }
    void OnGUI()
    {
        GUILayout.Label("Fit Cube Between Two Neighbors", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set as Cube to Scale"))
        {
            cubeToScale = Selection.activeTransform;
        }
        cubeToScale = (Transform)EditorGUILayout.ObjectField(cubeToScale, typeof(Transform), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set as Left Neighbor"))
        {
            leftNeighbor = Selection.activeTransform;
        }
        leftNeighbor = (Transform)EditorGUILayout.ObjectField(leftNeighbor, typeof(Transform), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set as Right Neighbor"))
        {
            rightNeighbor = Selection.activeTransform;
        }
        rightNeighbor = (Transform)EditorGUILayout.ObjectField(rightNeighbor, typeof(Transform), true);
        GUILayout.EndHorizontal();

        selectedAxis = GUILayout.TextField(selectedAxis);

        if (GUILayout.Button("Fit Cube"))
        {
            if (cubeToScale != null && leftNeighbor != null && rightNeighbor != null)
            {
                FitCube(selectedAxis);
            }
            else
            {
                Debug.LogError("Please select the cube to scale and its neighbors.");
            }
        }
    }

    private void FitCube(string axis)
    {
        // Calculate the half-extents of the neighbor cubes
        float leftExtent = GetNeighborHalfExtent(leftNeighbor, axis);
        float rightExtent = GetNeighborHalfExtent(rightNeighbor, axis);

        // Calculate the world space positions of the closest edges
        float leftNeighborClosestEdge = leftNeighbor.position[GetAxisIndex(axis)] + (leftNeighbor.position[GetAxisIndex(axis)] < cubeToScale.position[GetAxisIndex(axis)] ? leftExtent : -leftExtent);
        float rightNeighborClosestEdge = rightNeighbor.position[GetAxisIndex(axis)] + (rightNeighbor.position[GetAxisIndex(axis)] > cubeToScale.position[GetAxisIndex(axis)] ? -rightExtent : rightExtent);

        // Calculate the space to fill, which is the distance between the closest edges of the neighbors
        float spaceToFill = Mathf.Abs(rightNeighborClosestEdge - leftNeighborClosestEdge);

        // Set the new scale of cubeToScale to exactly spaceToFill
        Vector3 newScale = cubeToScale.localScale;
        newScale[GetAxisIndex(axis)] = spaceToFill;

        // Apply the new scale
        Undo.RecordObject(cubeToScale, "Fit Cube");
        cubeToScale.localScale = newScale;

        // Calculate the midpoint between the closest edges of the neighbors
        float midpoint = (leftNeighborClosestEdge + rightNeighborClosestEdge) / 2;

        // Calculate the new position for cubeToScale in world space
        Vector3 newPosition = cubeToScale.position;
        newPosition[GetAxisIndex(axis)] = midpoint;

        // Apply the new position
        cubeToScale.position = newPosition;

        // Debug information for new position and scale
        Debug.LogFormat("New Scale Value: {0}", newScale[GetAxisIndex(axis)]);
        Debug.LogFormat("Midpoint: {0}", midpoint);
        Debug.LogFormat("Left Neighbor Closest Edge: {0}, Right Neighbor Closest Edge: {1}", leftNeighborClosestEdge, rightNeighborClosestEdge);
        Debug.LogFormat("New World Position Value: {0}", newPosition);

        Debug.Log("Cube fitted between neighbors.");
    }

    // The rest of the methods remain unchanged.


    private float GetNeighborHalfExtent(Transform neighbor, string axis)
    {
        // Get the half-size extent of the neighbor along the specified axis
        Bounds bounds = neighbor.GetComponent<Renderer>().bounds;
        return bounds.extents[GetAxisIndex(axis)];
    }
    private float GetDistanceBetweenNeighbors(string axis)
    {
        return Mathf.Abs(rightNeighbor.position[GetAxisIndex(axis)] - leftNeighbor.position[GetAxisIndex(axis)]);
    }

    private float GetNeighborEdgeDistance(Transform neighbor, string axis, bool isLeftNeighbor)
    {
        Bounds bounds = neighbor.GetComponent<Renderer>().bounds;
        return isLeftNeighbor ? bounds.extents[GetAxisIndex(axis)] : bounds.extents[GetAxisIndex(axis)];
    }

    private int GetAxisIndex(string axis)
    {
        switch (axis)
        {
            case "x": return 0;
            case "y": return 1;
            case "z": return 2;
            default:
                Debug.LogError("Invalid axis selected. Defaulting to 'x' axis.");
                return 0;
        }
    }
}
