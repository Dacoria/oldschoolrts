using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MultipleGameObjectGrowScript : MonoBehaviour, IGrowOnFarmField
{
    public float GrowSpeed = 10f;
    public float StartScale = 0.1f;
    public float EndScale = 1f;

    private List<ObjectToGrow> growTargetProcessList; // welke go gepakt worden

    private bool hasFinishedGrowingAllGameObjects;

    public bool HasFinishedGrowing() => hasFinishedGrowingAllGameObjects;

    void Start()
    {
        var growTargets = GetComponentsInChildren<GameObjectGrowTarget>(true).ToList(); // script dat op elke GO zit die je wilt laten groeien
        if (growTargets.Count == 0) { throw new System.Exception("Hoort meerdere resultaten te hebben"); }
                
        growTargetProcessList = new List<ObjectToGrow>();

        foreach (var targetToGrow in growTargets)
        {
            var go = targetToGrow.gameObject;
            var targetSize = go.transform.localScale * EndScale;
            go.transform.localScale = targetToGrow.gameObject.transform.localScale * StartScale;

            growTargetProcessList.Add(new ObjectToGrow { GameObject = go, TargetSize = targetSize });
        }
    }

    void Update()
    {
        if(!hasFinishedGrowingAllGameObjects)
        {
            foreach (var gameObjectToGrow in growTargetProcessList.Where(x => !x.HasReachedTarget))
            {
                gameObjectToGrow.HasReachedTarget = Vector3.SqrMagnitude(gameObjectToGrow.GameObject.transform.localScale - gameObjectToGrow.TargetSize) < 0.01f;

                if (!gameObjectToGrow.HasReachedTarget)
                {
                    gameObjectToGrow.GameObject.transform.localScale = Vector3.MoveTowards(gameObjectToGrow.GameObject.transform.localScale, gameObjectToGrow.TargetSize, GrowSpeed * Time.deltaTime);
                }
            }
        }

        hasFinishedGrowingAllGameObjects = growTargetProcessList.All(x => x.HasReachedTarget);
    }

    public class ObjectToGrow
    {
        public GameObject GameObject;
        public Vector3 TargetSize;
        public bool HasReachedTarget;
    }
}