using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowAnim : MonoBehaviour {

    public float speed = 2.0f;

    public float displacement = 2.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float x_val = displacement * Mathf.Sin(Time.timeSinceLevelLoad * speed);

        transform.position = new Vector3(x_val, transform.position.y, transform.position.z);

	}
}
