using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class banner_anim : MonoBehaviour {

    public int direction = 1;

    public float speed = 5.0f;

    public float limit = 550;

    RectTransform rect;

    public bool notify_manager = false;
    bool completed = false;

    public Flash flash = null;

	// Use this for initialization
	void Start () {
        rect = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        if (completed)
            return;
        rect.anchoredPosition += new Vector2(direction * Time.deltaTime * speed, 0);

        if(direction > 0)
        {
            if(rect.anchoredPosition.x >= limit * -direction)
            {
                rect.anchoredPosition = new Vector2(limit * -direction, rect.anchoredPosition.y);
                if(notify_manager)
                {
                    Invoke("Notify", 2.0f);
                }
                completed = true;
            }
        }
        else
        {
            if (rect.anchoredPosition.x <= limit * -direction)
            {
                rect.anchoredPosition = new Vector2(limit * -direction, rect.anchoredPosition.y);
                if (notify_manager)
                {
                    Invoke("Notify", 2.0f);
                }
                completed = true;
            }
        }
	}

    void Notify()
    {
        flash.IntroFinished();
    }
}
