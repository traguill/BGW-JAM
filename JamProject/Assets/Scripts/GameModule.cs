using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameModule : MonoBehaviour 
{
    [Header("Start Text anim")]
    public Text start_text;
    public int max_font;

    bool game_over = false;
    float alpha_txt = 0.0f;

    bool doing_intro = true;
    int countdown = 3;
    float countdown_counter = 0.0f;

    [Header("GameOver Anim")]
    public Text winner;
    public Button rematch;
    public Button quit;


	// Use this for initialization
	void Start () 
    {
        start_text.gameObject.SetActive(true);
        start_text.text = "3";
        start_text.fontSize = 0;

        winner.gameObject.SetActive(false);
        rematch.gameObject.SetActive(false);
        quit.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(doing_intro)
        {
            Intro();
        }

        if(game_over)
        {
            if(Input.GetAxis("Cancel") > 0.0f)
            {
                ReturnToMenu();
            }
        }
		
	}

    void Intro()
    {
        countdown_counter += Time.deltaTime;
        start_text.fontSize = (int)((float)max_font * Mathf.Sin(countdown_counter));
        if(countdown_counter >= 1.0f)
        {
            --countdown;
            countdown_counter = 0.0f;
            start_text.fontSize = 0;
            if(countdown > 0)
            {
                start_text.text = countdown.ToString();
            }else if(countdown == 0)
            {
                start_text.text = "GO!";
            }
            else
            {
                start_text.gameObject.SetActive(false);
                doing_intro = false;
                //START!!!

                EndGame(1);
            }
        }
    }

    public void EndGame(int looser_id)
    {
        game_over = true;
        //Say winner and show retry/quit

        //Tell everybody to fucking stop!
        //winner stop->game is over
        //Bullets end
        int win = (looser_id == 1) ? 2 : 1;
        winner.text = "Player " + win + " wins!!!";

        winner.gameObject.SetActive(true);
        rematch.gameObject.SetActive(true);
        quit.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(rematch.gameObject);
    }

    public void ReturnToMenu()
    {
        Debug.Log("Changing scene to menu");
        //scene change shit
    }

    public void Rematch()
    {
        //Reload the scene
        Debug.Log("Reload the scene");
    }
}
