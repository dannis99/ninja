using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rewired;
using UnityEngine.SceneManagement;
using System.Timers;

public class MenuController : MonoBehaviour {
    public static string GAME_PAUSED = "GAME_PAUSED";
    public static string GAME_RESUMED = "GAME_RESUMED";
    private GameController controller;
	public GameObject menu;
    public GameObject menuCanvas;
    public GameObject countdownCanvas;
    public Text countdownText;

    public Rewired.Player player1Input;
    public Rewired.Player player2Input;
    public Rewired.Player player3Input;
    public Rewired.Player player4Input;

    int countdown;
    bool resuming;

    void Start()
	{
		controller = GameObject.FindObjectOfType<GameController>();

        if (ReInput.players != null)
        {
            player1Input = ReInput.players.GetPlayer(0);
            player2Input = ReInput.players.GetPlayer(1);
            player3Input = ReInput.players.GetPlayer(2);
            player4Input = ReInput.players.GetPlayer(3);
        }
        else
        {
            SceneManager.LoadScene("title");
        }        
    }

    void Update()
    {
        if (player1Input.GetButtonDown("Start") || player2Input.GetButtonDown("Start") || player3Input.GetButtonDown("Start") || player4Input.GetButtonDown("Start"))
        {
            toggleMenu();
        }

        if (menu.activeSelf && (player1Input.GetButtonDown("Back") || player2Input.GetButtonDown("Back") || player3Input.GetButtonDown("Back") || player4Input.GetButtonDown("Back")))
        {
            startHidingMenu();
        }

        if (menu.activeSelf && (player1Input.GetButtonDown("Accept") || player2Input.GetButtonDown("Accept") || player3Input.GetButtonDown("Accept") || player4Input.GetButtonDown("Accept")))
        {
            quit();
        }
    }

    public void quit()
	{
		controller.gameOver();
		hideMenu();
	}
	
	public void toggleMenu()
	{
        if (menu.activeSelf && !resuming)
		{
			startHidingMenu();
		}
		else
		{
			showMenu();
		}
	}

    public void showMenu()
    {
        resuming = false;
        menu.SetActive(true);
        menuCanvas.SetActive(true);
        countdownCanvas.SetActive(false);
        Time.timeScale = 0;
        EventManager.TriggerEvent(GAME_PAUSED);
    }

    public void hideMenu()
    {
        resuming = false;
        menu.SetActive(false);
        Time.timeScale = 1;
        EventManager.TriggerEvent(GAME_RESUMED);
    }
    
    public void startHidingMenu()
    {
        countdown = 3;
        countdownText.text = "" + countdown;
        menuCanvas.SetActive(false);
        countdownCanvas.SetActive(true);
        StartCoroutine(ResumeGame());
    }

    IEnumerator ResumeGame()
    {
        resuming = true;
        float resumeCheckTime = Time.realtimeSinceStartup;
        float resumeEndTime = Time.realtimeSinceStartup + countdown;

        while (Time.realtimeSinceStartup < resumeEndTime)
        {
            if ((Time.realtimeSinceStartup - resumeCheckTime) >= 1f)
            {
                resumeCheckTime = Time.realtimeSinceStartup;
                countdown--;
                countdownText.text = "" + countdown;
            }

            yield return 0;
        }

        if(resuming && countdown <= 1)
            hideMenu();
    }
}
