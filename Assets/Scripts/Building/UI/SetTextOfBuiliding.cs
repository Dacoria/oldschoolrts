using TMPro;
using UnityEngine;

public class SetTextOfBuiliding : MonoBehaviour
{
    [ComponentInject] private TextMeshPro TextMeshPro;

    public void Awake() {
        this.ComponentInject();
    }

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
