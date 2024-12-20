using TMPro;

public class SetTextOfBuilding : MonoBehaviourCI
{
    [ComponentInject] private TextMeshPro TextMeshPro;

    void Start()
    {
        if(TextMeshPro != null)
        {
            var highestParentGo = transform.gameObject;

            for (var i = 0; i < 10; i++)
            {
                if(highestParentGo.transform.parent != null)
                {
                    highestParentGo = highestParentGo.transform.parent.gameObject;
                }
                else
                {
                    break;
                }
                if(i == 4) { throw new System.Exception("onwaarschijnlijk dat er zoveel parent-niveaus zijn --> hoe dan?"); }
            }

            TextMeshPro.text = highestParentGo.name.Replace("Prefab", "").Replace("(Clone)", "");
        }
    }
}