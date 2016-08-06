using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StatsController : MonoBehaviour {

	private Dictionary<Object, Dictionary<string, double>> gameStats;
	private int gameStatLineHeight = 15;
	
	public GameObject resultCanvas;
	public GameObject[] champStatsPanels;
	public GameObject statsTextPrefab;
	
	private static StatsController _instance;
	public static StatsController instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<StatsController>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}
	
	void OnEnable()
	{
        EventManager.StopListening(GameController.EVENT_GAME_OVER, onGameOver);
    }
	
	void OnDisable()
	{
        EventManager.StartListening(GameController.EVENT_GAME_OVER, onGameOver);
	}
	
	void onGameOver(){
		DestroyObject(gameObject);
	}
	
	void Awake () {
		if(_instance == null)
		{
			_instance = this;
			gameStats = new Dictionary<Object, Dictionary<string, double>>();
			DontDestroyOnLoad(this);
		}
		else
		{
			if(this != _instance)
				DestroyImmediate(this.gameObject);
		}
	}
	
	public void showStats()
	{
		resultCanvas.SetActive(true);
		int champCount = 0;
		foreach(KeyValuePair<Object, Dictionary<string, double>> entry in gameStats)
		{
			PlayerController player = (PlayerController)entry.Key;
			addStatsText(champStatsPanels[champCount].transform, 65, player.playerColor+" Ninja Stats", new Color(1f, .9f, 0f));
			Dictionary<string, double> stats = entry.Value;
			int currYPosition = 90;
			foreach(KeyValuePair<string, double> subEntry in entry.Value)
			{
				string currStatText = subEntry.Key + " - " + Mathf.Round((float)subEntry.Value);
				addStatsText(champStatsPanels[champCount].transform, currYPosition, currStatText, getStatColor(subEntry.Key));
				currYPosition += gameStatLineHeight;		
			}
			champCount++;
		}
	}
	
	void addStatsText(Transform parent, int currYPosition, string currStatsText, Color color)
	{
		GameObject statsText = Instantiate(statsTextPrefab) as GameObject;
		statsText.transform.SetParent(parent, false);
		statsText.transform.localPosition = new Vector2(statsText.transform.localPosition.x, -currYPosition);
		statsText.GetComponent<RectTransform>().sizeDelta = new Vector2(statsText.GetComponent<RectTransform>().sizeDelta.x, gameStatLineHeight);
		Debug.Log("localPosition: "+statsText.transform.localPosition+" sizeDelta: "+statsText.GetComponent<RectTransform>().sizeDelta);
		statsText.GetComponent<Text>().text = currStatsText;
		statsText.GetComponent<Text>().color = color;
	}
	
	Color getStatColor(string text)
	{
		Color color = Color.white;
		
		if(text.Contains("Damage"))
		{
			color = new Color(1f, 0f, 0f);
		}
		else if(text.Contains("Shield"))
		{
			color = new Color(1f, .8f, 0f);
		}
		else if(text.Contains("Heal"))
		{
			color = new Color(0f, 1f, 0f);
		}
		
		
		return color;
	}
	
	public void setStats(Object key, Dictionary<string, double> stats)
	{
		gameStats[key] = stats;
	}
	
	public Dictionary<Object, Dictionary<string, double>> getStats()
	{
		return gameStats;
	}
}
