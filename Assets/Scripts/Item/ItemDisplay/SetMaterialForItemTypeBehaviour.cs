using System.Linq;
using UnityEngine;

public class SetMaterialForItemTypeBehaviour : MonoBehaviour
{
    public void SetMaterial(ItemType itemType)
    {
        var renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GameManager.Instance.ResourcePrefabItems.Single(x => x.ItemType == itemType).Icon.texture;
    }    
}