using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SummonGoScript : MonoBehaviour
{
    public SummonableBehaviour SummonPrefab;
    public int MinRangeToSummon = 1;
    public int MaxRangeToSummon = 5;

    public int MinDistanceBetweenSpawns = 2;

    public int TimeToWaitForSpawnInSeconds = 10;
    public int SpawnLimit = 5;

    public bool IsTryingToSummon => TimeStartedForSpawnTimer.HasValue;
    public DateTime? TimeStartedForSpawnTimer;

    void Start()
    {
        StartCoroutine(SummonGoAfterXSeconds());
    }

    private IEnumerator SummonGoAfterXSeconds()
    {
        var currSummons = GetComponentsInChildren<SummonableBehaviour>();
        if(currSummons.Length >= SpawnLimit)
        {
            yield return Wait4Seconds.Get(0.5f);
        }
        else
        {
            yield return new WaitForEndOfFrame(); // voor initieel laden spel; dan krijg je een vertraging de 1x -> dit voorkomt dat
            TimeStartedForSpawnTimer = DateTime.Now;
            yield return Wait4Seconds.Get(TimeToWaitForSpawnInSeconds);

            // refresh na wachten (summon kan gekilled zijn)
            currSummons = GetComponentsInChildren<SummonableBehaviour>();
            var positionToSummon = DeterminePosition(currSummons.ToList());

            if (!positionToSummon.IsEmptyVector())
            {
                var summonedGo = Instantiate(SummonPrefab, positionToSummon, Quaternion.identity, transform);
            }
        }

        TimeStartedForSpawnTimer = null;
        StartCoroutine(SummonGoAfterXSeconds());
    }

    private Vector3 DeterminePosition(List<SummonableBehaviour> currentSpawns)
    {        
        // 20x random positie proberen, anders 0-vector teruggeven
        for (var i = 0; i < 40; i++)
        {
            var randomVector2 = VectorExtensions.Random(MinRangeToSummon, MaxRangeToSummon);
            var newLoc = transform.position + new Vector3(randomVector2.x, 0, randomVector2.y);

            var distance = Vector3.Distance(newLoc, transform.position);
            if (distance < MaxRangeToSummon && !currentSpawns.Any(x => x != null && IsInRange(x.transform.position, newLoc, MinDistanceBetweenSpawns)))
            {
                return newLoc;
            }
        }

        // geen geschikte locatie gevonden
        return new Vector3(0, 0, 0);        
    }

    private bool IsInRange(Vector3 position, Vector3 newLoc, int minDistance)
    {
        return Vector3.Distance(position, newLoc) <= minDistance;
    }
}