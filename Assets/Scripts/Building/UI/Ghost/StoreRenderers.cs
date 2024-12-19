using System.Collections.Generic;
using UnityEngine;

public class StoreRenderers : MonoBehaviour
{
    private Renderer[] OriginalRenderers;
    private Renderer[] OriginalChildrenRenderers;

    private List<Material> StoreOriginalMaterials = new List<Material>(); 
    private List<Material> StoreChildrenMaterials = new List<Material>();

    private Material TemplateMaterial;
    private Material CollidingMaterial;
      

    public StoreRenderers(Renderer[] OriginalRenderers, Renderer[] OriginalChildrenRenderers)
    {
        this.OriginalRenderers = OriginalRenderers;
        this.OriginalChildrenRenderers = OriginalChildrenRenderers;

        this.TemplateMaterial = new Material(Shader.Find("Standard"));
        this.TemplateMaterial.color = Color.white;

        this.CollidingMaterial = new Material(Shader.Find("Standard"));
        this.CollidingMaterial.color = Color.red;
    }

    public void StoreCurrentMaterials()
    {
        for (var i = 0; i < OriginalRenderers.Length; i++)
        {
            if (OriginalRenderers[i]?.material != null)
            {
                StoreOriginalMaterials.Add(OriginalRenderers[i].material);
            }
        }
        for (var i = 0; i < OriginalChildrenRenderers.Length; i++)
        {
            if (OriginalChildrenRenderers[i]?.material != null)
            {
                StoreChildrenMaterials.Add(OriginalChildrenRenderers[i].material);
            }
        }
    }

    public void SetMaterialsToTemplateMaterial()
    {
        for (var i = 0; i < OriginalRenderers.Length; i++)
        {
            if (OriginalRenderers[i]?.material != null)
            {
               OriginalRenderers[i].material = TemplateMaterial;
            }
        }

        for (var i = 0; i < OriginalChildrenRenderers.Length; i++)
        {
            if (OriginalChildrenRenderers[i]?.material != null)
            {
               OriginalChildrenRenderers[i].material = TemplateMaterial;
            }
        }
    }

    public void SetColorToNotColliding()
    {
        TemplateMaterial.color = Color.white;
    }

    public void SetColorToColliding()
    {
        TemplateMaterial.color = Color.red;
    }

    public void RestoreOriginalMaterials()
    {
        var counter = 0;
        for (var i = 0; i < OriginalRenderers.Length; i++)
        {
            if (OriginalRenderers[i]?.material != null)
            {
                OriginalRenderers[i].material = StoreOriginalMaterials[counter];
                counter++;
            }
        }

        counter = 0;
        for (var i = 0; i < OriginalChildrenRenderers.Length; i++)
        {
            if (OriginalChildrenRenderers[i]?.material != null)
            {
                OriginalChildrenRenderers[i].material = StoreChildrenMaterials[counter];
                counter++;
            }
        }
    }
}