using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMover : MonoBehaviour {

	private Vector2 uvOffset = Vector2.zero;
	public Vector2 uvAnimationRate = new Vector2(0.0f, 1.0f);
	int materialIndex = 0;
	string textureName = "_MainTex";
	private Renderer renderer;

	void Start() {
		renderer = GetComponent<Renderer>();
	}

	void Update () {
		uvOffset += ( uvAnimationRate * Time.deltaTime );
		if(renderer.enabled)
		{
			renderer.materials[materialIndex].SetTextureOffset(textureName, uvOffset );
		}
	}
}
