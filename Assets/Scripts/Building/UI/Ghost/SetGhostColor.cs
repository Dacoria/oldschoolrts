using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetGhostColor : BaseAEMonoCI
{
    public bool UseColorSetterOfMaterials = false; // voor als een building in 1 mesh meerdere materialen heeft -> die zijn niet op een temp material te zetten -> dan deze workaround....

    [ComponentInject] private GhostBuildingBehaviour GhostBuildingBehaviour;
    [ComponentInject] private SetTemplateMaterialInTemplateMode SetTemplateMaterialInTemplateMode; // error als deze er niet is
    [ComponentInject] private List<Renderer> Renderers;

    private bool hasSetColorGhostToBuild = false;

    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        if (GhostBuildingBehaviour != null && 
            GhostBuildingBehaviour.isActiveAndEnabled && 
            builderRequest.GameObject == GhostBuildingBehaviour.transform.parent.gameObject)
        {
            if (builderRequest.Status == BuildStatus.NEEDS_PREPARE)
            {
                var colorGhostToBuild = new Color(0.5f, 0.8f, 0.9f, 0.1f); // aquamarine blauw;
                SetColorBuilding(colorGhostToBuild);
            }
            else if (builderRequest.Status == BuildStatus.COMPLETED_PREPARING)
            {
                var colorGhostWaitForBeingBuild = new Color(0.7f, 0.7f, 0.9f, 0.1f); // licht paars;
                SetColorBuilding(colorGhostWaitForBeingBuild);
            }            
        }            
    } 

    private void SetColorBuilding(Color colorGhost)
    {
        if (UseColorSetterOfMaterials)
        {
            SetAllMaterialsToColor(colorGhost);
        }
        else
        {
            var templateMaterial = new Material(Shader.Find("Standard"));
            templateMaterial.color = colorGhost;

            SetMaterialsToTemplateMaterial(templateMaterial);
        }
    }

    private void SetMaterialsToTemplateMaterial(Material templateMaterial)
    {
        foreach (var renderer in Renderers.Where(renderer => renderer?.material != null && 
                                                             !renderer.material.name.Contains("Image") && 
                                                             !renderer.material.name.StartsWith("LiberationSans")))
        {
            renderer.material = templateMaterial;
        }
    }

    private void SetAllMaterialsToColor(Color color)
    {
        foreach (var renderer in Renderers.Where(renderer => renderer?.materials != null))
        {
            foreach (var rendererMaterial in renderer.materials.Where(x => 
                !x.name.Contains("Image") && !x.name.StartsWith("LiberationSans")))
            {
                rendererMaterial.color = color;
            }
        }
    }
}