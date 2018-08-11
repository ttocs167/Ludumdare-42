using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oceanMovement : MonoBehaviour {
    [Range(0, 5f)]
    public float waveStrength;
    [Range(0, 0.1f)]
    public float waveSpeed;
    public float planeSpeed;
    public float planeAccel;
    public GameObject managerObject;
    public bool rising;

    private Mesh mesh;
    private Vector3[] vertices;
    private List<float> offsets = new List<float>();
    private List<bool> flowDirection = new List<bool>();
    private float initial;
    private gameManagement gameManager;
    private bool started;

	// Use this for initialization
	void Start () {
        rising = true;
        gameManager = managerObject.GetComponent<gameManagement>();
        mesh = this.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        initial = vertices[0].y;

        for (int i = 0; i < vertices.Length; i++)
        {
            offsets.Add(Random.value * waveStrength);
            flowDirection.Add(Random.value > 0.5f);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        started = gameManager.started;
        mesh = this.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (flowDirection[i])
            {
                vertices[i].y += waveSpeed * waveStrength;
            }
            else
            {
                vertices[i].y -= waveSpeed * waveStrength;
            }
            if (flowDirection[i] && vertices[i].y > initial + offsets[i])
            {
                flowDirection[i] = !flowDirection[i];
            }
            if (!flowDirection[i] && vertices[i].y < initial - offsets[i])
            {
                flowDirection[i] = !flowDirection[i];
            }
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        //Rising water
        if (started)
        {
            if (rising)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + planeSpeed / 100, this.transform.position.z);
                planeSpeed = planeSpeed + planeAccel;
            }
        }
    }
}
