using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MultipleGameObjectGrowScript : MonoBehaviourCI, IGrowOnFarmField
{
    public float GrowSpeed = 10f;
    public float StartScale = 0.1f;
    public float EndScale = 1f;

    public bool DynamicallyRetrieveGoToGrow; //bij ja, wordt MultipleGameObjectToGrow opgehaald adhv children die de klasse 'GameObjectGrowTarget' hebben

    public List<GameObject> MultipleGameObjectToGrow; // welke go gepakt worden
    private List<ObjectToGrow> GrowProgressMultipleGameObjectToGrow; // welke go gepakt worden
    [ComponentInject] private List<GameObjectGrowTarget> GameObjectGrowTargets;

    private bool hasFinishedGrowingAllGameObjects;

    public bool HasFinishedGrowing() => hasFinishedGrowingAllGameObjects;

    void Start()
    {
        if(DynamicallyRetrieveGoToGrow)
        {
            SetMultipleGameObjectToGrow();
        }
        if (MultipleGameObjectToGrow.Count == 0) { throw new System.Exception("Hoort meerdere resultaten te hebben"); }

        GrowProgressMultipleGameObjectToGrow = new List<ObjectToGrow>();

        foreach(var gameObjectToGrow in MultipleGameObjectToGrow)
        {
            var targetSize = gameObjectToGrow.transform.localScale * EndScale;
            GrowProgressMultipleGameObjectToGrow.Add(new ObjectToGrow { GameObject = gameObjectToGrow, TargetSize = targetSize });

            //gameObjectToGrow.transform.localPosition = new Vector3(gameObjectToGrow.transform.localPosition.x, 0.04f, gameObjectToGrow.transform.localPosition.z);
            gameObjectToGrow.transform.localScale = gameObjectToGrow.transform.localScale * StartScale;
        }
    }

    private void SetMultipleGameObjectToGrow()
    {
        if(GameObjectGrowTargets.Count == 0) { throw new System.Exception("Hoort meerdere resultaten te hebben"); }

        MultipleGameObjectToGrow = new List<GameObject>();
        foreach (var targetToGrow in GameObjectGrowTargets)
        {
            MultipleGameObjectToGrow.Add(targetToGrow.gameObject);
        }
    }

    void Update()
    {
        if(!hasFinishedGrowingAllGameObjects)
        {
            foreach (var gameObjectToGrow in GrowProgressMultipleGameObjectToGrow.Where(x => !x.HasReachedTarget))
            {
                gameObjectToGrow.HasReachedTarget = Vector3.SqrMagnitude(gameObjectToGrow.GameObject.transform.localScale - gameObjectToGrow.TargetSize) < 0.01f;

                if (!gameObjectToGrow.HasReachedTarget)
                {
                    gameObjectToGrow.GameObject.transform.localScale = Vector3.MoveTowards(gameObjectToGrow.GameObject.transform.localScale, gameObjectToGrow.TargetSize, GrowSpeed * Time.deltaTime);
                }
            }
        }

        hasFinishedGrowingAllGameObjects = GrowProgressMultipleGameObjectToGrow.All(x => x.HasReachedTarget);
    }

    public class ObjectToGrow
    {
        public GameObject GameObject;
        public Vector3 TargetSize;
        public bool HasReachedTarget;
    }
}
