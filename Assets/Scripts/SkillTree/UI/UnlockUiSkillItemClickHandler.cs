using UnityEngine.EventSystems;

public class UnlockUiSkillItemClickHandler : MonoBehaviourCI, IPointerClickHandler
{
    [ComponentInject] private SelectedSkillUiItemScript SelectedSkillUiItemScript;

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectedSkillUiItemScript.OnUnlockButtonClicked();
    }   
}