using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject start_focus;

    bool controls_visible = false;

    public string game_scene = "GameScene";
    public string credits_scene = "CredtisScene";

    public Image controls_img;

	// Use this for initialization
	void Start () {
		EventSystem.current.SetSelectedGameObject(start_focus);
	}
	
	public void PlayPressed()
    {
        SceneManager.LoadScene(game_scene, LoadSceneMode.Single);
    }

    public void ControlsPressed()
    {
        controls_visible = !controls_visible;

        if(controls_visible)
        {
            //Show hide controls
            controls_img.enabled = true;
        }
        else
        {
            //Show hide controls
            controls_img.enabled = false;
        }
    }

    public void CreditsPressed()
    {
        SceneManager.LoadScene(credits_scene, LoadSceneMode.Single);
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}
