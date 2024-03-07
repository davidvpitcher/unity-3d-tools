using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;

public class ObjectPositionerWindow : EditorWindow
{
    private Transform _transform;
    private GameObject _prefab;
    private Texture2D _myTexture;
    private List<Material> _materials;

    private float snapValue = 2.5f;


    private bool showMaterialTools = false;
    private bool showMoveUpDownButtons = false;
    private bool showGridSnapper = false;

    private bool showWallButtons = false;
    private bool showOldReplacementTools = false;
    private bool showSpecialMovementTool = false;

    // Assume you have a class-wide list of materials.
    private List<Material> newMaterials = new List<Material>();

    [MenuItem("Window/Object Positioner")]
    public static void ShowWindow()
    {
        GetWindow<ObjectPositionerWindow>("Object Positioner"); //
    }

    private void OnEnable()
    {
        _myTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/EditorResources/MyImage.png");
        _materials = new List<Material>();
        newMaterials = new List<Material>();

    }

    private void DebugStoredActions()
    {
        Debug.Log("Stored Actions:");
        Debug.Log("total - " + storedActions.Count);
        foreach (Action action in storedActions)
        {
            if (action == (Action)duplicateObject)
            {
                Debug.Log("duplicateObject");
            }
            if (action == (Action)duplicateObjectWithPrefab)
            {
                Debug.Log("duplicateObjectWithPrefab");
            }
            else if (action == (Action)LoadObject)
            {
                Debug.Log("LoadObject");
            }
            else if (action == (Action)materialReplacement)
            {
                Debug.Log("materialReplacement");
            }
            else if (action == (Action)RemoveCloneFromSelected)
            {
                Debug.Log("RemoveCloneFromSelected");
            }
            else if (action == (Action)clearAll)
            {
                Debug.Log("clearAll");
            }
            else if (action == (Action)rotateLeft)
            {
                Debug.Log("rotateLeft");
            }
            else if (action == (Action)rotateRight)
            {
                Debug.Log("rotateRight");
            }
            else if (action == (Action)copy1)
            {
                Debug.Log("copy1");
            }
            else if (action == (Action)copy2)
            {
                Debug.Log("copy2");
            }
            else if (action == (Action)copy3)
            {
                Debug.Log("copy3");
            }
            else if (action == (Action)addNewMaterial)
            {
                Debug.Log("addNewMaterial");
            }
            else if (action == (Action)removeMaterial)
            {
                Debug.Log("removeMaterial");
            }
            else if (action == (Action)materialReplacement)
            {
                Debug.Log("materialReplacement");
            }
            else if (action == (Action)replaceAndMatchBounds1)
            {
                Debug.Log("replaceAndMatchBounds1");
            }
            else if (action == (Action)replaceAndMatchBounds2)
            {
                Debug.Log("replaceAndMatchBounds2");
            }
            else if (action == (Action)replaceAndMatchBounds3)
            {
                Debug.Log("replaceAndMatchBounds3");
            }
            else if (action == (Action)upArrowPressed)
            {
                Debug.Log("upArrowPressed");
            }
            else if (action == (Action)downArrowPressed)
            {
                Debug.Log("downArrowPressed");
            }
            else if (action == (Action)leftArrowPressed)
            {
                Debug.Log("leftArrowPressed");
            }
            else if (action == (Action)rightArrowPressed)
            {
                Debug.Log("rightArrowPressed");
            }
            else if (action == (Action)upArrowPressedPrimary)
            {
                Debug.Log("upArrowPressedPrimary");
            }
            else if (action == (Action)downArrowPressedPrimary)
            {
                Debug.Log("downArrowPressedPrimary");
            }
            else if (action == (Action)leftArrowPressedPrimary)
            {
                Debug.Log("leftArrowPressedPrimary");
            }
            else if (action == (Action)rightArrowPressedPrimary)
            {
                Debug.Log("rightArrowPressedPrimary");
            }
            else if (action == (Action)createSouthWallProperly)
            {
                Debug.Log("createSouthWallProperly");
            }
            else if (action == (Action)createNorthWallProperly)
            {
                Debug.Log("createNorthWallProperly");
            }
            else if (action == (Action)createEastWallProperly)
            {
                Debug.Log("createEastWallProperly");
            }
            else if (action == (Action)createWestWallProperly)
            {
                Debug.Log("createWestWallProperly");
            }
            else if (action == (Action)LoadObject)
            {
                Debug.Log("LoadObject");
            }
            else if (action == (Action)moveObjectUp1)
            {
                Debug.Log("moveObjectUp1");
            }
            else if (action == (Action)moveObjectDown1)
            {
                Debug.Log("moveObjectDown1");
            }
            else if (action == (Action)moveObjectUp2)
            {
                Debug.Log("moveObjectUp2");
            }
            else if (action == (Action)moveObjectDown2)
            {
                Debug.Log("moveObjectDown2");
            }
            else if (action == (Action)snapSwap)
            {
                Debug.Log("snapSwap");
            }
        }
    }
    private void StoreLastTwoActions()
    {
        if (actionHistory.Count < 2)
        {
            Debug.LogError("Not enough actions in history to store.");
            return;
        }

        storedActions.Clear();
        foreach (Action action in actionHistory)
        {
            storedActions.Add(action);
        }

        Debug.Log("Stored last two actions.");
    }

    public void ExecuteStoredActions()
    {
        if (storedActions.Count < 2)
        {
            Debug.LogError("Not enough actions in storage to execute.");
            return;
        }

        foreach (Action action in storedActions)
        {
            action();
        }

        Debug.Log("Executed stored actions.");
    }
    private List<Action> storedActions = new List<Action>();
    private Queue<Action> actionHistory = new Queue<Action>();
    private void AddActionToHistory(Action action)
    {
        if (actionHistory.Count >= 2)
        {
            actionHistory.Dequeue(); // Ensure only the last two actions are stored
        }
        actionHistory.Enqueue(action);
    }

    /*

    private void RepeatLastTwoActions()
    {
        if (actionHistory.Count < 2)
        {
            Debug.LogError("Not enough actions in history to repeat.");
            return;
        }

        // Call the last two actions in the reverse order they were added
        for (int i = 0; i < 2; i++)
        {
            Action action = actionHistory.Pop();
            action();
        }*/

    private void DrawMaterialListField(string label, List<Material> materials)
    {
        GUILayout.Label(label);
        for (int i = 0; i < materials.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            materials[i] = EditorGUILayout.ObjectField(materials[i], typeof(Material), true) as Material;
            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                materials.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Material", GUILayout.Width(120)))
        {
            materials.Add(null);
        }
        GUILayout.EndHorizontal();
    }

    private void LoadObject()
    {

        _transform = Selection.activeTransform;

    }

    private void addNewMaterial()
    {


        newMaterials.Add(null);
    }
    private void removeMaterial()
    {

        newMaterials.RemoveAt(newMaterials.Count - 1);
    }

    private void materialReplacement()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length > 45)
        {
            Debug.LogError("Too many objects selected. Cannot apply materials to more than 45 objects at once.");
        }
        else
        {
            foreach (GameObject obj in selectedObjects)
            {
                _transform = obj.transform;
                ReplaceMaterials();
            }
        }
    }
    private void duplicateObject()
    {
        Debug.Log("DUPLICATING");
        _transform = Selection.activeTransform;
        if (_transform != null)
        {

            Vector3 originalPosition = _transform.position;

            GameObject newObject = Instantiate(_transform.gameObject);
            Undo.RegisterCreatedObjectUndo(newObject, "Duplicate Object");

            _transform = newObject.transform;
            _transform.position = originalPosition;

            Selection.activeGameObject = newObject;
        }
        else
        {
            Debug.LogError("No object selected to duplicate.");
        }
    }
    private void duplicateObjectWithPrefab()
    {
        Debug.Log("DUPLICATING");
        _transform = Selection.activeTransform;
        if (_transform != null)
        {
            GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromOriginalSource(_transform.gameObject);

            GameObject newObject;
            Transform originalParent = _transform.parent;
            Vector3 originalPosition = _transform.position;
            Quaternion originalRotation = _transform.rotation;
            Vector3 originalLocalScale = _transform.localScale;

            if (prefabRoot != null)
            {
                // The object is a prefab instance, duplicate it while maintaining the prefab link
                newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
                Undo.RegisterCreatedObjectUndo(newObject, "Duplicate Prefab Instance");
            }
            else
            {
                // The object is not a prefab instance, duplicate it normally
                newObject = Instantiate(_transform.gameObject);
                Undo.RegisterCreatedObjectUndo(newObject, "Duplicate Object");
            }

            if (originalParent != null && newObject.transform.parent != originalParent)
            {
                // When duplicating a non-prefab object or when the parent is not part of a prefab, set the parent
                newObject.transform.SetParent(originalParent, true);
            }

            newObject.transform.position = originalPosition;
            newObject.transform.rotation = originalRotation;
            newObject.transform.localScale = originalLocalScale;
            Selection.activeGameObject = newObject;
        }
        else
        {
            Debug.LogError("No object selected to duplicate.");
        }
    }









    private void replaceAndMatchBounds1()
    {
        Transform[] transforms = Selection.transforms;
        if (transforms.Length > 0 && _prefab != null)
        {
            foreach (Transform _transform in transforms)
            {
                // Store the original local bounds, position, and rotation
                Renderer oldRenderer = _transform.GetComponent<Renderer>();
                Bounds originalBounds = oldRenderer.bounds;
                Vector3 originalPosition = _transform.position;
                Quaternion originalRotation = _transform.rotation;

                // Create the new object and register the creation for undo
                GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);
                Undo.RegisterCreatedObjectUndo(newObject, "Replace And Match Bounds");

                // Set the position and rotation of the new object to match the old one
                newObject.transform.position = originalPosition;
                newObject.transform.rotation = originalRotation;

                // Get the new object's renderer and initial local bounds
                Renderer newRenderer = newObject.GetComponent<Renderer>();
                Bounds newBounds = newRenderer.bounds;

                // Calculate the scale factors required to match the original bounds
                Vector3 scale = new Vector3(
                    newObject.transform.localScale.x * (originalBounds.size.x / newBounds.size.x),
                    newObject.transform.localScale.y * (originalBounds.size.y / newBounds.size.y),
                    newObject.transform.localScale.z * (originalBounds.size.z / newBounds.size.z)
                );

                // Apply the new scale to the new object
                newObject.transform.localScale = scale;

                // Recalculate the bounds after scaling
                newBounds = newRenderer.bounds;

                // Move the new object to match the center of the old bounds
                newObject.transform.position = originalBounds.center - newObject.transform.TransformVector(newObject.transform.InverseTransformPoint(newBounds.center));

                // Remove the old object
                Undo.DestroyObjectImmediate(_transform.gameObject);

                // If there were any materials on the old object, apply them to the new one
                if (_materials != null && _materials.Count > 0 && newRenderer != null)
                {
                    newRenderer.sharedMaterials = _materials.ToArray();
                }
            }

            // Select the new objects in the editor
            Selection.objects = transforms;
        }
        else
        {
            Debug.LogError("No objects selected to replace or no prefab selected.");
        }
    }
















    /*











    private void replaceAndMatchBounds1()
    {
        _transform = Selection.activeTransform;
        if (_transform != null && _prefab != null)
        {
            // Store the original local bounds, position, and rotation
            Renderer oldRenderer = _transform.GetComponent<Renderer>();
            Bounds originalBounds = oldRenderer.bounds;
            Vector3 originalPosition = _transform.position;
            Quaternion originalRotation = _transform.rotation;

            // Create the new object and register the creation for undo
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);
            Undo.RegisterCreatedObjectUndo(newObject, "Replace And Match Bounds");

            // Set the position and rotation of the new object to match the old one
            newObject.transform.position = originalPosition;
            newObject.transform.rotation = originalRotation;

            // Get the new object's renderer and initial local bounds
            Renderer newRenderer = newObject.GetComponent<Renderer>();
            Bounds newBounds = newRenderer.bounds;

            // Calculate the scale factors required to match the original bounds
            Vector3 scale = new Vector3(
                newObject.transform.localScale.x * (originalBounds.size.x / newBounds.size.x),
                newObject.transform.localScale.y * (originalBounds.size.y / newBounds.size.y),
                newObject.transform.localScale.z * (originalBounds.size.z / newBounds.size.z)
            );

            // Apply the new scale to the new object
            newObject.transform.localScale = scale;

            // Recalculate the bounds after scaling
            newBounds = newRenderer.bounds;

            // Move the new object to match the center of the old bounds
            newObject.transform.position = originalBounds.center - newObject.transform.TransformVector(newObject.transform.InverseTransformPoint(newBounds.center));

            // Select the new object in the editor
            Selection.activeGameObject = newObject;

            // Remove the old object
            Undo.DestroyObjectImmediate(_transform.gameObject);

            // If there were any materials on the old object, apply them to the new one
            if (_materials != null && _materials.Count > 0 && newRenderer != null)
            {
                newRenderer.sharedMaterials = _materials.ToArray();
            }

            // Update the stored transform
            _transform = newObject.transform;
        }
        else
        {
            Debug.LogError("No object selected to replace or no prefab selected.");
        }
    }
    


    */

    private void replaceAndMatchBounds2()
    {
        _transform = Selection.activeTransform;
        if (_transform != null && _prefab != null)
        {
            // Store the original local bounds, position, and rotation
            Renderer oldRenderer = _transform.GetComponent<Renderer>();
            Bounds originalBounds = oldRenderer.bounds;
            Vector3 originalLocalCenter = _transform.InverseTransformPoint(originalBounds.center);
            Vector3 originalPosition = _transform.position;
            Quaternion originalRotation = _transform.rotation;

            // Create the new object and register the creation for undo
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);
            Undo.RegisterCreatedObjectUndo(newObject, "Replace And Match Bounds");

            // Get the new object's renderer and initial local bounds
            Renderer newRenderer = newObject.GetComponent<Renderer>();
            Bounds newBounds = newRenderer.bounds;
            Vector3 newLocalCenter = newObject.transform.InverseTransformPoint(newBounds.center);

            // Calculate the scale factors required to match the original bounds
            Vector3 scale = new Vector3(
                newObject.transform.localScale.x * (originalBounds.size.x / newBounds.size.x),
                newObject.transform.localScale.y * (originalBounds.size.y / newBounds.size.y),
                newObject.transform.localScale.z * (originalBounds.size.z / newBounds.size.z)
            );

            // Apply the original rotation, and new scale to the new object
            newObject.transform.rotation = originalRotation;
            newObject.transform.localScale = scale;

            // Calculate the position offset due to the change of the pivot point after scaling
            Vector3 positionOffset = originalPosition + _transform.TransformVector(originalLocalCenter - newLocalCenter);

            // Apply the position offset to the original position
            newObject.transform.position = positionOffset;

            // Select the new object in the editor
            Selection.activeGameObject = newObject;

            // Remove the old object
            Undo.DestroyObjectImmediate(_transform.gameObject);

            // If there were any materials on the old object, apply them to the new one
            if (_materials != null && _materials.Count > 0 && newRenderer != null)
            {
                newRenderer.sharedMaterials = _materials.ToArray();
            }

            // Update the stored transform
            _transform = newObject.transform;
        }
        else
        {
            Debug.LogError("No object selected to replace or no prefab selected.");
        }
    }
    private void replaceAndMatchBounds3()
    {


        _transform = Selection.activeTransform;
        if (_transform != null && _prefab != null)
        {
            Vector3 originalPosition = _transform.position;
            Quaternion originalRotation = _transform.rotation;
            Vector3 originalScale = _transform.localScale;

            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);
            Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefab");

            newObject.transform.position = originalPosition;
            newObject.transform.rotation = originalRotation;
            newObject.transform.localScale = originalScale;

            Selection.activeGameObject = newObject;

            Undo.DestroyObjectImmediate(_transform.gameObject);
            if (_materials != null && _materials.Count > 0)
            {
                Renderer renderer = newObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterials = _materials.ToArray();
                }
            }
            _transform = newObject.transform;
        }
        else
        {
            Debug.LogError("No object selected to replace or no prefab selected.");
        }
    }


    private void upArrowPressed()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Renderer renderer = _transform.GetComponent<Renderer>();
                if (renderer == null)
                {
                    if (meshFilter != null)
                    {
                        renderer = meshFilter.GetComponent<Renderer>();
                    }
                }
                // If still no renderer, return.
                if (renderer == null)
                {
                    return;
                }

                Vector3 size2 = renderer.bounds.size;
                float maxDimension = Mathf.Max(size2.x, size2.z);
                _transform = Selection.activeTransform;
                if (_transform != null)
                {
                    Undo.RecordObject(_transform, "Move Up");
                    _transform.position += new Vector3(0, 0, isSnapping ? snapValue : maxDimension);
                }

            }
        }
    }

    private void downArrowPressed()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Renderer renderer = _transform.GetComponent<Renderer>();
                if (renderer == null)
                {
                    if (meshFilter != null)
                    {
                        renderer = meshFilter.GetComponent<Renderer>();
                    }
                }
                // If still no renderer, return.
                if (renderer == null)
                {
                    return;
                }

                Vector3 size2 = renderer.bounds.size;
                float maxDimension = Mathf.Max(size2.x, size2.z);
                _transform = Selection.activeTransform;
                if (_transform != null)
                {
                    Undo.RecordObject(_transform, "Move Down");
                    _transform.position -= new Vector3(0, 0, isSnapping ? snapValue : maxDimension);
                }
            }
        }
    }

    private void leftArrowPressed()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Renderer renderer = _transform.GetComponent<Renderer>();
                if (renderer == null)
                {
                    if (meshFilter != null)
                    {
                        renderer = meshFilter.GetComponent<Renderer>();
                    }
                }
                // If still no renderer, return.
                if (renderer == null)
                {
                    return;
                }

                Vector3 size2 = renderer.bounds.size;
                float maxDimension = Mathf.Max(size2.x, size2.z);
                _transform = Selection.activeTransform;
                if (_transform != null)
                {
                    Undo.RecordObject(_transform, "Move Left");
                    _transform.position -= new Vector3(isSnapping ? snapValue : maxDimension, 0, 0);
                }
            }
        }
    }

    private void rightArrowPressed()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Renderer renderer = _transform.GetComponent<Renderer>();
                if (renderer == null)
                {
                    if (meshFilter != null)
                    {
                        renderer = meshFilter.GetComponent<Renderer>();
                    }
                }
                // If still no renderer, return.
                if (renderer == null)
                {
                    return;
                }

                Vector3 size2 = renderer.bounds.size;
                float maxDimension = Mathf.Max(size2.x, size2.z);
                _transform = Selection.activeTransform;
                if (_transform != null)
                {
                    Undo.RecordObject(_transform, "Move Right");
                    _transform.position += new Vector3(isSnapping ? snapValue : maxDimension, 0, 0);
                }
            }
        }
    }














    private void upArrowPressedPrimary()
    {
        Debug.Log("PRIMARY UP");
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Vector3 size = meshFilter.sharedMesh.bounds.size;
                Vector3 localSize = Vector3.Scale(size, _transform.localScale);

                _transform = Selection.activeTransform;
                if (_transform != null)
                {
                    Undo.RecordObject(_transform, "Move Forward");
                    _transform.position += isSnapping ? _transform.forward * snapValue : _transform.forward * localSize.z;
                }


            }
        }
    }
    private void downArrowPressedPrimary()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Vector3 size = meshFilter.sharedMesh.bounds.size;
                Vector3 localSize = Vector3.Scale(size, _transform.localScale);


                _transform = Selection.activeTransform;
                if (_transform != null)
                {
                    Undo.RecordObject(_transform, "Move Backward");
                    _transform.position -= isSnapping ? _transform.forward * snapValue : _transform.forward * localSize.z;
                }
            }
        }
    }
    private void leftArrowPressedPrimary()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Vector3 size = meshFilter.sharedMesh.bounds.size;
                Vector3 localSize = Vector3.Scale(size, _transform.localScale);

                _transform = Selection.activeTransform;
                if (_transform != null)
                {
                    Undo.RecordObject(_transform, "Move Left");
                    _transform.position -= isSnapping ? _transform.right * snapValue : _transform.right * localSize.x;
                }


            }
        }
    }
    private void rightArrowPressedPrimary()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Vector3 size = meshFilter.sharedMesh.bounds.size;
                Vector3 localSize = Vector3.Scale(size, _transform.localScale);

                _transform = Selection.activeTransform;
                if (_transform != null)
                {
                    Undo.RecordObject(_transform, "Move Right");
                    _transform.position += isSnapping ? _transform.right * snapValue : _transform.right * localSize.x;
                }


            }

        }
    }
    private void OnGUI()
    {
        if (_myTexture != null)
        {
            GUILayout.Label(_myTexture);
        }

        EditorGUILayout.BeginVertical();

        _transform = (Transform)EditorGUILayout.ObjectField("Target Transform", _transform, typeof(Transform), true);
        _prefab = (GameObject)EditorGUILayout.ObjectField("New Prefab", _prefab, typeof(GameObject), false);
        DrawMaterialListField("Materials", _materials);



        if (GUILayout.Button("Load Selected Object"))
        {
            LoadObject();
            AddActionToHistory(LoadObject);
        }
        if (GUILayout.Button("Load Prefab & Material from Selected"))
        {

            LoadMaterialFromSelected();
            AddActionToHistory(LoadMaterialFromSelected);
        }
        GUILayout.Space(10);

        //...

        GUILayout.Label("Material Replacement Tool");
        showMaterialTools = EditorGUILayout.Foldout(showMaterialTools, "Material Tools");

        if (showMaterialTools)
        {
            // Display a list of slots for the new materials
            for (int i = 0; i < newMaterials.Count; i++)
            {
                newMaterials[i] = (Material)EditorGUILayout.ObjectField("Material " + (i + 1), newMaterials[i], typeof(Material), false);
            }

            // Button to add a new material slot
            if (GUILayout.Button("Add Material"))
            {
                addNewMaterial();
                AddActionToHistory(addNewMaterial);
            }
            if (GUILayout.Button("Remove Material"))
            {
                removeMaterial();
                AddActionToHistory(removeMaterial);
            }

            if (GUILayout.Button("Replace Materials on Selected Objects"))
            {
                materialReplacement();
                AddActionToHistory(materialReplacement);
            }
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Duplicate Object"))
        {
            duplicateObject();
            AddActionToHistory(duplicateObject);
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Duplicate Prefab"))
        {
            duplicateObjectWithPrefab();
            AddActionToHistory(duplicateObjectWithPrefab);
        }
        if (GUILayout.Button("Replace And Match Bounds"))
        {
            replaceAndMatchBounds1();
            AddActionToHistory(replaceAndMatchBounds1);

        }
        if (_transform != null)
        {



            showOldReplacementTools = EditorGUILayout.Foldout(showOldReplacementTools, "Replacement Tool Variations");

            if (showOldReplacementTools)
            {

                if (GUILayout.Button("Replace And Match Bounds Working"))
                {
                    replaceAndMatchBounds2();
                    AddActionToHistory(replaceAndMatchBounds2);
                }


                if (GUILayout.Button("Replace With Prefab"))
                {
                    replaceAndMatchBounds3();
                    AddActionToHistory(replaceAndMatchBounds3);
                }
            }
        }
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Vector3 size = meshFilter.sharedMesh.bounds.size;
                Vector3 localSize = Vector3.Scale(size, _transform.localScale);
                EditorGUILayout.LabelField($"Width: {localSize.x}");
                EditorGUILayout.LabelField($"Height: {localSize.y}");
                EditorGUILayout.LabelField($"Length: {localSize.z}");

                if (GUILayout.Button("Copy Bounds to Clipboard"))
                {
                    copy1();
                    AddActionToHistory(copy1);




                }
                if (GUILayout.Button("Copy Coordinates to Clipboard"))
                {
                    copy2();
                    AddActionToHistory(copy2);

                }
                if (GUILayout.Button("Copy GameObject Info to Clipboard"))
                {
                    copy3();
                    AddActionToHistory(copy3);

                }

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Space(30);

                if (GUILayout.Button("^", GUILayout.Width(50), GUILayout.Height(50)))
                {
                    upArrowPressedPrimary();
                    AddActionToHistory(upArrowPressedPrimary);
                }
                GUILayout.Space(30);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("<", GUILayout.Width(50), GUILayout.Height(50)))
                {
                    leftArrowPressedPrimary();
                    AddActionToHistory(leftArrowPressedPrimary);

                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(">", GUILayout.Width(50), GUILayout.Height(50)))
                {
                    rightArrowPressedPrimary();
                    AddActionToHistory(rightArrowPressedPrimary);

                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Space(30);

                if (GUILayout.Button("v", GUILayout.Width(50), GUILayout.Height(50)))
                {
                    downArrowPressedPrimary();
                    AddActionToHistory(downArrowPressedPrimary);
                }

                GUILayout.Space(30);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                showSpecialMovementTool = EditorGUILayout.Foldout(showSpecialMovementTool, "Alternative movement tool:");

                if (showSpecialMovementTool)
                {

                    Renderer renderer = _transform.GetComponent<Renderer>();
                    if (renderer == null)
                    {
                        if (meshFilter != null)
                        {
                            renderer = meshFilter.GetComponent<Renderer>();
                        }
                    }
                    // If still no renderer, return.
                    if (renderer == null)
                    {
                        return;
                    }

                    Vector3 size2 = renderer.bounds.size;
                    float maxDimension = Mathf.Max(size2.x, size2.z);
                    //  float maxDimension = Mathf.Max(size2.x, size2.y, size2.z);

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Space(30);

                    if (GUILayout.Button("^", GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        upArrowPressed();
                        AddActionToHistory(upArrowPressed);
                    }
                    GUILayout.Space(30);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("<", GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        leftArrowPressed();
                        AddActionToHistory(leftArrowPressed);
                    }


                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button(">", GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        rightArrowPressed();
                        AddActionToHistory(rightArrowPressed);
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Space(30);

                    if (GUILayout.Button("v", GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        downArrowPressed();
                        AddActionToHistory(downArrowPressed);
                    }

                    GUILayout.Space(30);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();



                }


                if (GUILayout.Button("Rotate Left"))
                {
                    rotateLeft();
                    AddActionToHistory(rotateLeft);
                }

                if (GUILayout.Button("Rotate Right"))
                {
                    rotateRight();
                    AddActionToHistory(rotateRight);
                }
                GUILayout.Space(10);

                showMoveUpDownButtons = EditorGUILayout.Foldout(showMoveUpDownButtons, "Vertical Movement");

                if (showMoveUpDownButtons)
                {


                    if (GUILayout.Button("Move Up"))
                    {
                        moveObjectUp1();
                        AddActionToHistory(moveObjectUp1);
                    }

                    if (GUILayout.Button("Move Down"))
                    {
                        moveObjectDown1();
                        AddActionToHistory(moveObjectDown1);
                    }
                    GUILayout.Space(10);

                    if (GUILayout.Button("Move Floor Up"))
                    {
                        moveObjectUp2();
                        AddActionToHistory(moveObjectUp2);
                    }

                    if (GUILayout.Button("Move Floor Down"))
                    {
                        moveObjectDown2();
                        AddActionToHistory(moveObjectDown2);
                    }
                    GUILayout.Space(10);
                    GUILayout.Space(10);
                    GUILayout.Space(10);
                }


            }
            else
            {
                EditorGUILayout.HelpBox("Selected object does not have a MeshFilter!", MessageType.Error);
            }
        }



        showWallButtons = EditorGUILayout.Foldout(showWallButtons, "Wall Creation Tools");

        if (showWallButtons)
        {

            if (GUILayout.Button("Create North Wall"))
            {

                createNorthWallProperly();
                AddActionToHistory(createNorthWallProperly);
            }

            if (GUILayout.Button("Create East Wall"))
            {

                createEastWallProperly();
                AddActionToHistory(createEastWallProperly);
            }

            if (GUILayout.Button("Create South Wall"))
            {

                createSouthWallProperly();
                AddActionToHistory(createSouthWallProperly);
            }

            if (GUILayout.Button("Create West Wall"))
            {
                createWestWallProperly();
                AddActionToHistory(createWestWallProperly);
            }
        }

        showGridSnapper = EditorGUILayout.Foldout(showGridSnapper, "Grid Snap Tools");

        if (showGridSnapper)
        {
            isSnapping = EditorGUILayout.Toggle("Snap to Grid", isSnapping);
            snapValue = EditorGUILayout.FloatField("Snap Value", snapValue);

            if (GUILayout.Button("Toggle Snap to Grid"))
            {
                snapSwap();
                AddActionToHistory(snapSwap);
            }

        }
        if (GUILayout.Button("Remove '(Clone)' From Selected"))
        {
            RemoveCloneFromSelected();
            AddActionToHistory(RemoveCloneFromSelected);
        }
        /*
        if (GUILayout.Button("Repeat Last Two Actions"))
        {
            RepeatLastTwoActions();
        }*/
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Store Last Two Actions"))
        {
            StoreLastTwoActions();
            DebugStoredActions();
        }

        if (GUILayout.Button("Execute Stored Actions"))
        {
            ExecuteStoredActions();
        }

        EditorGUILayout.EndHorizontal();
        /*
        if (GUILayout.Button("Debug Stored Actions"))
        {
        }*/
        GUILayout.Space(10);
        if (GUILayout.Button("Clear All"))
        {
            clearAll();
            AddActionToHistory(clearAll);
        }
        EditorGUILayout.EndVertical();


    } // end of ongui

    private void createSouthWallProperly()
    {
        _transform = Selection.activeTransform;
        MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            Vector3 size = meshFilter.sharedMesh.bounds.size;
            Vector3 localSize = Vector3.Scale(size, _transform.localScale);
            float width = localSize.x;

            if (_transform != null)
            {
                CreateSouthOrWestWall(new Vector3(0, 180, 0), width);
            }
            else
            {
                Debug.LogError("No object selected to wall.");
            }
        }
    }
    private void createNorthWallProperly()
    {
        _transform = Selection.activeTransform;
        if (_transform != null)
        {
            CreateWall(new Vector3(0, 0, 0));


        }
        else
        {
            Debug.LogError("No object selected to wall.");
        }
    }
    private void createEastWallProperly()
    {
        _transform = Selection.activeTransform;
        if (_transform != null)
        {
            CreateWall(new Vector3(0, 90, 0));
        }
        else
        {
            Debug.LogError("No object selected to wall.");
        }
    }
    private void createWestWallProperly()
    {
        _transform = Selection.activeTransform;
        MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            Vector3 size = meshFilter.sharedMesh.bounds.size;
            Vector3 localSize = Vector3.Scale(size, _transform.localScale);
            float width = localSize.x;

            if (_transform != null)
            {
                CreateSouthOrWestWall(new Vector3(0, -90, 0), width);
            }
            else
            {
                Debug.LogError("No object selected to wall.");
            }
        }
    }


    private void moveObjectUp1()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            Vector3 size = meshFilter.sharedMesh.bounds.size;
            Vector3 localSize = Vector3.Scale(size, _transform.localScale);
            if (meshFilter != null)
            {
                Renderer renderer = _transform.GetComponent<Renderer>();
                if (renderer == null)
                {
                    if (meshFilter != null)
                    {
                        renderer = meshFilter.GetComponent<Renderer>();
                    }
                }
                // If still no renderer, return.
                if (renderer == null)
                {
                    return;
                }

                Vector3 size2 = renderer.bounds.size;
                float maxDimension = Mathf.Max(size2.x, size2.z);

                _transform = Selection.activeTransform;
                if (_transform == null)
                {
                    return;
                }
                Undo.RecordObject(_transform, "Move Up");
                _transform.position += Vector3.up * localSize.y;
            }
        }

    }


    private void moveObjectDown1()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            Vector3 size = meshFilter.sharedMesh.bounds.size;
            Vector3 localSize = Vector3.Scale(size, _transform.localScale);
            if (meshFilter != null)
            {
                Renderer renderer = _transform.GetComponent<Renderer>();
                if (renderer == null)
                {
                    if (meshFilter != null)
                    {
                        renderer = meshFilter.GetComponent<Renderer>();
                    }
                }
                // If still no renderer, return.
                if (renderer == null)
                {
                    return;
                }

                _transform = Selection.activeTransform;
                if (_transform == null)
                {
                    return;
                }
                Undo.RecordObject(_transform, "Move Down");
                _transform.position -= Vector3.up * localSize.y;


            }
        }
    }


    private void moveObjectUp2()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            Vector3 size = meshFilter.sharedMesh.bounds.size;
            Vector3 localSize = Vector3.Scale(size, _transform.localScale);
            if (meshFilter != null)
            {
                Renderer renderer = _transform.GetComponent<Renderer>();
                if (renderer == null)
                {
                    if (meshFilter != null)
                    {
                        renderer = meshFilter.GetComponent<Renderer>();
                    }
                }
                // If still no renderer, return.
                if (renderer == null)
                {
                    return;
                }

                _transform = Selection.activeTransform;
                if (_transform == null)
                {
                    return;
                }
                Undo.RecordObject(_transform, "Move Floor Up");
                _transform.position += Vector3.up * 3f;
            }
        }

    }


    private void moveObjectDown2()
    {
        if (_transform != null)
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

            Vector3 size = meshFilter.sharedMesh.bounds.size;
            Vector3 localSize = Vector3.Scale(size, _transform.localScale);
            if (meshFilter != null)
            {
                Renderer renderer = _transform.GetComponent<Renderer>();
                if (renderer == null)
                {
                    if (meshFilter != null)
                    {
                        renderer = meshFilter.GetComponent<Renderer>();
                    }
                }
                // If still no renderer, return.
                if (renderer == null)
                {
                    return;
                }

                _transform = Selection.activeTransform;

                if (_transform == null)
                {
                    return;
                }
                Undo.RecordObject(_transform, "Move Floor Down");
                _transform.position -= Vector3.up * 3f;

            }
        }
    }


















    private void snapSwap()
    {
        isSnapping = !isSnapping; // Assuming you have a boolean called isSnapping
    }
    private void clearAll()
    {
        _transform = null;
        _prefab = null;
        _materials.Clear();
        // Clear action history and stored actions
        actionHistory.Clear();
        storedActions.Clear();
    }


    private void rotateLeft()
    {

        _transform = Selection.activeTransform;
        Undo.RecordObject(_transform, "Rotate Left");
        _transform.Rotate(0, -90, 0);
    }

    private void rotateRight()
    {
        _transform = Selection.activeTransform;
        Undo.RecordObject(_transform, "Rotate Right");
        _transform.Rotate(0, 90, 0);
    }
    private void copy1()
    {
        MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            Vector3 size = meshFilter.sharedMesh.bounds.size;
            Vector3 localSize = Vector3.Scale(size, _transform.localScale);

            EditorGUIUtility.systemCopyBuffer = $"Width: {localSize.x}, Height: {localSize.y}, Length: {localSize.z}";
        }
    }

    private void copy2()
    {

        EditorGUIUtility.systemCopyBuffer = $"X: {_transform.position.x}, Y: {_transform.position.y}, Z: {_transform.position.z}";

    }

    private void copy3()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"GameObject Name: {_transform.name}");
        sb.AppendLine($"World Position: {_transform.position}");
        sb.AppendLine($"Local Position: {_transform.localPosition}");
        sb.AppendLine($"World Rotation: {_transform.rotation.eulerAngles}");
        sb.AppendLine($"Local Rotation: {_transform.localRotation.eulerAngles}");
        sb.AppendLine($"World Scale: {_transform.lossyScale}");
        sb.AppendLine($"Local Scale: {_transform.localScale}");

        Renderer renderer = _transform.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            sb.AppendLine($"Bounds Center (World Space): {bounds.center}");
            sb.AppendLine($"Bounds Size: {bounds.size}");

            Vector3 localBoundsCenter = _transform.InverseTransformPoint(bounds.center);
            sb.AppendLine($"Bounds Center (Local Space): {localBoundsCenter}");
        }

        EditorGUIUtility.systemCopyBuffer = sb.ToString();

    }
    private void RemoveCloneFromSelected()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            if (selectedObject != null)
            {
                selectedObject.name = selectedObject.name.Replace("(Clone)", "");
            }
        }
    }
    void ReplaceMaterials()
    {
        if (newMaterials.Count > 0 && Selection.transforms.Length > 0)
        {
            foreach (Transform transform in Selection.transforms)
            {
                Renderer renderer = transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // Create a SerializedObject for the current renderer
                    SerializedObject serializedRenderer = new SerializedObject(renderer);

                    Undo.RecordObject(renderer, "Material Replacement");
                    Material[] currentMaterials = renderer.sharedMaterials;

                    // Loop through both the current and new materials lists.
                    for (int i = 0; i < currentMaterials.Length && i < newMaterials.Count; i++)
                    {
                        currentMaterials[i] = newMaterials[i];
                    }

                    renderer.sharedMaterials = currentMaterials;

                    // Apply changes to the serializedRenderer, which applies them to the actual Renderer
                    serializedRenderer.ApplyModifiedProperties();
                }
            }
        }
        else
        {
            if (newMaterials.Count == 0)
            {
                Debug.LogError("No new materials selected.");
            }
            if (Selection.transforms.Length == 0)
            {
                Debug.LogError("No objects selected.");
            }
        }
    }

    void MoveObject(Vector3 direction) // This is an example function. Replace with your own.
    {
        if (_transform != null)
        {
            Vector3 newPosition = _transform.position + direction;

            if (isSnapping)
            {
                newPosition.x = Mathf.Round(newPosition.x / snapValue) * snapValue;
                newPosition.y = Mathf.Round(newPosition.y / snapValue) * snapValue;
                newPosition.z = Mathf.Round(newPosition.z / snapValue) * snapValue;
            }

            Undo.RecordObject(_transform, "Move");
            _transform.position = newPosition;
        }
    }
    private bool isSnapping = false;
    private void LoadMaterialFromSelected()
    {
        _transform = Selection.activeTransform;
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            Renderer renderer = selectedObject.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterials != null && renderer.sharedMaterials.Length > 0)
            {
                _materials.Clear();
                _materials.AddRange(renderer.sharedMaterials);
                newMaterials.Clear();
                newMaterials.AddRange(renderer.sharedMaterials);

            }
            else
            {
                _materials.Clear();
                newMaterials.Clear();
                Debug.LogError("No materials found on the selected object or its Renderer component.");
            }

            // Locate and load matching prefab
            string prefabName;
            MeshFilter meshFilter = selectedObject.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                prefabName = meshFilter.sharedMesh.name;
            }
            else
            {
                string selectedObjectName = selectedObject.name;
                prefabName = selectedObjectName.Split(' ')[0]; // Get the name before the space
            }

            string[] prefabGUIDs = AssetDatabase.FindAssets($"t:prefab {prefabName}");
            if (prefabGUIDs.Length > 0)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUIDs[0]);
                _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }
            else
            {
                _prefab = null;
                Debug.LogError($"No prefab found with the name: {prefabName}");
            }
        }
        else
        {
            _materials.Clear();
            _prefab = null;
            Debug.LogError("No object selected to load materials from.");
        }
    }

    private void CreateSouthOrWestWall(Vector3 rotationEulers, float width)
    {
        if (_transform != null && _prefab != null)
        {
            Vector3 originalPosition = _transform.position;
            Quaternion originalRotation = Quaternion.Euler(rotationEulers);
            Vector3 originalScale = _transform.localScale;

            float vecX = originalPosition.x;
            float vecY = originalPosition.y;
            float vecZ = originalPosition.z;
            vecX -= width;
            vecZ += width;

            Vector3 newVecto = new Vector3(vecX, vecY, vecZ);
            originalPosition = newVecto;
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);
            Undo.RegisterCreatedObjectUndo(newObject, "Create Wall");

            newObject.transform.position = originalPosition;
            newObject.transform.rotation = originalRotation;
            newObject.transform.localScale = originalScale;
            if (_materials != null && _materials.Count > 0)
            {
                Renderer renderer = newObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterials = _materials.ToArray();
                }
            }
            Selection.activeGameObject = newObject;

            _transform = newObject.transform;
        }
        else
        {
            Debug.LogError("No object selected or no prefab selected.");
        }
    }
    private void CreateWall(Vector3 rotationEulers)
    {
        if (_transform != null && _prefab != null)
        {
            Vector3 originalPosition = _transform.position;
            Quaternion originalRotation = Quaternion.Euler(rotationEulers);
            Vector3 originalScale = _transform.localScale;

            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);
            Undo.RegisterCreatedObjectUndo(newObject, "Create Wall");

            newObject.transform.position = originalPosition;
            newObject.transform.rotation = originalRotation;
            newObject.transform.localScale = originalScale;
            if (_materials != null && _materials.Count > 0)
            {
                Renderer renderer = newObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterials = _materials.ToArray();
                }
            }
            Selection.activeGameObject = newObject;

            _transform = newObject.transform;
        }
        else
        {
            Debug.LogError("No object selected or no prefab selected.");
        }
    }















    // David Vere Pitcher 2024



}
