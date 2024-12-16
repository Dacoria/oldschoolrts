using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUIWrapperBehaviour: MonoBehaviour, IUiCardLeftClick
{
    private SkillItem _skillItem;
    private DependencyDirectionType _dependencyDirectionType;

    public Image SkillImage;
    public Image CardOutlineImage;
    public Image LineConnectUpImage;
    public Image LineConnectUpLeftImage;

    public Text TextName;
    public TMP_Text CostText;
    public Button UnlockButton;

    public SkillUIBehaviour SkillUIBehaviour;

    public DependencyDirectionType DependencyDirectionType
    {
        get { return _dependencyDirectionType; }
        set
        {
            _dependencyDirectionType = value;
            LineConnectUpImage.gameObject.SetActive(_dependencyDirectionType == DependencyDirectionType.Up);
            LineConnectUpLeftImage.gameObject.SetActive(_dependencyDirectionType == DependencyDirectionType.UpLeft);
        }
    }

    public SkillItem SkillItem
    {
        get { return _skillItem; }
        set{
            _skillItem = value;
            if (SkillItem != null)
            {
                TextName.text = _skillItem.SkillType.ToString().Replace("_", " ");

                if (_skillItem.IsUnlocked)
                {
                    LineConnectUpImage.color = LineConnectUpImage.color.SetA(1);
                    LineConnectUpLeftImage.color = LineConnectUpLeftImage.color.SetA(1);
                    CardOutlineImage.color = CardOutlineImage.color.SetA(1);
                    SkillImage.color = SkillImage.color.SetA(1);
                    CostText.color = CostText.color.SetA(1);

                    CostText.text = "";
                }
                else
                {
                    var transparencyLevel = 0.6f;

                    LineConnectUpImage.color = LineConnectUpImage.color.SetA(transparencyLevel);
                    LineConnectUpLeftImage.color = LineConnectUpLeftImage.color.SetA(transparencyLevel);
                    CardOutlineImage.color = CardOutlineImage.color.SetA(transparencyLevel);
                    SkillImage.color = SkillImage.color.SetA(transparencyLevel);
                    CostText.color = CostText.color.SetA(transparencyLevel);

                    CostText.text = _skillItem.Cost.ToString();
                }
            }
        }
    }

    public void ClickOnCardLeftClick()
    {
        if (_skillItem != null)
        {
            SkillUIBehaviour.SelectSkill(this);
        }
    }
}

