using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour {

    [Range(0f, 1f)]
    public float smoothTime;
    public Vector3 cameraTarget = new Vector3 (-0.7f, 2.3f, 4.2f);

    private GameObject player;
    private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 target = player.transform.position + cameraTarget;

        this.transform.position = Vector3.SmoothDamp(this.transform.position, target, ref velocity, smoothTime);

	}
}
