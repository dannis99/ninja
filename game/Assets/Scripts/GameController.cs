using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


class GameController : MonoBehaviour
{
    public float roundLength;
    public float warningTime;
    public float oxygenTime;

    public float chestTimerMin;
    public float chestTimerMax;
    bool allChestsShown;
    float chestTimer;

    float gameTime;
    PlayerController[] players;
    WarningLight[] warningLights;
    Window[] windows;
    Chest[] chests;
    bool reloadingScene;

    void Start()
    {
        players = Object.FindObjectsOfType<PlayerController>();
        warningLights = Object.FindObjectsOfType<WarningLight>();
        windows = Object.FindObjectsOfType<Window>();
        chests = Object.FindObjectsOfType<Chest>();
        foreach(Chest chest in chests)
        {
            chest.gameObject.SetActive(false);
        }
        chestTimer = Random.Range(chestTimerMin, chestTimerMax);        
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        if(gameTime >= roundLength - warningTime)
        {
            foreach(WarningLight warningLight in warningLights)
            {
                warningLight.beginWarning();
            }
        }
        if (gameTime >= roundLength)
        {
            foreach (Window window in windows)
            {
                window.breakWindow();
            }
        }
        if(gameTime >= roundLength + oxygenTime)
        {
            foreach (PlayerController player in players)
            {
                if(!player.dead)
                    player.takeDamage();
            }
        }

        if (!allChestsShown && gameTime >= chestTimer)
        {
            int chestIndex = Random.Range(0, chests.Length - 1);
            int chestsChecked = 0;
            for(int i = chestIndex; i < chests.Length; i++)
            {
                Debug.Log("i: "+i);
                if (!chests[i].gameObject.activeSelf)
                {
                    chests[i].gameObject.SetActive(true);
                    break;
                }
                else if (i == chestIndex && chestsChecked > 0)
                {
                    allChestsShown = true;
                    break;
                }
                else if(i == chests.Length - 1)
                {
                    i = -1;
                }
                
                chestsChecked++;
                if (chestsChecked >= chests.Length)
                    break;
            }
            chestTimer += Random.Range(chestTimerMin, chestTimerMax);
            Debug.Log("chestTimer: " + chestTimer + " gameTime: " + gameTime);
        }
    }

    void OnEnable()
    {
        EventManager.StartListening("playerDeath", playerDeath);
    }

    void OnDisable()
    {
        EventManager.StopListening("playerDeath", playerDeath);
    }

    void playerDeath()
    {
        int liveCount = 0;
        foreach(PlayerController player in players)
        {
            if (!player.dead)
                liveCount++;
        }

        if(liveCount < 2 && !reloadingScene)
        {
            reloadingScene = true;
            Invoke("reloadScene", 2f);
        }
    }

    void reloadScene()
    {
        reloadingScene = false;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void gameOver()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
