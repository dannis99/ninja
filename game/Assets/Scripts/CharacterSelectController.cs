using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using System;

public class CharacterSelectController : MonoBehaviour {

    GameController gameController;

    string pressToPlay = "Press A To Play";
    string pressToLock = "Press A To Lock";

    List<Color> availableColors = new List<Color>{Color.red, Color.blue, Color.green, Color.yellow};

    public GameObject startText;

    public Sprite redHead;
    public Sprite redBody;
    public Sprite blueHead;
    public Sprite blueBody;
    public Sprite greenHead;
    public Sprite greenBody;
    public Sprite yellowHead;
    public Sprite yellowBody;

    public Rewired.Player player1Input;
    public Rewired.Player player2Input;
    public Rewired.Player player3Input;
    public Rewired.Player player4Input;

    public List<GameObject> playerNinjaSprites;

    public List<SpriteRenderer> playerHeadSprite;

    public List<SpriteRenderer> playerBodySprite;

    public List<GameObject> playerNoCharacterSprite;

    public GameObject[] playerPlayText;

    List<Color> playerColors = new List<Color>() {Color.clear, Color.clear, Color.clear, Color.clear};

    List<Color> playerLockedColors = new List<Color>() {Color.clear, Color.clear, Color.clear, Color.clear};

    List<bool> canChangeColor = new List<bool>() {true, true, true, true};

    int lockCount = 0;

    void Start()
	{
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

        gameController = GameObject.FindObjectOfType<GameController>();        
	}

	void Update()
	{
		if(player1Input.GetButtonDown("Accept")){
            playerAcceptPushed(0);
		}

        if (player2Input.GetButtonDown("Accept"))
        {
            playerAcceptPushed(1);
        }

        if (player3Input.GetButtonDown("Accept"))
        {
            playerAcceptPushed(2);
        }

        if (player4Input.GetButtonDown("Accept"))
        {
            playerAcceptPushed(3);
        }

        if (player1Input.GetButtonDown("Back"))
        {
            playerBackPushed(0);
        }

        if (player2Input.GetButtonDown("Back"))
        {
            playerBackPushed(1);
        }

        if (player3Input.GetButtonDown("Back"))
        {
            playerBackPushed(2);
        }

        if (player4Input.GetButtonDown("Back"))
        {
            playerBackPushed(3);
        }

        if(player1Input.GetAxis("Move Horizontal") > .3f)
        {
            StartCoroutine(playerRightPushed(0));
        }
        else if (player1Input.GetAxis("Move Horizontal") < -.3f)
        {
            StartCoroutine(playerLeftPushed(0));
        }

        if (player2Input.GetAxis("Move Horizontal") > .3f)
        {
            StartCoroutine(playerRightPushed(1));
        }
        else if (player2Input.GetAxis("Move Horizontal") < -.3f)
        {
            StartCoroutine(playerLeftPushed(1));
        }

        if (player3Input.GetAxis("Move Horizontal") > .3f)
        {
            StartCoroutine(playerRightPushed(2));
        }
        else if (player3Input.GetAxis("Move Horizontal") < -.3f)
        {
            StartCoroutine(playerLeftPushed(2));
        }

        if (player4Input.GetAxis("Move Horizontal") > .3f)
        {
            StartCoroutine(playerRightPushed(3));
        }
        else if (player4Input.GetAxis("Move Horizontal") < -.3f)
        {
            StartCoroutine(playerLeftPushed(3));
        }
    }
    
    void playerBackPushed(int playerIndex)
    {
        if (!playerNoCharacterSprite[playerIndex].activeSelf && playerPlayText[playerIndex].activeSelf)
        {
            playerNoCharacterSprite[playerIndex].SetActive(true);
            playerNinjaSprites[playerIndex].SetActive(false);
            playerPlayText[playerIndex].GetComponent<Text>().text = pressToPlay;
            playerColors[playerIndex] = Color.clear;            
        }        
        else if(!playerNoCharacterSprite[playerIndex].activeSelf && !playerPlayText[playerIndex].activeSelf)
        {
            lockCount--;
            if(lockCount < 1)
            {
                startText.SetActive(false);
            }
            playerLockedColors[playerIndex] = Color.clear;
            playerPlayText[playerIndex].SetActive(true);
            playerPlayText[playerIndex].GetComponent<Text>().text = pressToLock;
        }
    }

    void playerAcceptPushed(int playerIndex)
    {
        if (playerNoCharacterSprite[playerIndex].activeSelf)
        {
            playerNoCharacterSprite[playerIndex].SetActive(false);
            playerNinjaSprites[playerIndex].SetActive(true);
            playerPlayText[playerIndex].GetComponent<Text>().text = pressToLock;

            Color ninjaColor = getNextAvailableColor();
            setPlayerColor(playerIndex, ninjaColor);            
        }
        else if(playerLockedColors[playerIndex] == Color.clear)
        {
            playerLockIn(playerIndex);
        }
        else if(lockCount > 0)
        {
            LoadNewGame();
        }
    }

    IEnumerator playerRightPushed(int playerIndex)
    {
        if(canChangeColor[playerIndex] && !playerNoCharacterSprite[playerIndex].activeSelf && playerPlayText[playerIndex].activeSelf)
        {
            canChangeColor[playerIndex] = false;
            int previousColorIndex = availableColors.IndexOf(playerColors[playerIndex]);
            int startIndex = previousColorIndex + 1;
            if (startIndex == availableColors.Count)
                startIndex = 0;

            for (int i = startIndex; i < availableColors.Count; i++)
            {
                if (i == previousColorIndex)
                    break;

                if(isColorAvailable(availableColors[i]))
                    setPlayerColor(playerIndex, availableColors[i]);

                if (i == availableColors.Count - 1)
                    i = -1;
            }

            yield return new WaitForSeconds(.25f);
            canChangeColor[playerIndex] = true;
        }        
    }

    IEnumerator playerLeftPushed(int playerIndex)
    {
        if (canChangeColor[playerIndex] && !playerNoCharacterSprite[playerIndex].activeSelf && playerPlayText[playerIndex].activeSelf)
        {
            canChangeColor[playerIndex] = false;
            int previousColorIndex = availableColors.IndexOf(playerColors[playerIndex]);
            int startIndex = previousColorIndex - 1;
            if (startIndex == -1)
                startIndex = availableColors.Count - 1;

            for (int i = startIndex; i >= 0; i--)
            {
                if (i == previousColorIndex)
                    break;

                if (isColorAvailable(availableColors[i]))
                    setPlayerColor(playerIndex, availableColors[i]);

                if (i == 0)
                    i = availableColors.Count;
            }

            yield return new WaitForSeconds(.25f);
            canChangeColor[playerIndex] = true;
        }
    }

    private Color getNextAvailableColor()
    {
        Color nextAvailableColor = Color.clear;
        foreach (Color availableColor in availableColors)
        {
            bool colorAvailable = true;
            colorAvailable = isColorAvailable(availableColor);
            if (colorAvailable)
            {
                nextAvailableColor = availableColor;
                break;
            }
        }
        return nextAvailableColor;
    }

    private bool isColorAvailable(Color colorToCheck)
    {
        bool colorAvailable = true;
        foreach (Color currPlayerColor in playerLockedColors)
        {
            if (colorToCheck == currPlayerColor)
            {
                colorAvailable = false;
                break;
            }
        }

        return colorAvailable;
    }

    private void setPlayerColor(int playerIndex, Color ninjaColor)
    {
        playerColors[playerIndex] = ninjaColor;
        if (ninjaColor == Color.red)
        {
            playerHeadSprite[playerIndex].sprite = redHead;
            playerBodySprite[playerIndex].sprite = redBody;
        }
        else if (ninjaColor == Color.blue)
        {
            playerHeadSprite[playerIndex].sprite = blueHead;
            playerBodySprite[playerIndex].sprite = blueBody;
        }
        else if (ninjaColor == Color.green)
        {
            playerHeadSprite[playerIndex].sprite = greenHead;
            playerBodySprite[playerIndex].sprite = greenBody;
        }
        else if (ninjaColor == Color.yellow)
        {
            playerHeadSprite[playerIndex].sprite = yellowHead;
            playerBodySprite[playerIndex].sprite = yellowBody;
        }
    }

    private void playerLockIn(int playerIndex)
    {
        playerLockedColors[playerIndex] = playerColors[playerIndex];
        playerPlayText[playerIndex].SetActive(false);
        lockCount++;
        if (lockCount > 0)
        {
            startText.SetActive(true);
        }
    }

    public void LoadNewGame()
	{
        if(lockCount > 0)
        {
            gameController.setPlayerColors(playerLockedColors);
            SceneManager.LoadScene("spaceship_" + 1);//UnityEngine.Random.Range(1, 3));
        }        
	}
}
