using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject followingPlayer;
    public float horizontalOffset = 10f;
    private static bool isFollowingPlayer = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (isFollowingPlayer)
        {
            Vector3 targetPos = new Vector3(followingPlayer.transform.position.x + horizontalOffset, transform.position.y, transform.position.z);
            this.transform.position = Vector3.Lerp(transform.position, targetPos, .4f);
        }
        
    }
    public static void SetFollowingPlayer(bool set)
    {
        isFollowingPlayer = set;
    }
    
}
