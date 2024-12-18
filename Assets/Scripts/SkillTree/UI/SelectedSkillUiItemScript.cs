using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedSkillUiItemScript : MonoBehaviour
{
    public Image SkillImage;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI CostLabelText;
    public TextMeshProUGUI CostValueText;

    public Image CostStarImage;
    public Image SkillUnlockedImage;

    public GameObject LockedUnlockUiGo;
    public GameObject UnlockUiGo;

    public SkillUIBehaviour SkillUIBehaviour;

    public SkillItem _skillItem;
    public SkillItem SkillItem
    {
        get { return _skillItem; }
        set
        {
            _skillItem = value;
            gameObject.SetActive(_skillItem != null);

            if (_skillItem != null)
            {
                SkillImage.sprite = MonoHelper.Instance.GetSpriteForSkillType(_skillItem.SkillType);
                TitleText.text = _skillItem.SkillType.ToString().Replace("_", " ");
                DescriptionText.text = _skillItem.Description;

                // unlocked --> alleen vinkje
                SkillUnlockedImage.gameObject.SetActive(_skillItem.IsUnlocked);

                // locked, alles voor kosten
                CostLabelText.gameObject.SetActive(!_skillItem.IsUnlocked);
                CostValueText.text = _skillItem.IsUnlocked ? "" : _skillItem.Cost.ToString();
                CostStarImage.gameObject.SetActive(!_skillItem.IsUnlocked);
                
                // toon unlockbutton als unlocken mogelijk is
                var canBeUnlocked = SkillUIBehaviour.SkillCanBeUnlocked(SkillItem);
                LockedUnlockUiGo.gameObject.SetActive(!_skillItem.IsUnlocked && !canBeUnlocked);
                UnlockUiGo.gameObject.SetActive(!_skillItem.IsUnlocked && canBeUnlocked);


            }
        }
    }

    public void OnUnlockButtonClicked()
    {
        SkillUIBehaviour.UnlockSkillItem(SkillItem);
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);        
    }
}