using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderOnFather : MonoBehaviour {

    SpriteRenderer s_ren;
    SpriteRenderer p_ren;
	// Use this for initialization
	void Start () {
        s_ren = GetComponent<SpriteRenderer>();
        p_ren = transform.parent.GetComponent<SpriteRenderer>();

    }
	
	// Update is called once per frame
	void Update () {
        s_ren.sortingOrder = p_ren.sortingOrder + 1;
	}
}
