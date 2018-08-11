﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockBehaviour : MonoBehaviour {
    public GameObject[] path;
    private int pathCounter;
    public float Speed;
    private bool triggered;
    public bool trigger;
    public bool repeat;
    private Vector3 direction;
    public bool retrigger;
    // Use this for initialization
    void Start()
    {
        pathCounter = 0;
        triggered = false;
        if (path.Length != 0)
        {
            direction = (path[pathCounter].transform.position - this.transform.position) / (path[pathCounter].transform.position - this.transform.position).magnitude;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path.Length != 0)
        {
            if (triggered || trigger == false)
            {
                if (Vector3.Distance(path[pathCounter].transform.position, this.transform.position) < 2 * Speed * Time.fixedDeltaTime)
                {
                    pathCounter++;
                    if (pathCounter >= path.Length)
                    {
                        pathCounter = 0;
                        if (retrigger)
                        {
                            triggered = false;
                        }
                        else { 
                            if (!repeat)
                            {
                                Destroy(this);
                            }
                        }
                    }
                    direction = (path[pathCounter].transform.position - this.transform.position) / (path[pathCounter].transform.position - this.transform.position).magnitude;
                }
                this.transform.position += direction * Speed * Time.fixedDeltaTime;
            }
        }
    }

    void playerCollision()
    {
        triggered = true;
    }
}
