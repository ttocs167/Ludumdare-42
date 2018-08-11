using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockBehaviour : MonoBehaviour {
    public class block
    {
        public string BlockType;
        public Vector3 Speed;
        public bool Triggereder;
        public block(string bT) {
            switch (bT)
            {
                case "Standard":
                    Speed = new Vector3(0,0,0);
                    Triggereder = false;
                    break;
                case "Elevator":
                    Speed = new Vector3(0, 1, 0);
                    Triggereder = false;
                    break;
                default:
                    Speed = new Vector3(0, 0, 0);
                    Triggereder = false;
                    break;
            }

        }
    }
    public string blockType;
    public block ablock;
    private bool triggered;
	// Use this for initialization
	void Start () {
        ablock = new block(blockType);
        triggered = false;
        Debug.Log("START");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (triggered || ablock.Triggereder == false)
        {
            //change this to move to empty markers
            this.transform.position = this.transform.position + ablock.Speed/100;
        }	
	}

    void playerCollision()
    {
        Debug.Log("Poop");
        triggered = true;
    }
}
