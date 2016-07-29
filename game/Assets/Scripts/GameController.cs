using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


class GameController : MonoBehaviour
{
    public GameObject ninjaPrefab;
    public float roundLength;
    public float warningTime;
    public float oxygenTime;

    public float chestTimerMin;
    public float chestTimerMax;
    bool allChestsShown;
    float chestTimer;

    float gameTime;
    StartingPositions startingPositions;
    PlayerController[] players;
    List<Color> playerColors;
    WarningLight[] warningLights;
    Window[] windows;
    Chest[] chests;
    bool reloadingScene;
    bool inGame = false;


    private static GameController _instance;
    public static GameController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameController>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                DestroyImmediate(this.gameObject);
        }
    }

    void OnLevelWasLoaded(int level)
    {
        Scene scene = SceneManager.GetActiveScene();
        List<Vector2> selectedPositions = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
        if (scene.name != "characterSelect" && scene.name != "title")
        {
            startingPositions = Object.FindObjectOfType<StartingPositions>();
            int ninjaCount = 0;
            foreach(Color color in playerColors)
            {
                if(color != Color.clear)
                {
                    GameObject newNinjaGO = Instantiate(ninjaPrefab);
                    PlayerController newNinja = newNinjaGO.GetComponent<PlayerController>();
                    newNinja.playerId = ninjaCount;
                    newNinja.playerColor = color;
                    newNinja.initialize();

                    Vector2 ninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
                    while(selectedPositions.Contains(ninjaPosition))
                    {
                        ninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
                    }
                    newNinja.transform.position = ninjaPosition;
                }
                ninjaCount++;
            }

            //TEMP CODE FOR ADDING AN EXTRA NINJA
            GameObject tempNinjaGO = Instantiate(ninjaPrefab);
            PlayerController tempNewNinja = tempNinjaGO.GetComponent<PlayerController>();
            tempNewNinja.playerId = 1;
            tempNewNinja.playerColor = Color.yellow;
            tempNewNinja.initialize();

            Vector2 tempNninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
            while (selectedPositions.Contains(tempNninjaPosition))
            {
                tempNninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
            }
            tempNewNinja.transform.position = tempNninjaPosition;
            //END TEMP CODE



            players = Object.FindObjectsOfType<PlayerController>();
            warningLights = Object.FindObjectsOfType<WarningLight>();
            windows = Object.FindObjectsOfType<Window>();
            chests = Object.FindObjectsOfType<Chest>();
            foreach (Chest chest in chests)
            {
                chest.gameObject.SetActive(false);
            }
            chestTimer = Random.Range(chestTimerMin, chestTimerMax);
            inGame = true;
        }
    }

    void Update()
    {
        if (inGame)
        {
            gameTime += Time.deltaTime;
            if (gameTime >= roundLength - warningTime)
            {
                foreach (WarningLight warningLight in warningLights)
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
            if (gameTime >= roundLength + oxygenTime)
            {
                foreach (PlayerController player in players)
                {
                    if (!player.dead)
                        player.takeDamage();
                }
            }

            if (!allChestsShown && gameTime >= chestTimer)
            {
                int chestIndex = Random.Range(0, chests.Length - 1);
                int chestsChecked = 0;
                for (int i = chestIndex; i < chests.Length; i++)
                {
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
                    else if (i == chests.Length - 1)
                    {
                        i = -1;
                    }

                    chestsChecked++;
                    if (chestsChecked >= chests.Length)
                        break;
                }
                chestTimer += Random.Range(chestTimerMin, chestTimerMax);
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

    public void setPlayerColors(List<Color> playerColors)
    {
        this.playerColors = playerColors;
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
        gameTime = 0;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void gameOver()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
