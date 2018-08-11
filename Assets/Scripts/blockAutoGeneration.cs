using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockAutoGeneration : MonoBehaviour {
    public float minJumpHeight = 4f;
    public float maxJumpHeight = 4f;
    public float bounceHeight = 4f;
    public float jumpWidthMin = 4f;
    public float jumpWidthMax = 6f;
    public float bounceWidth = 4f;
    public float iniJumpWidth = 4f;
    public float iniJumpHeight = 2f;
    public GameObject[] obj;
    public GameObject firstBlock;
    public GameObject boundary;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.transform.tag=="AutoBlock")
        {
            createBlock(25f, 12f,col.transform.position.x, col.transform.position.y);
            Destroy(col.gameObject);
        }
    }
    void createBlock(float h, float w, float oX, float oY)
    {
        float dir =Mathf.Round(Random.Range(0f, 1f))*2 - 1;
        float currH = oY;
        float currX = oX;        
        h = h + oY;        
        currH = currH + iniJumpHeight;
        currX = currX + dir*iniJumpWidth-2*dir;
        Instantiate(firstBlock, new Vector3(currX,currH,1), Quaternion.identity);
        while (currH<h)
        {            
            float dh = Mathf.Round(Random.Range(minJumpHeight+1, maxJumpHeight));
            float dx = Mathf.Round(Random.Range(jumpWidthMin, jumpWidthMax));
            if((dir>0)&&((currX+dx)>w+oX-2))
            {
                Debug.Log("HighFlip");
                if(Mathf.Abs(currX-(oX+ w-3)) >1)
                {
                    Instantiate(obj[Random.Range(0, (obj.Length - 1))], new Vector3(oX + w - 3,currH,1), Quaternion.identity);
                }
                dh = Mathf.Round(bounceHeight);
                currX = oX + w - bounceWidth;
                dir = -dir;
            }
            else if(((dir < 0) && ((currX - dx) < oX-w+2)))
            {
                Debug.Log("LowFlip");
                if (Mathf.Abs(currX - (oX - w + (dir - 1))) > 1)
                {
                    Instantiate(obj[Random.Range(0, (obj.Length - 1))], new Vector3(oX - w +2, currH, 1), Quaternion.identity);
                }
                dh = Mathf.Round(bounceHeight);
                currX = oX - w + bounceWidth +2;
                dir = -dir;
            }
            else
            {
                currX = currX + dir * (dx)+dir;      
                
                
            }
            currH = currH + dh;
                        
            Vector3 currPos = new Vector3(currX, currH, 1);
            //Debug.Log("Instantiate"+currH);
            Instantiate(obj[Random.Range(0, (obj.Length - 1))], currPos, Quaternion.identity);
            
        }
        GameObject BlockLeft = Instantiate(boundary, new Vector3(oX - w, oY, 1), Quaternion.identity);
        BlockLeft.transform.localScale = new Vector3(1, currH-oY, 1);
        GameObject BlockRight = Instantiate(boundary, new Vector3(oX + w, oY, 1), Quaternion.identity);
        BlockRight.transform.localScale = new Vector3(1, currH-oY, 1);
    }
}
