using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameModule : MonoBehaviour 
{
    public TurretManager turret_manager;
    public Player p1;
    public Player p2;

    public string menu_scene;
    public string game_scene;

    [Header("Start Text anim")]
    public Text start_text;
    public int max_font;

    bool game_over = false;
    float alpha_txt = 0.0f;

    bool doing_intro = true;
    int countdown = 3;
    float countdown_counter = 0.0f;

    [Header("GameOver Anim")]
    public Image winner_p1;
    public Image winner_p2;
    public Button rematch;
    public Button quit;
    public Image black;


	// Use this for initialization
	void Start () 
    {
        start_text.gameObject.SetActive(true);
        start_text.text = "3";
        start_text.fontSize = 0;

        winner_p1.gameObject.SetActive(false);
        winner_p2.gameObject.SetActive(false);
        rematch.gameObject.SetActive(false);
        quit.gameObject.SetActive(false);
        black.gameObject.SetActive(false);
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
                EventStart();
            }
        }
    }

    public void EndGame(int looser_id)
    {
        game_over = true;

        turret_manager.GameEnds();

        if (looser_id == 1)
            winner_p2.gameObject.SetActive(true);
        else
            winner_p1.gameObject.SetActive(true);
        rematch.gameObject.SetActive(true);
        quit.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(rematch.gameObject);

        black.gameObject.SetActive(true);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menu_scene);
    }

    public void Rematch()
    {
        SceneManager.LoadScene(game_scene);
    }

    void EventStart()
    {
        turret_manager.GameStarts();
        p1.GameStars();
        p2.GameStars();
    }
}
