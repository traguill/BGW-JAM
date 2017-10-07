using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour {

    bool start = false;

    public GameModule gm;

    public Image blue;
    public Image red;
    public Image vs;

    Image img;

    public float speed = 8.0f;

    float alpha = 0.0f;
    bool done = false;
	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        if (done)
            return;
        if(start)
        {
            alpha += Time.deltaTime * speed;
            if(alpha >= 1.0f)
            {
                done = true;
                img.enabled = false;
                blue.enabled = false;
                red.enabled = false;
                vs.enabled = false;
                gm.black.gameObject.SetActive(false);
                Invoke("StartTheGame", 1.0f);
            }
            img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
        }
		
	}

    void StartTheGame()
    {
        gm.IntroFinished();
    }

    public void IntroFinished()
    {
        start = true;
    }
}
