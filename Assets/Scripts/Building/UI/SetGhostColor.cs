using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetGhostColor : MonoBehaviour
{
    public bool UseColorSetterOfMaterials = false; // voor als een building in 1 mesh meerdere materialen heeft -> die zijn niet op een temp material te zetten -> dan deze workaround....

    [ComponentInject] private GhostBuildingBehaviour GhostBuildingBehaviour;
    [ComponentInject] private SetTemplateMaterialInTemplateMode SetTemplateMaterialInTemplateMode; // error als deze er niet is
    [ComponentInject] private List<Renderer> Renderers;

    private bool updateActive;

    void Awake()
    {
        this.ComponentInject();
    }

    void Start()
    {
        updateActive = GhostBuildingBehaviour != null;
        ActionEvents.BuilderRequestStatusChanged += BuilderRequestStatusChanged;
    }

    private bool hasSetColorGhostToBuild = false;

    private void BuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        if (GhostBuildingBehaviour != null && 
            GhostBuildingBehaviour.isActiveAndEnabled && 
            builderRequest.GameObject == GhostBuildingBehaviour.transform.parent.gameObject && 
            builderRequest.Status == BuildStatus.COMPLETED_PREPARING)
        {
            var colorGhostWaitForBeingBuild = new Color(0.7f, 0.7f, 0.9f, 0.1f); // licht paars;
            SetColorBuilding(colorGhostWaitForBeingBuild);
        }
    }

    void Update()
    {
        if (updateActive)
        {
            if (GhostBuildingBehaviour != null && GhostBuildingBehaviour.isActiveAndEnabled)
            {
                // als ghost mode enabled is, zet eenmaal de kleur
                if(!hasSetColorGhostToBuild)
                {
                    var colorGhostToBuild = new Color(0.5f, 0.8f, 0.9f, 0.1f); // aquamarine blauw;
                    SetColorBuilding(colorGhostToBuild);
                    hasSetColorGhostToBuild = true;
                } 
                
                updateActive = false;//onnodige checks stoppen; verandert van kleur naar paars in BuilderRequestStatusChanged
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