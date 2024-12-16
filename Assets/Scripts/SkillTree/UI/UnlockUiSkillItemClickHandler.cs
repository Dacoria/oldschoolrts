using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnlockUiSkillItemClickHandler : MonoBehaviour, IPointerClickHandler
{
    [ComponentInject]
    private SelectedSkillUiItemScript SelectedSkillUiItemScript;

    private void Awake()
    {
        this.ComponentInject();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectedSkillUiItemScript.OnUnlockButtonClicked();
    }   
}
