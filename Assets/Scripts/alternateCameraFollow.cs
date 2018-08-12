using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alternateCameraFollow : MonoBehaviour {
    private GameObject player;
    public float moveOffsetSlow = 1f;
    public float moveOffsetFast = 2f;
    public Vector3 cameraOffset= new Vector3(0f, 2.3f, -7f);
    public Vector3 velocity = Vector3.zero;
    public float smoothTime;
    public float maxSpeed = 1f;
    private float currOffset;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        this.transform.position = player.transform.position + cameraOffset;
    }
	
	// Update is called once per frame
	void Update () {
        float xOffset;
        if (player.GetComponent<constantMovement>().isDashing)
        {
            xOffset =  moveOffsetFast*(float)(player.GetComponent<constantMovement>().direction);            
        }
        else
        {
            xOffset = moveOffsetSlow* (float)(player.GetComponent<constantMovement>().direction);            
        }
        if (Mathf.Abs(currOffset) < Mathf.Abs(xOffset))
        {
            currOffset = currOffset + (float)(player.GetComponent<constantMovement>().direction) * maxSpeed * Time.deltaTime;
        }
        else
        {
            currOffset = xOffset;
        }

        Vector3 moveOffset = new Vector3(currOffset,0,0);
        this.transform.forward = -this.transform.position + player.transform.position;
        Vector3 target = player.transform.position + cameraOffset - moveOffset;

        this.transform.position = Vector3.SmoothDamp(this.transform.position, target, ref velocity, smoothTime);

    }
}
