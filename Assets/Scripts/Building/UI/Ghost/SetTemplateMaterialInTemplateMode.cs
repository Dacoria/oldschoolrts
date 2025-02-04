using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTemplateMaterialInTemplateMode : BaseAEMonoCI
{
    [ComponentInject] private GhostBuildingBehaviour GhostBuildingBehaviour;
    [ComponentInject] private List<Renderer> Renderers;

    private CheckCollisionHandler checkCollisionForBuilding; // init via start --> wordt later toegevoegd

    public bool UseColorSetterOfMaterials = false; // voor als een building in 1 mesh meerdere materialen heeft -> die zijn niet op een temp material te zetten -> dan deze workaround....

    private bool scriptIsActive;

    private Color NormalColor;
    private Color CollidingColor;

    void Start()
    {
        InitColorSettings();
        StartCoroutine(InitScriptWhenPossible());
    }

    private IEnumerator InitScriptWhenPossible()
    {
        //zorg dat er een CheckCollisionHandler is
        for (var i = 0; i <= 30; i++)
        {
            checkCollisionForBuilding = GetComponentInParent<CheckCollisionHandler>();
            if (checkCollisionForBuilding != null)
            {
                break;
            }
            else if (i < 30)
            {
                Debug.Log("Wait 0.1 sec");
                yield return Wait4Seconds.Get(0.1f);
                continue;
            }
            else
            {
                throw new System.Exception("SetTemplateMaterialInTemplateMode -> No CheckCollisionHandler found");
            }
        }

        InitScript();
    }

    private void InitScript()
    {
        if(GhostBuildingBehaviour == null || GhostBuildingBehaviour.isActiveAndEnabled)
        {
            Destroy(this);
        }

        if (!UseColorSetterOfMaterials)
        {
            SetMaterialsToTemplateMaterial();
        }
        else
        {
            SetAllMaterialsToColor(NormalColor);
        }

        scriptIsActive = true;
    }

    private void InitColorSettings()
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
    }        

    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        if (GhostBuildingBehaviour != null &&
            GhostBuildingBehaviour.isActiveAndEnabled &&
            builderRequest.GameObject == GhostBuildingBehaviour.transform.parent.gameObject)
        {
            if(builderRequest.Status == BuildStatus.NEEDS_PREPARE)
            {
                Destroy(this);
            }
        }
    }

    private bool previousIsColliding;

    void Update()
    {
        if(scriptIsActive)
        {
            if (checkCollisionForBuilding.IsColliding() != previousIsColliding)
            {
                var colorToSet = NormalColor;
                if(checkCollisionForBuilding.IsColliding())
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
            previousIsColliding = checkCollisionForBuilding.IsColliding();
        }       
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