﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
    public Player player;

    public int offset_x;
    public int offset_y;

    public Image bar;
    

	// Use this for initialization
	void Start () {
        bar.fillAmount = 0.24f;
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.position = new Vector3(player.transform.position.x + offset_x, player.transform.position.y + offset_y, transform.position.z);

        bar.fillAmount = 0.24f + (0.75f - 0.24f) * (player.death_bar / 100.0f);
	}
}
