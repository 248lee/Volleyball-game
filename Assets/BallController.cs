using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BallController : MonoBehaviour
{
    [HideInInspector] public float shiftReceiveForce = 10f;
    [HideInInspector] public float shiftReceiveRadius = 2f;
    public Rigidbody2D ballRigidbody;
    private GluedBool canShiftReceive = new GluedBool();
    [HideInInspector] public float shiftReceiveFromAngle = -45f;
    [HideInInspector, Range(0f, 359f)] public float shiftRangeAngle = 90f;
    [HideInInspector] public Vector3 shiftReceiveFromVector;
    [HideInInspector] public Vector3 shiftReceiveToVector;

    private void Start()
    {
        this.shiftReceiveFromVector = Quaternion.AngleAxis(shiftReceiveFromAngle, Vector3.forward) * transform.right;
        this.shiftReceiveToVector = Quaternion.AngleAxis(shiftRangeAngle, Vector3.forward) * shiftReceiveFromVector;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canShiftReceive.value())
        {
            ballRigidbody.velocity = new Vector2(0f, 0f);
            ballRigidbody.AddForce(Vector2.up * shiftReceiveForce, ForceMode2D.Impulse);
            _ = canShiftReceive.GluedChangeValue(false, 0.2f);
        }
    }

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, ballRigidbody.transform.position);
        Vector3 ballDir = ballRigidbody.transform.position - transform.position;
        this.canShiftReceive.ChangeValue(distance <= shiftReceiveRadius && MathV.isVectorBetween(shiftReceiveFromVector, shiftReceiveToVector, ballDir));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BallController))]
public class BallControllerEditor : Editor
{
    public static bool showShiftReceiveSettings = false;
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
    }
}
#endif
