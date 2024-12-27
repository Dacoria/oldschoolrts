using System.Linq;
using UnityEngine;

public class SetMaterialForItemTypeBehaviour : MonoBehaviour
{
    public void SetMaterial(ItemType itemType)
    {
        var renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = ResourcePrefabs.Get().Single(x => x.ItemType == itemType).Icon.texture;
    }    
}