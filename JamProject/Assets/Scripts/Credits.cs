using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour 
{
    public string menu_scene = "MainMenu";
    public CreditsClass[] members;
    public float fade_in_speed = 1.0f;
    public float fade_out_speed = 2.0f;
    public float stay_time = 1.0f;

    int mem_id = 0;
    int state = 0; //0 fade in 1 stay 2 fade out

    float alpha = 0.0f;
    float stay_counter = 0.0f;

    void Start()
    {
        members[mem_id].container.SetActive(true);
        members[mem_id].img.color = new Color(1, 1, 1, alpha);
        members[mem_id].txt.color = new Color(1, 1, 1, alpha);
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetAxis("Cancel") > 0.0f)
        {
            GoToMenu();
            return;
        }
	    switch(state)
        {
            case 0:
                alpha += fade_in_speed * Time.deltaTime;
                if (alpha > 1.0f)
                {
                    alpha = 1.0f;
                    ++state;
                }
                members[mem_id].img.color = new Color(1, 1, 1, alpha);
                members[mem_id].txt.color = new Color(1, 1, 1, alpha);
                break;
            case 1:
                stay_counter += Time.deltaTime;
                if(stay_counter >= stay_time)
                {
                    stay_counter = 0.0f;
                    ++state;
                }
                break;
            case 2:
                 alpha -= fade_out_speed * Time.deltaTime;
                 if (alpha < 0.0f)
                 {
                     alpha = 0.0f;
                     state = 0;
                     members[mem_id].container.SetActive(false);
                     ++mem_id;
                     if (mem_id == 5)
                     {
                         //Quit
                         GoToMenu();
                         return;
                     }
                     else
                     {
                         members[mem_id].container.SetActive(true);
                         members[mem_id].img.color = new Color(1, 1, 1, alpha);
                         members[mem_id].txt.color = new Color(1, 1, 1, alpha);
                     }
                 }
                 else
                 {
                     members[mem_id].img.color = new Color(1, 1, 1, alpha);
                     members[mem_id].txt.color = new Color(1, 1, 1, alpha);
                 }
                break;
        }
	}

    void GoToMenu()
    {
        SceneManager.LoadScene(menu_scene, LoadSceneMode.Single);
    }
}
