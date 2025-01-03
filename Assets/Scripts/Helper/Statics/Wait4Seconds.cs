using UnityEngine;
using System.Collections.Generic;

public static class Wait4Seconds
{
    static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds Get(float seconds)
    {
        if (!_timeInterval.ContainsKey(seconds))
        {
            _timeInterval.Add(seconds, new WaitForSeconds(seconds));
        }
        return _timeInterval[seconds];
    }
}