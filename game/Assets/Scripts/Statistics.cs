using UnityEngine;
using System;
using System.Collections.Generic;
public abstract class Statistics : MonoBehaviour
{
	public static string KILLS = "Kills";
	public static string DEATHS = "Deaths";
	public static string SUICIDES = "Suicides";
	public static string SHIELDS_BROKEN = "Shields Broken";
	public static string SHIELDS_TAKEN = "Shields Taken";
	public static string CHESTS_OPENED = "Chests Opened";
	public static string JUMPS = "Jumps";
    public static string WALL_JUMPS = "Wall Jumps";
    public static string DODGES = "Dodges";
    public static string WALL_SLIDES = "Wall Slides";
    public static string GRENADES_THROWN = "Grenades Thrown";
    public static string SHURIKENS_THROWN = "Shurikens Thrown";

	private Dictionary<string, double> stats;
	public Statistics ()
	{
		stats = new Dictionary<string, double>();
	}
	
	public void incrementStat(string key)
	{
		incrementStat(key, 1.0f);
	}
	
	public void incrementStat(string key, double value)
	{
		Debug.Log("incrementing stat "+key+" by "+value);
		if(stats.ContainsKey(key))
		{
			stats[key] += value;
		}
		else
		{
			stats[key] = value;
		}
	}
	
	public Dictionary<string, double> getStats()
	{
		return stats;
	}

    public double getStat(string key)
    {
        if (stats.ContainsKey(key))
            return stats[key];
        else
            return 0;
    }
}

