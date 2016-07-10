using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


class GameController : MonoBehaviour
{
    public float roundLength;
    public float warningTime;
    float gameTime;
    PlayerController[] players;
    WarningLight[] warningLights;
    bool reloadingScene;

    void Start()
    {
        players = Object.FindObjectsOfType<PlayerController>();
        warningLights = Object.FindObjectsOfType<WarningLight>();
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
}
