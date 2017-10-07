using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public string game_scene = "GameScene";
    public string credits_scene = "CredtisScene";

    public Image play;
    public Image credits;
    public Image exit;

    public Image arrow;

    GameObject current_selected;

    public Color selected_color;
    public Color unselected_color;

	// Use this for initialization
	void Start () {
		EventSystem.current.SetSelectedGameObject(play.gameObject);

        current_selected = play.gameObject;
        HandleSelectionAnim();
	}

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if(selected != current_selected)
        {
            arrow.rectTransform.anchoredPosition = new Vector2(arrow.rectTransform.anchoredPosition.x, selected.transform.localPosition.y);
            current_selected = selected;
            HandleSelectionAnim();
        }
    }
	
	public void PlayPressed()
    {
        SceneManager.LoadScene(game_scene, LoadSceneMode.Single);
    }

    public void CreditsPressed()
    {
        SceneManager.LoadScene(credits_scene, LoadSceneMode.Single);
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    void HandleSelectionAnim()
    {

        if(current_selected == play.gameObject)
        {
            play.color = selected_color;
            play.rectTransform.anchoredPosition = new Vector2(494, play.rectTransform.anchoredPosition.y);
        }
        else
        {
            play.color = unselected_color;
            play.rectTransform.anchoredPosition = new Vector2(514, play.rectTransform.anchoredPosition.y);
        }

        if (current_selected == credits.gameObject)
        {
            credits.color = selected_color;
            credits.rectTransform.anchoredPosition = new Vector2(554, credits.rectTransform.anchoredPosition.y);
        }
        else
        {
            credits.color = unselected_color;
            credits.rectTransform.anchoredPosition = new Vector2(574, credits.rectTransform.anchoredPosition.y);
        }

        if (current_selected == exit.gameObject)
        {
            exit.color = selected_color;
            exit.rectTransform.anchoredPosition = new Vector2(491, exit.rectTransform.anchoredPosition.y);
        }
        else
        {
            exit.color = unselected_color;
            exit.rectTransform.anchoredPosition = new Vector2(511, exit.rectTransform.anchoredPosition.y);
        }
    }


}
