using System.Collections.Generic;
using UnityEngine;

public class SetTemplateMaterialInTemplateMode : MonoBehaviourCI
{
    [ComponentInject] private GhostBuildingBehaviour GhostBuildingBehaviour;
    [ComponentInject] private CheckCollisionHandler CheckCollisionForBuilding;
    [ComponentInject] private List<Renderer> Renderers;

    public bool UseColorSetterOfMaterials = false; // voor als een building in 1 mesh meerdere materialen heeft -> die zijn niet op een temp material te zetten -> dan deze workaround....

    private bool ScriptIsActive;

    private Color NormalColor;
    private Color CollidingColor;

    void Start()
    {
        NormalColor = Color.white;
        CollidingColor = Color.red;

        if (!UseColorSetterOfMaterials)
        {
            this.TemplateMaterial = new Material(Shader.Find("Standard"));
            this.TemplateMaterial.color = NormalColor;

            this.CollidingMaterial = new Material(Shader.Find("Standard"));
            this.CollidingMaterial.color = CollidingColor;
        }

        ScriptIsActive = GhostBuildingBehaviour != null && !GhostBuildingBehaviour.isActiveAndEnabled;
        if(ScriptIsActive)
        {
            if(!UseColorSetterOfMaterials)
            {
                SetMaterialsToTemplateMaterial();
            }
            else
            {
                SetAllMaterialsToColor(NormalColor);
            }
            PreviousCheckIsInGhostMode = GhostBuildingBehaviour.isActiveAndEnabled;
        }
    }

    private bool PreviousCheckIsInGhostMode;
    private bool PreviousIsColliding;

    void Update()
    {
        if(ScriptIsActive)
        {
            if (GhostBuildingBehaviour != null && PreviousCheckIsInGhostMode != GhostBuildingBehaviour.isActiveAndEnabled)
            {
                ScriptIsActive = false;
            }
            else if (CheckCollisionForBuilding != null && CheckCollisionForBuilding.IsColliding() != PreviousIsColliding)
            {
                var colorToSet = NormalColor;
                if(CheckCollisionForBuilding.IsColliding())
                {
                    colorToSet = CollidingColor;
                } 

                if (UseColorSetterOfMaterials)
                {
                    SetAllMaterialsToColor(colorToSet);
                }
                else
                {
                    TemplateMaterial.color = colorToSet;
                }                
            }
        }

        PreviousIsColliding = CheckCollisionForBuilding.IsColliding();
    }

    // VOOR OPSLAAN VAN RENDERERS & ZETTEN VAN KLEUREN

    [ComponentInject] private Renderer[] OriginalChildrenRenderers;
    private Material TemplateMaterial;
    private Material CollidingMaterial;
    private Material ResourceMatchedMaterial;

    private void SetMaterialsToTemplateMaterial()
    {
        foreach (var renderer in OriginalChildrenRenderers)
        {
            if (renderer?.material != null && 
                !renderer.material.name.Contains("Image") && 
                !renderer.material.name.StartsWith("LiberationSans"))
            {
                renderer.material = TemplateMaterial;
            }
        }
    }

    private void SetAllMaterialsToColor(Color color)
    {
        foreach (var renderer in OriginalChildrenRenderers)
        {
            if (renderer?.materials != null)
            {
                for (var j = 0; j < renderer?.materials.Length; j++)
                {
                    if (renderer?.materials[j] != null &&
                        !renderer.materials[j].name.Contains("Image") &&
                        !renderer.materials[j].name.StartsWith("LiberationSans"))
                    {
                        renderer.materials[j].color = color;
                    }
                }
            }
        }
    }
}