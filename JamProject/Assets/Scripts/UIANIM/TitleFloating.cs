using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFloating : MonoBehaviour 
{
    public float displacement = 1.0f;
    public float speed = 3.0f;

    Vector2 initial_pos;

    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        initial_pos = rect.anchoredPosition;
    }
	
	// Update is called once per frame
	void Update () 
    {
        rect.anchoredPosition = initial_pos +new Vector2(0.0f, Mathf.Sin(Time.timeSinceLevelLoad * speed) * displacement);
	}
}
