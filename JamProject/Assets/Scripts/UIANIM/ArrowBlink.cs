using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowBlink : MonoBehaviour 
{

    public float show_time = 0.5f;
    public float hide_time = 0.2f;

    float timer = 0.0f;

    bool show = true;

    Image img;

    void Start()
    {
        img = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        timer += Time.deltaTime;
        if((timer >= show_time && show) || (!show && timer >= hide_time))
        {
            timer = 0.0f;
            show = !show;

            img.enabled = show;
        }
	}
}
