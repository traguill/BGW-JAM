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

        winner_p1.gameObject.SetActive(false);
        winner_p2.gameObject.SetActive(false);
        rematch.gameObject.SetActive(false);
        quit.gameObject.SetActive(false);
        black.gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(doing_intro)
        {
            return;
        }

        if(game_over)
        {
            if(Input.GetAxis("Cancel") > 0.0f)
            {
                ReturnToMenu();
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

    public void IntroFinished()
    {
        doing_intro = false;
        EventStart();
    }
}
