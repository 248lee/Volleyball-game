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
                if (Time.time - timers[i].startTime >= timers[i].duration)
                {
                    isGarbageTimerExists = true;
                    timers[i].startTime = Time.time;
                    break;
                }
                else
                {
                    Debug.Log("There's an existing {" + name + "} already!");
                    return;
                }
            }
        }
        if (!isGarbageTimerExists)
            timers.Add(new Timer(name, Time.time, duration));

        CleanGarbageTimer();
    }

    public static void SetTimer(string name, float duration, bool canOverride)
    {
        bool isGarbageTimerExists = false;
        if (!canOverride)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (name == timers[i].name)
                {
                    if (Time.time - timers[i].startTime >= timers[i].duration)
                    {
                        isGarbageTimerExists = true;
                        timers[i].startTime = Time.time;
                        break;
                    }
                    else
                    {
                        Debug.Log("There's an existing {" + name + "} already!");
                        return;
                    }
                }
            }

        }
        else
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (name == timers[i].name)
                {
                    isGarbageTimerExists = true;
                    timers[i].startTime = Time.time;
                    break;
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
    public static float GetTimerPrgress(string name)
    {
        for (int i = 0; i < timers.Count; i++)
        {
            if (name == timers[i].name)
            {
                float timePassed = Time.time - timers[i].startTime;
                if (timePassed < timers[i].duration)
                    return timePassed / timers[i].duration;
                else
                {
                    timers.RemoveAt(i);
                    return 1f;
                }
            }
        }
        return 1f; // No such timer found, it may be cleaned, so it is hundred percent progressed.
    }
    public static void CleanGarbageTimer()
    {
        timers.RemoveAll(t => Time.time - t.startTime >= t.duration);
    }
}

