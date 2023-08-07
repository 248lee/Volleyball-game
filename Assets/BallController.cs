using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BallController : MonoBehaviour
{
    
    public Rigidbody2D ballRigidbody;
    private GluedBool canShiftReceive = new GluedBool();
    private GluedBool canUpReceive = new GluedBool();
    private GluedBool canSaveReceive = new GluedBool();
    // Inspector values of ShiftReceive
    [HideInInspector] public float shiftReceiveFromAngle = -45f;
    [HideInInspector, Range(0f, 359f)] public float shiftRangeAngle = 90f;
    [HideInInspector] public float shiftReceiveForce = 10f;
    [HideInInspector] public float shiftReceiveRadius = 2f;
    [HideInInspector] public Vector3 shiftReceiveFromVector;
    [HideInInspector] public Vector3 shiftReceiveToVector;

    // Inspector values of upReceive
    [HideInInspector] public float upReceiveFromAngle = -45f;
    [HideInInspector, Range(0f, 359f)] public float upRangeAngle = 90f;
    [HideInInspector] public float upReceiveForce = 10f;
    [HideInInspector] public float upReceiveRadius = 2f;
    [HideInInspector] public Vector3 upReceiveFromVector;
    [HideInInspector] public Vector3 upReceiveToVector;
    

    private void Start()
    {
        this.shiftReceiveFromVector = Quaternion.AngleAxis(shiftReceiveFromAngle, Vector3.forward) * transform.right;
        this.shiftReceiveToVector = Quaternion.AngleAxis(shiftRangeAngle, Vector3.forward) * shiftReceiveFromVector;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canShiftReceive.value())
        {
            // start perform ShiftReceive
            this.ballRigidbody.velocity = new Vector2(0f, 0f);
            this.ballRigidbody.AddForce(Vector2.up * shiftReceiveForce, ForceMode2D.Impulse);
            _ = canShiftReceive.GluedChangeValue(false, 0.2f);
        }
        if (Input.GetMouseButtonDown(0) && canUpReceive.value())
        {
            // start perform UpReceive
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            this.ballRigidbody.velocity = new Vector2(0f, 0f);
            this.ballRigidbody.AddForce(direction * upReceiveForce, ForceMode2D.Impulse);
            _ = canUpReceive.GluedChangeValue(false, 0.2f);
        }
    }

    private void FixedUpdate()
    {
        float distancePlayer_Ball = Vector2.Distance(transform.position, ballRigidbody.transform.position);
        Vector3 directionPlayer_Ball = ballRigidbody.transform.position - transform.position;
        // Change the value: canShiftReceive
        this.canShiftReceive.ChangeValue(distancePlayer_Ball <= shiftReceiveRadius && MathV.isVectorBetween(shiftReceiveFromVector, shiftReceiveToVector, directionPlayer_Ball));
        // Change the value: canUpReceive
        this.canUpReceive.ChangeValue(distancePlayer_Ball <= upReceiveRadius && MathV.isVectorBetween(upReceiveFromVector, upReceiveToVector, directionPlayer_Ball));
    }
    public void SetCanSaveReceive(bool set)
    {
        this.canSaveReceive.ChangeValue(set);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BallController))]
public class BallControllerEditor : Editor
{
    public static bool showShiftReceiveSettings = false;
    public static bool showUpReceiveSettings = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BallController linkedObject = target as BallController;

        if (GUILayout.Button(showShiftReceiveSettings ? "Hide Shift Receive" : "Modify Shift Receive"))
        {
            showShiftReceiveSettings = !showShiftReceiveSettings;
        }

        if (showShiftReceiveSettings)
        {
            EditorGUI.BeginChangeCheck();

            float newShiftReceiveForce = EditorGUILayout.FloatField("Shift Receive Force", linkedObject.shiftReceiveForce);
            float newShiftReceiveRadius = EditorGUILayout.FloatField("Shift Receive Radius", linkedObject.shiftReceiveRadius);
            float newShiftReceiveFromAngle = EditorGUILayout.FloatField("Shift Receive From Angle", linkedObject.shiftReceiveFromAngle);
            float newShiftRangeAngle = EditorGUILayout.Slider("Shift Range Angle", linkedObject.shiftRangeAngle, 0f, 359f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Modify Shift Receive");
                linkedObject.shiftReceiveForce = newShiftReceiveForce;
                linkedObject.shiftReceiveRadius = newShiftReceiveRadius;
                linkedObject.shiftReceiveFromAngle = newShiftReceiveFromAngle;
                linkedObject.shiftRangeAngle = newShiftRangeAngle;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        if (GUILayout.Button(showUpReceiveSettings ? "Hide Up Receive" : "Modify Up Receive"))
        {
            showUpReceiveSettings = !showUpReceiveSettings;
        }

        if (showUpReceiveSettings)
        {
            EditorGUI.BeginChangeCheck();

            float newUpReceiveForce = EditorGUILayout.FloatField("Up Receive Force", linkedObject.upReceiveForce);
            float newUpReceiveRadius = EditorGUILayout.FloatField("Up Receive Radius", linkedObject.upReceiveRadius);
            float newUpReceiveFromAngle = EditorGUILayout.FloatField("Up Receive From Angle", linkedObject.upReceiveFromAngle);
            float newUpRangeAngle = EditorGUILayout.Slider("Up Range Angle", linkedObject.upRangeAngle, 0f, 359f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Modify Up Receive");
                linkedObject.upReceiveForce = newUpReceiveForce;
                linkedObject.upReceiveRadius = newUpReceiveRadius;
                linkedObject.upReceiveFromAngle = newUpReceiveFromAngle;
                linkedObject.upRangeAngle = newUpRangeAngle;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
    public void OnSceneGUI()
    {
        BallController linkedObject = target as BallController;
        if (showShiftReceiveSettings)
        {
            linkedObject.shiftReceiveFromVector = Quaternion.AngleAxis(linkedObject.shiftReceiveFromAngle, Vector3.forward) * linkedObject.transform.right;
            linkedObject.shiftReceiveToVector = Quaternion.AngleAxis(linkedObject.shiftRangeAngle, Vector3.forward) * linkedObject.shiftReceiveFromVector;

            Handles.color = new Color(1, 0, 1, 0.5f);
            Handles.DrawSolidArc(linkedObject.transform.position, new Vector3(0, 0, 1), linkedObject.shiftReceiveFromVector, linkedObject.shiftRangeAngle, linkedObject.shiftReceiveRadius);
        }
        if (showUpReceiveSettings)
        {
            linkedObject.upReceiveFromVector = Quaternion.AngleAxis(linkedObject.upReceiveFromAngle, Vector3.forward) * linkedObject.transform.right;
            linkedObject.upReceiveToVector = Quaternion.AngleAxis(linkedObject.upRangeAngle, Vector3.forward) * linkedObject.upReceiveFromVector;

            Handles.color = new Color(0.5f, 1f, 0.5f, 0.5f);
            Handles.DrawSolidArc(linkedObject.transform.position, new Vector3(0, 0, 1), linkedObject.upReceiveFromVector, linkedObject.upRangeAngle, linkedObject.upReceiveRadius);
        }
    }
}
#endif
