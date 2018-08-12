using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deletebLOCK : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="DeathBlock")
        {
            Debug.Log("Die Die Die");
            Destroy(other.transform.parent.gameObject);
            
        }
    }
    // Update is called once per frame
    void Update () {
		
	}

    void killChildren(GameObject obj)
    {
        //
    }
}
