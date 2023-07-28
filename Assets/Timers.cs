using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timers : MonoBehaviour
{
    public class Timer
    {
        public string name;
        public float startTime;
        public float duration;

        public Timer(string _name, float _startTime, float _duration)
        {
            this.name = _name;
            this.startTime = _startTime;
            this.duration = _duration;
        }
    }
    public static List<Timer> timers = new List<Timer>();
    
    public static void SetTimer(string name, float duration)
    {
        bool isGarbageTimerExists = false;
        for (int i = 0; i < timers.Count; i++)
        {
            if (name == timers[i].name)
            {
                if (Time.time - timers[i].startTime >=  timers[i].duration)
                {
                    isGarbageTimerExists = true;
                    timers[i].startTime = Time.time;
                    break;
                }
                else
                {
                    Debug.Log("There's an existing already!");
                    return;
                }
            }
        }
        if (!isGarbageTimerExists)
            timers.Add(new Timer(name, Time.time, duration));

        CleanGarbageTimer();
    }

    public static bool isTimerFinished(string name)
    {
        for (int i = 0; i < timers.Count; i++)
        {
            if (name == timers[i].name)
            {
                if (Time.time - timers[i].startTime >= timers[i].duration)
                {
                    timers.RemoveAt(i);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return true; // No such timer found, it may be cleaned, so it is finished.
    }
    public static float getTimerStart(string name)
    {
        for (int i = 0; i < timers.Count; i++)
        {
            if (name == timers[i].name)
            {
                return timers[i].startTime;
            }
        }
        return 0;
    }
    public static void CleanGarbageTimer()
    {
        timers.RemoveAll(t => Time.time - t.startTime >= t.duration);
    }
}

