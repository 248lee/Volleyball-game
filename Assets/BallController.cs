using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BallController : MonoBehaviour
{
    public float shiftReceiveForce = 10f;
    public float shiftReceiveRadius = 2f;
    public Rigidbody2D ballRigidbody;
    private bool canShiftReceive = false;
    public float shiftReceiveFromAngle = -45f;
    public float shiftRangeAngle = 90f;
    public Vector3 shiftReceiveFromVector;
    public Vector3 shiftReceiveToVector;
    private void Start()
    {
        this.shiftReceiveFromVector = Quaternion.AngleAxis(shiftReceiveFromAngle, Vector3.forward) * transform.right;
        this.shiftReceiveToVector = Quaternion.AngleAxis(shiftRangeAngle, Vector3.forward) * shiftReceiveFromVector;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canShiftReceive)
        {
            ballRigidbody.velocity = new Vector2(0f, 0f);
            ballRigidbody.AddForce(Vector2.up * shiftReceiveForce, ForceMode2D.Impulse);
            canShiftReceive = false;
        }
    }

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, ballRigidbody.transform.position);
        Vector3 ballDir = ballRigidbody.transform.position - transform.position;
        canShiftReceive = (distance <= shiftReceiveRadius && MathV.isVectorBetween(shiftReceiveFromVector, shiftReceiveToVector, ballDir));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BallController))]
public class BallControllerEditor : Editor
{
    private void OnSceneGUI()
    {
        BallController linkedObject = target as BallController;
        linkedObject.shiftReceiveFromVector = Quaternion.AngleAxis(linkedObject.shiftReceiveFromAngle, Vector3.forward) * linkedObject.transform.right;
        linkedObject.shiftReceiveToVector = Quaternion.AngleAxis(linkedObject.shiftRangeAngle, Vector3.forward) * linkedObject.shiftReceiveFromVector;
        Handles.color = new Color(1, 0, 1, 0.5f);
        Handles.DrawSolidArc(linkedObject.transform.position, new Vector3(0, 0, 1), linkedObject.shiftReceiveFromVector, linkedObject.shiftRangeAngle, linkedObject.shiftReceiveRadius);
        
    }
}
#endif
