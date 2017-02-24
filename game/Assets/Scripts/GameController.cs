using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


class GameController : MonoBehaviour
{
    public static string EVENT_GAME_OVER = "GAME_OVER";

    public GameObject ninjaPrefab;
    public float roundLength;
    public int killCount;
    public float warningTime;
    public float oxygenTime;

    public float chestTimerMin;
    public float chestTimerMax;
    bool allChestsShown;
    float chestTimer;

    private float playerDeathTimeSpeed = .25f;

    float gameTime;
    StartingPositions startingPositions;
    PlayerController[] players;
    List<Color> playerColors;
    WarningLight[] warningLights;
    Window[] windows;
    Chest[] chests;
    bool reloadingScene;
    bool inGame = false;

    private EZReplayManager replayManager;


    private static GameController _instance;
    public static GameController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType<GameController>();
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
            replayManager = Object.FindObjectOfType<EZReplayManager>();
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

            if(players == null)
            {
                int ninjaCount = 0;
                foreach (Color color in playerColors)
                {
                    if (color != Color.clear)
                    {
                        GameObject newNinjaGO = Instantiate(ninjaPrefab);
                        PlayerController newNinja = newNinjaGO.GetComponent<PlayerController>();
                        newNinja.playerId = ninjaCount;
                        newNinja.playerColor = color;
                        newNinja.initialize();

                        Vector2 ninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
                        while (selectedPositions.Contains(ninjaPosition))
                        {
                            ninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
                        }
                        newNinja.transform.position = ninjaPosition;
                    }
                    ninjaCount++;
                }



                ////TEMP CODE FOR ADDING AN EXTRA NINJA
                //GameObject tempNinjaGO = Instantiate(ninjaPrefab);
                //PlayerController tempNewNinja = tempNinjaGO.GetComponent<PlayerController>();
                //tempNewNinja.playerId = 1;
                //tempNewNinja.playerColor = Color.yellow;
                //tempNewNinja.initialize();

                //Vector2 tempNninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
                //while (selectedPositions.Contains(tempNninjaPosition))
                //{
                //    tempNninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
                //}
                //tempNewNinja.transform.position = tempNninjaPosition;
                ////END TEMP CODE



                players = Object.FindObjectsOfType<PlayerController>();
            }
            else
            {
                int ninjaCount = 0;
                foreach (PlayerController player in players)
                {                    
                    Vector2 ninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
                    while (selectedPositions.Contains(ninjaPosition))
                    {
                        ninjaPosition = startingPositions.possibleNinjaPositions[Random.Range(0, startingPositions.possibleNinjaPositions.Length)];
                    }
                    player.transform.position = ninjaPosition;

                    player.initialize();
                    ninjaCount++;
                }
            }

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
                        player.takeDamage(null);
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
        EventManager.StartListening(PlayerController.EVENT_PLAYER_DEATH, playerDeath);
    }

    void OnDisable()
    {
        EventManager.StopListening(PlayerController.EVENT_PLAYER_DEATH, playerDeath);
    }

    public void setPlayerColors(List<Color> playerColors)
    {
        this.playerColors = playerColors;
    }

    void playerDeath()
    {
        slowTimeForDeath();
        Invoke("resumeTimeAfterDeath", .75f);
        int liveCount = 0;
        foreach(PlayerController player in players)
        {
            if (!player.dead)
                liveCount++;
        }
        if(liveCount < 2 && !reloadingScene)
        {
            reloadingScene = true;
            Invoke("reloadScene", 3f);
        }
    }

    void slowTimeForDeath()
    {
        Debug.Log("setting timeScale to " + playerDeathTimeSpeed);
        Time.timeScale = playerDeathTimeSpeed;
    }

    void resumeTimeAfterDeath()
    {
        //dont want to resume out of a pause
        if(Time.timeScale == playerDeathTimeSpeed)
            Time.timeScale = 1f;
    }

    void reloadScene()
    {
        List<PlayerController> winners = new List<PlayerController>();
        foreach(PlayerController player in players)
        {
            if(player.getStat(Statistics.KILLS) >= killCount)
            {
                winners.Add(player);
            }
        }

        if(winners.Count == 1)//winner
        {
        }
        else if(winners.Count > 1)//tie breaker
        {
        }
        else//keep going
        {
            reloadingScene = false;
            gameTime = 0;
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }        
    }

    public void gameOver()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
