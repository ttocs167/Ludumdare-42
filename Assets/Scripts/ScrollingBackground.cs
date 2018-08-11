using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour {

	public float backgroundSize;

	private Transform cameraTransform;
	private Transform[] layers;
	private float viewZone = 10;
	private int bottomIndex;
	private int topIndex;

	private int ZPos = 1;

	// Use this for initialization
	void Start () {
		cameraTransform = Camera.main.transform;
		layers = new Transform[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			layers [i] = transform.GetChild (i);
		}

		topIndex = 0;
		bottomIndex = layers.Length - 1;
	}

	private void Update(){

		if (cameraTransform.position.y < (layers[bottomIndex].transform.position.y - viewZone)) {
			ScrollDown();
		}

		if (cameraTransform.position.y > (layers[topIndex].transform.position.y + viewZone)) {
			ScrollUp();
		}

	}


	private void ScrollUp(){
		int lastBottom = bottomIndex;

		layers[bottomIndex].position = Vector3.up *(layers[topIndex].position.y +backgroundSize);
		layers [bottomIndex].position = new Vector3(layers[bottomIndex].position.x, layers[bottomIndex].position.y, ZPos);
		topIndex = bottomIndex;
		bottomIndex--;
		if (bottomIndex < 0)
			bottomIndex = layers.Length - 1;	

	}

	private void ScrollDown(){
		int lastTop = topIndex;

		layers[topIndex].position = Vector3.up *(layers[bottomIndex].position.y -backgroundSize);
		layers [topIndex].position = new Vector3(layers[topIndex].position.x, layers[topIndex].position.y, ZPos);
		bottomIndex = topIndex;
		topIndex++;
		if (topIndex == layers.Length)
			topIndex = 0;
	}

}
