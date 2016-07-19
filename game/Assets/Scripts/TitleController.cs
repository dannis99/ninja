using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Rewired;

public class TitleController : MonoBehaviour {

	public Texture2D cursorTexture;
	public AudioClip titleMusic;
	
	private MusicController musicController;
    Player playerInput;

    void Start()
	{
        playerInput = ReInput.players.GetPlayer(0);
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        musicController = GameObject.FindObjectOfType<MusicController>();

        if (titleMusic != null)
		    musicController.playMusic(titleMusic);
	}

	void Update()
	{
		if(playerInput.GetButtonDown("Jump")){
			LoadNewGame();
		}
	}
	
	public void LoadNewGame()
	{
        SceneManager.LoadScene("characterSelect");
	}
}
