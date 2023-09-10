using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BallActionController : MonoBehaviour
{
    public Rigidbody2D ballRigidbody;
    public DirectionIndicator killDirectionIndicator;
    private GluedBool canShiftReceive = new GluedBool();
    private GluedBool canUpReceive = new GluedBool();
    private GluedBool canSaveReceive = new GluedBool();
    private GluedBool canKill = new GluedBool();
    private GluedBool killSignal = new GluedBool();

    /*Here we explain what does the below values mean. The first 5 variables, which are Center, FromAngle, RangeAngle, Force, and Radius, are tunable in the inspector.
      The last 2 variables are vectors calculated from the FromAngle and the RangeAngle, and are used in below algorithms to determine whether such ball action can be performed.*/

    // Inspector values of ShiftReceive
    [HideInInspector] public Vector2 shiftReceiveCenterOffset = Vector2.zero;
    [HideInInspector] public float shiftReceiveFromAngle = -45f;
    [HideInInspector, Range(0f, 359f)] public float shiftRangeAngle = 90f;
    [HideInInspector] public float shiftReceiveForce = 10f;
    [HideInInspector] public float shiftReceiveRadius = 2f;
    [HideInInspector] public Vector3 shiftReceiveFromVector;
    [HideInInspector] public Vector3 shiftReceiveToVector;

    // Inspector values of upReceive
    [HideInInspector] public Vector2 upReceiveCenterOffset = Vector2.zero;
    [HideInInspector] public float upReceiveFromAngle = -45f;
    [HideInInspector, Range(0f, 359f)] public float upRangeAngle = 90f;
    [HideInInspector] public float upReceiveForce = 10f;
    [HideInInspector] public float upReceiveRadius = 2f;
    [HideInInspector] public Vector3 upReceiveFromVector;
    [HideInInspector] public Vector3 upReceiveToVector;

    // Inspector values of SaveReceive
    [HideInInspector] public Vector2 saveReceiveCenterOffset = Vector2.zero;
    [HideInInspector] public float saveReceiveFromAngle = -45f;
    [HideInInspector, Range(0f, 359f)] public float saveRangeAngle = 90f;
    [HideInInspector] public float saveReceiveForce = 10f;
    [HideInInspector] public float saveReceiveRadius = 2f;
    [HideInInspector] public Vector3 saveReceiveFromVector;
    [HideInInspector] public Vector3 saveReceiveToVector;

    // Inspector values of kill
    [HideInInspector] public Vector2 killCenterOffset = Vector2.zero;
    [HideInInspector] public float killFromAngle = -45f;
    [HideInInspector, Range(0f, 359f)] public float killRangeAngle = 90f;
    [HideInInspector] public float killForce = 10f;
    [HideInInspector] public float killRadius = 2f;
    [HideInInspector] public Vector3 killFromVector;
    [HideInInspector] public Vector3 killToVector;
    [HideInInspector] public float minimumForceMultiplier = .2f;
    [HideInInspector] public float killY_limit = .8f;

    private void Start()
    {
        this.killSignal.ChangeValue(false);
    }

    private void Update()
    {
        // Calculating vectors from the FromAngle and the RangeAngle
        float dir = transform.right.x;
        this.shiftReceiveFromVector = Quaternion.AngleAxis(shiftReceiveFromAngle, Vector3.forward * dir) * transform.right;
        this.shiftReceiveToVector = Quaternion.AngleAxis(shiftRangeAngle, Vector3.forward * dir) * shiftReceiveFromVector;
        this.upReceiveFromVector = Quaternion.AngleAxis(upReceiveFromAngle, Vector3.forward * dir) * transform.right;
        this.upReceiveToVector = Quaternion.AngleAxis(upRangeAngle, Vector3.forward * dir) * upReceiveFromVector;
        this.saveReceiveFromVector = Quaternion.AngleAxis(saveReceiveFromAngle, Vector3.forward * dir) * transform.right;
        this.saveReceiveToVector = Quaternion.AngleAxis(saveRangeAngle, Vector3.forward * dir) * saveReceiveFromVector;
        this.killFromVector = Quaternion.AngleAxis(killFromAngle, Vector3.forward * dir) * transform.right;
        this.killToVector = Quaternion.AngleAxis(killRangeAngle, Vector3.forward * dir) * killFromVector;

        if (canShiftReceive.value())
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                // start perform ShiftReceive
                this.ballRigidbody.velocity = new Vector2(0f, 0f);
                this.ballRigidbody.AddForce(Vector2.up * shiftReceiveForce, ForceMode2D.Impulse);
                _ = canShiftReceive.GluedChangeValue(false, 0.4f);
            }
        }
        if (canUpReceive.value())
        {
            if (Input.GetMouseButtonDown(0))
            {
                // start perform UpReceive
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
                this.ballRigidbody.velocity = new Vector2(0f, 0f);
                this.ballRigidbody.AddForce(direction * upReceiveForce, ForceMode2D.Impulse);
                _ = canUpReceive.GluedChangeValue(false, 0.4f);
            }
        }
        if (canSaveReceive.value())
        {
            if (GetComponent<PlayerController>().GetIsSaveReceiving())
            {
                // start perform SaveReceive
                this.ballRigidbody.velocity = new Vector2(0f, 0f);
                this.ballRigidbody.AddForce(Vector2.up * saveReceiveForce, ForceMode2D.Impulse);
                _ = canSaveReceive.GluedChangeValue(false, 0.4f);
            }
        }
        if (canKill.value())
        {
            // Draw the kill direction indicator
            this.killDirectionIndicator.SetFromOffset(killCenterOffset);
            this.killDirectionIndicator.gameObject.SetActive(true);
            // Modify the from point of the mouse-direction indicator to match the kill direction indicator.
            GameObject.FindGameObjectWithTag("Indicator_Mouse").GetComponent<SegmentIndicatorMouse>().SetFromOffset(killCenterOffset);
            if (this.killSignal.value()) // if the kill signal comes
            {
                _ = this.killSignal.GluedChangeValue(false, .3f);
                // start perform Kill
                this.ballRigidbody.GetComponent<BallController>().ResumeBall();
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 directionPlayer_Mouse = (mousePos - (Vector2)transform.position).normalized;
                Vector2 directionPlayer_Ball = ballRigidbody.transform.position - (transform.position + (Vector3)(killCenterOffset));
                float cos = MathV.cos(directionPlayer_Ball, directionPlayer_Mouse);
                float forceMultiplier;
                if (0.86f < cos && cos <= 1f) // if the degrees is between 0 ~ 30
                    forceMultiplier = 1f;
                else if (0.76f < cos && cos <= 0.86f) // if the degrees is between 30 ~ 40
                    forceMultiplier = 0.8f;
                else if (0.65f < cos && cos <= 0.76f) // if the degrees is between 40 ~ 50
                    forceMultiplier = 0.6f;
                else // if the degrees is between 50 ~ 180
                    forceMultiplier = 0.2f;
                    forceMultiplier = forceMultiplier > 0.1f ? forceMultiplier : 0.1f; // Filter out the too-small-multiplier.
                this.ballRigidbody.velocity = new Vector2(0f, 0f);
                this.ballRigidbody.AddForce(directionPlayer_Mouse * killForce * forceMultiplier, ForceMode2D.Impulse);
                _ = canKill.GluedChangeValue(false, 0.4f);
                // Resume the mouse-direction indicator
                GameObject.FindGameObjectWithTag("Indicator_Mouse").GetComponent<SegmentIndicatorMouse>().SetFromOffset(Vector2.zero);
            }
        }
        else
        {
            // Erase the kill direction indicator
            this.killDirectionIndicator.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        float distancePlayer_Ball = Vector2.Distance(transform.position, ballRigidbody.transform.position);
        Vector3 directionPlayer_Ball = ballRigidbody.transform.position - transform.position;

        Vector3 directionShiftCenter_Ball = directionPlayer_Ball - (Vector3)(this.shiftReceiveCenterOffset);
        float distanceShiftCenter_Ball = directionShiftCenter_Ball.magnitude;

        Vector3 directionUpCenter_Ball = directionPlayer_Ball - (Vector3)(this.upReceiveCenterOffset);
        float distanceUpCenter_Ball = directionUpCenter_Ball.magnitude;

        Vector3 directionSaveCenter_Ball = directionPlayer_Ball - (Vector3)(this.saveReceiveCenterOffset);
        float distanceSaveCenter_Ball = directionSaveCenter_Ball.magnitude;

        Vector3 directionKillCenter_Ball = directionPlayer_Ball - (Vector3)(this.killCenterOffset);
        float distanceKillCenter_Ball = directionKillCenter_Ball.magnitude;

        // Change the value: canShiftReceive
        this.canShiftReceive.ChangeValue(distanceShiftCenter_Ball <= shiftReceiveRadius && MathV.isVectorBetween(shiftReceiveFromVector, shiftReceiveToVector, directionShiftCenter_Ball));
        // Change the value: canUpReceive
        this.canUpReceive.ChangeValue(distanceUpCenter_Ball <= upReceiveRadius && MathV.isVectorBetween(upReceiveFromVector, upReceiveToVector, directionUpCenter_Ball));
        // Change the value: canSaveReceive
        this.canSaveReceive.ChangeValue(distanceSaveCenter_Ball <= saveReceiveRadius && MathV.isVectorBetween(saveReceiveFromVector, saveReceiveToVector, directionSaveCenter_Ball));
        // Change the value: canKill
        this.canKill.ChangeValue(transform.position.y > killY_limit && distanceKillCenter_Ball <= killRadius && MathV.isVectorBetween(killFromVector, killToVector, directionKillCenter_Ball));
    }
    public bool GetCanKill()
    {
        return this.canKill.value();
    }
    public void StartKill()
    {
        this.killSignal.ChangeValue(true);
    }
    public void StuckTheBall()
    {
        this.ballRigidbody.GetComponent<BallController>().PauseBall();
    }
    public void ResumeTheBall()
    {
        this.ballRigidbody.GetComponent<BallController>().ResumeBall();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BallActionController))]
public class BallActionControllerEditor : Editor
{
    public static bool showShiftReceiveSettings = false;
    public static bool showUpReceiveSettings = false;
    public static bool showSaveReceiveSettings = false;
    public static bool showKillSettings = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BallActionController linkedObject = target as BallActionController;

        // Show / Hide settings of Shift Receive 
        if (GUILayout.Button(showShiftReceiveSettings ? "Hide Shift Receive" : "Modify Shift Receive"))
        {
            showShiftReceiveSettings = !showShiftReceiveSettings;
        }
        if (showShiftReceiveSettings)
        {
            EditorGUI.BeginChangeCheck();

            Vector2 newShiftReceiveCenterOffset = EditorGUILayout.Vector2Field("Shift Receive Center Offset", linkedObject.shiftReceiveCenterOffset);
            float newShiftReceiveForce = EditorGUILayout.FloatField("Shift Receive Force", linkedObject.shiftReceiveForce);
            float newShiftReceiveRadius = EditorGUILayout.FloatField("Shift Receive Radius", linkedObject.shiftReceiveRadius);
            float newShiftReceiveFromAngle = EditorGUILayout.FloatField("Shift Receive From Angle", linkedObject.shiftReceiveFromAngle);
            float newShiftRangeAngle = EditorGUILayout.Slider("Shift Range Angle", linkedObject.shiftRangeAngle, 0f, 359f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Modify Shift Receive");
                linkedObject.shiftReceiveCenterOffset = newShiftReceiveCenterOffset;
                linkedObject.shiftReceiveForce = newShiftReceiveForce;
                linkedObject.shiftReceiveRadius = newShiftReceiveRadius;
                linkedObject.shiftReceiveFromAngle = newShiftReceiveFromAngle;
                linkedObject.shiftRangeAngle = newShiftRangeAngle;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        // Show / Hide settings of Up Receive 
        if (GUILayout.Button(showUpReceiveSettings ? "Hide Up Receive" : "Modify Up Receive"))
        {
            showUpReceiveSettings = !showUpReceiveSettings;
        }
        if (showUpReceiveSettings)
        {
            EditorGUI.BeginChangeCheck();

            Vector2 newUpReceiveCenterOffset = EditorGUILayout.Vector2Field("Up Receive Center Offset", linkedObject.upReceiveCenterOffset);
            float newUpReceiveForce = EditorGUILayout.FloatField("Up Receive Force", linkedObject.upReceiveForce);
            float newUpReceiveRadius = EditorGUILayout.FloatField("Up Receive Radius", linkedObject.upReceiveRadius);
            float newUpReceiveFromAngle = EditorGUILayout.FloatField("Up Receive From Angle", linkedObject.upReceiveFromAngle);
            float newUpRangeAngle = EditorGUILayout.Slider("Up Range Angle", linkedObject.upRangeAngle, 0f, 359f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Modify Up Receive");
                linkedObject.upReceiveCenterOffset = newUpReceiveCenterOffset;
                linkedObject.upReceiveForce = newUpReceiveForce;
                linkedObject.upReceiveRadius = newUpReceiveRadius;
                linkedObject.upReceiveFromAngle = newUpReceiveFromAngle;
                linkedObject.upRangeAngle = newUpRangeAngle;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        // Show / Hide settings of Save Receive 
        if (GUILayout.Button(showSaveReceiveSettings ? "Hide Save Receive" : "Modify Save Receive"))
        {
            showSaveReceiveSettings = !showSaveReceiveSettings;
        }
        if (showSaveReceiveSettings)
        {
            EditorGUI.BeginChangeCheck();

            Vector2 newSaveReceiveCenterOffset = EditorGUILayout.Vector2Field("Save Receive Center Offset", linkedObject.saveReceiveCenterOffset);
            float newSaveReceiveForce = EditorGUILayout.FloatField("Save Receive Force", linkedObject.saveReceiveForce);
            float newSaveReceiveRadius = EditorGUILayout.FloatField("Save Receive Radius", linkedObject.saveReceiveRadius);
            float newSaveReceiveFromAngle = EditorGUILayout.FloatField("Save Receive From Angle", linkedObject.saveReceiveFromAngle);
            float newSaveRangeAngle = EditorGUILayout.Slider("Save Range Angle", linkedObject.saveRangeAngle, 0f, 359f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Modify Up Receive");
                linkedObject.saveReceiveCenterOffset = newSaveReceiveCenterOffset;
                linkedObject.saveReceiveForce = newSaveReceiveForce;
                linkedObject.saveReceiveRadius = newSaveReceiveRadius;
                linkedObject.saveReceiveFromAngle = newSaveReceiveFromAngle;
                linkedObject.saveRangeAngle = newSaveRangeAngle;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        // Show / Hide settings of Kill
        if (GUILayout.Button(showKillSettings ? "Hide Kill" : "Modify Kill"))
        {
            showKillSettings = !showKillSettings;
        }
        if (showKillSettings)
        {
            EditorGUI.BeginChangeCheck();

            Vector2 newKillCenterOffset = EditorGUILayout.Vector2Field("Kill Center Offset", linkedObject.killCenterOffset);
            float newKillForce = EditorGUILayout.FloatField("Kill Force", linkedObject.killForce);
            float newKillRadius = EditorGUILayout.FloatField("Kill Radius", linkedObject.killRadius);
            float newKillFromAngle = EditorGUILayout.FloatField("Kill From Angle", linkedObject.killFromAngle);
            float newKillRangeAngle = EditorGUILayout.Slider("Kill Range Angle", linkedObject.killRangeAngle, 0f, 359f);
            float newMinForceMultiplier = EditorGUILayout.FloatField("Minimum Force Multiplier", linkedObject.minimumForceMultiplier);
            float newKillY_limit = EditorGUILayout.FloatField("Kill Y_limit", linkedObject.killY_limit);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Modify Kill");
                linkedObject.killCenterOffset = newKillCenterOffset;
                linkedObject.killForce = newKillForce;
                linkedObject.killRadius = newKillRadius;
                linkedObject.killFromAngle = newKillFromAngle;
                linkedObject.killRangeAngle = newKillRangeAngle;
                linkedObject.minimumForceMultiplier = newMinForceMultiplier;
                linkedObject.killY_limit = newKillY_limit;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
    public void OnSceneGUI()
    {
        BallActionController linkedObject = target as BallActionController;
        float dir = linkedObject.transform.right.x;
        Vector3 playerPos = linkedObject.transform.position;
        if (showShiftReceiveSettings)
        {
            linkedObject.shiftReceiveFromVector = Quaternion.AngleAxis(linkedObject.shiftReceiveFromAngle, Vector3.forward * dir) * linkedObject.transform.right;
            linkedObject.shiftReceiveToVector = Quaternion.AngleAxis(linkedObject.shiftRangeAngle, Vector3.forward * dir) * linkedObject.shiftReceiveFromVector;

            Handles.color = new Color(1, 0, 1, 0.5f);
            Handles.DrawSolidArc(playerPos + (Vector3)(linkedObject.shiftReceiveCenterOffset), new Vector3(0, 0, 1), linkedObject.shiftReceiveFromVector, linkedObject.shiftRangeAngle * dir, linkedObject.shiftReceiveRadius);
        }
        if (showUpReceiveSettings)
        {
            linkedObject.upReceiveFromVector = Quaternion.AngleAxis(linkedObject.upReceiveFromAngle, Vector3.forward * dir) * linkedObject.transform.right;
            linkedObject.upReceiveToVector = Quaternion.AngleAxis(linkedObject.upRangeAngle, Vector3.forward * dir) * linkedObject.upReceiveFromVector;

            Handles.color = new Color(0.5f, 1f, 0.5f, 0.5f);
            Handles.DrawSolidArc(playerPos + (Vector3)(linkedObject.upReceiveCenterOffset), new Vector3(0, 0, 1), linkedObject.upReceiveFromVector, linkedObject.upRangeAngle * dir, linkedObject.upReceiveRadius);
        }
        if (showSaveReceiveSettings)
        {
            linkedObject.saveReceiveFromVector = Quaternion.AngleAxis(linkedObject.saveReceiveFromAngle, Vector3.forward * dir) * linkedObject.transform.right;
            linkedObject.saveReceiveToVector = Quaternion.AngleAxis(linkedObject.saveRangeAngle, Vector3.forward * dir) * linkedObject.saveReceiveFromVector;

            Handles.color = new Color(0.75f, 0.91f, 0f, 0.5f);
            Handles.DrawSolidArc(playerPos + (Vector3)(linkedObject.saveReceiveCenterOffset), new Vector3(0, 0, 1), linkedObject.saveReceiveFromVector, linkedObject.saveRangeAngle * dir, linkedObject.saveReceiveRadius);
        }
        if (showKillSettings)
        {
            linkedObject.killFromVector = Quaternion.AngleAxis(linkedObject.killFromAngle, Vector3.forward * dir) * linkedObject.transform.right;
            linkedObject.killToVector = Quaternion.AngleAxis(linkedObject.killRangeAngle, Vector3.forward * dir) * linkedObject.killFromVector;

            Handles.color = new Color(0f, 0.91f, 0.75f, 0.5f);
            Handles.DrawSolidArc(playerPos + (Vector3)(linkedObject.killCenterOffset), new Vector3(0, 0, 1), linkedObject.killFromVector, linkedObject.killRangeAngle * dir, linkedObject.killRadius);
            Handles.DrawLine(new Vector3(-100, linkedObject.killY_limit, 0), new Vector3(100, linkedObject.killY_limit, 0));
        }
    }
}
#endif
