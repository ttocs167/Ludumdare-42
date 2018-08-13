using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updraft : MonoBehaviour {
    [Range(0, 1f)]
    public float updraftMaxSpeed;
    [Range(0, 0.5f)]
    public float updraftForce;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<constantMovement>().externalMovements.y < updraftMaxSpeed)
            other.gameObject.GetComponent<constantMovement>().externalMovements.y += updraftForce;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<constantMovement>().externalMovements = Vector3.zero;
        }
    }
}
