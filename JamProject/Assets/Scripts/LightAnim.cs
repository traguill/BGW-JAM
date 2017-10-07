using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnim : MonoBehaviour {

    public float min_val = 0.4f;
    public float max_val = 0.7f;

    public float min_speed = 1.0f;
    public float max_speed = 2.0f;

    public bool going_min = true;

    float speed;
    float rnd_val;
	// Use this for initialization
    float base_value;

    SpriteRenderer render;

	void Start () {
        speed = Random.RandomRange(min_speed, max_speed);
        rnd_val = Random.Range(min_val, max_val);
        render = GetComponent<SpriteRenderer>();
        base_value = render.color.a;
	}
	
	// Update is called once per frame
	void Update () {
        float sin_val = Mathf.Sin(Time.timeSinceLevelLoad);
        if(sin_val > 0.0f)
        {
            if(going_min)
            {
                speed = Random.RandomRange(min_speed, max_speed);
                rnd_val = Random.Range(min_val, max_val);
                going_min = false;
            }
        }
        else
        {
            if(going_min == false)
            {
                speed = Random.RandomRange(min_speed, max_speed);
                rnd_val = Random.Range(min_val, max_val);
                going_min = true;
            }
        }

        render.color = new Color(render.color.r, render.color.g, render.color.b, base_value + rnd_val * sin_val);
	}
}
