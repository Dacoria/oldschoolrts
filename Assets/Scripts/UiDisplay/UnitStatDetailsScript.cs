using Assets.Army;
using System;
using UnityEngine;

public class UnitStatDetailsScript : MonoBehaviour
{
    public ImageTextBehaviour Attack;
    public ImageTextBehaviour Defence;
    public ImageTextBehaviour ArmorPen;
    public ImageTextBehaviour Health;
    public ImageTextBehaviour Range;
    public ImageTextBehaviour Dodge;
    public ImageTextBehaviour Speed;

    [ComponentInject]
    private SelectedDisplayCardScript SelectedDisplayCardScript;

    public void Awake()
    {
        this.ComponentInject();
    }

    private int UpdateCounter;
    private BarracksUnitSetting LastUpdatedBarracksUnitSetting;

    void Update()
    {
        if (UpdateCounter == 0)
        {
            if(SelectedDisplayCardScript?.SelectedDisplayUiCard?.CardUiHandler?.CallingBuilding != null)
            {
                var prodSettings = SelectedDisplayCardScript.SelectedDisplayUiCard.CardUiHandler.CallingBuilding.GetCardDisplaySetting(SelectedDisplayCardScript.SelectedDisplayUiCard.Type);


                if (prodSettings is BarracksUnitSetting)
                {
                    var barracksProdSettings = (BarracksUnitSetting)prodSettings;
                    if (LastUpdatedBarracksUnitSetting != barracksProdSettings)
                    {
                        UpdateStats(barracksProdSettings.UnitStats);
                    }
                    LastUpdatedBarracksUnitSetting = barracksProdSettings;
                }
            }
        }

        UpdateCounter++;
        if(UpdateCounter > 25)
        {
            UpdateCounter = 0;
        }
    }

    private void UpdateStats(UnitStatsSetting unitStats)
    {
        Attack.Text.text = unitStats.Offence.BaseDamage.ToString() + "\n" + (unitStats.Offence.AttackHitRate).ToString() + "%";
        Defence.Text.text = unitStats.Defence.ArmorValue.ToString();
        ArmorPen.Text.text = unitStats.Offence.BaseFlatArmorPenetration.ToString() + "\n" + (unitStats.Offence.BaseBonusArmorPenetrationPercentage * 100).ToString() + "%";
        Range.Text.text = unitStats.RangeToAttractEnemies.ToString() + "\n" + unitStats.RangeToAttack.ToString();
        Health.Text.text = unitStats.Health.ToString();
        Dodge.Text.text = unitStats.Defence.Dodge.ToString() + "%";
        Speed.Text.text = unitStats.Speed.ToString();

        Attack.Image.sprite = MonoHelper.Instance.GetSpriteForAttackType(unitStats.Offence.AttackType);
        Defence.Image.sprite = MonoHelper.Instance.GetSpriteForArmorType(unitStats.Defence.ArmorType);
                               
        CreateOrUpdateTooltipCanvas(Attack, GetAttackText(unitStats.Offence.AttackType));
        CreateOrUpdateTooltipCanvas(Defence, GetArmorText(unitStats.Defence.ArmorType));
        CreateOrUpdateTooltipCanvas(ArmorPen, "ArmorPen flat: Damage that ignores armor \n" +
            "ArmorPen%: Perc of armor reduction" );
        CreateOrUpdateTooltipCanvas(Range, "AttractRange: Range to start walking towards enemy\nAttackRange: Range to start attacking");
        CreateOrUpdateTooltipCanvas(Health, "Total health of unit");
        CreateOrUpdateTooltipCanvas(Dodge, "Dodge%: 35% base + Attackhit% - this value. Hitchance between 1-100%");
        CreateOrUpdateTooltipCanvas(Speed, "Speed of the unit");
    }

    private void CreateOrUpdateTooltipCanvas(ImageTextBehaviour stat, string tooltipText)
    {
        var tooltip = stat.GetComponent<TooltipTriggerCanvas>();
        if(tooltip == null)
        {
            tooltip = stat.gameObject.AddComponent<TooltipTriggerCanvas>();
        }

        tooltip.Content = tooltipText;
    }

    private string GetAttackText(AttackType attackType)
    {
        var attackText = "Base damage: Damage of unit (before calculations)\n" +
            "Attackhit%: 35% base + this value - Dodge% -> hitchance between 1-100%\n" +
            "Attacktype: " + attackType.ToString().Capitalize() + " -> Modifies the damage; depending on Armortype:";

        foreach (ArmorType armorType in Enum.GetValues(typeof(ArmorType)))
        {
            attackText += "\n- " + armorType.ToString().Capitalize() + ": " + DamageLookup.LookUp[attackType][armorType];
        }

        return attackText;
    }

    private string GetArmorText(ArmorType armorType)
    {
        var armorText = "Armor: Reduced the damage (between 1-100) if no armor penetration\n" +
            "Armortype: " + armorType.ToString().Capitalize() + " -> Modifies the damage; depending on Attacktype:";

        foreach (AttackType attackType in Enum.GetValues(typeof(AttackType)))
        {
            armorText += "\n- " + attackType.ToString().Capitalize() + ": " + DamageLookup.LookUp[attackType][armorType];
        }

        return armorText;
    }
}
