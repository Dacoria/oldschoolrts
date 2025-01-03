using Assets.Army;
using System;

public class UnitStatDetailsScript : MonoBehaviourSlowUpdateFramesCI
{
    public ImageTextBehaviour Attack;
    public ImageTextBehaviour Defence;
    public ImageTextBehaviour Health;
    public ImageTextBehaviour Range;
    public ImageTextBehaviour Speed;

    [ComponentInject]
    private SelectedDisplayCardScript SelectedDisplayCardScript;

    private BarracksUnitSetting LastUpdatedBarracksUnitSetting;

    protected override int FramesTillSlowUpdate => 40;
    protected override void SlowUpdate()
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

    private void UpdateStats(UnitStatsSetting unitStats)
    {
        Attack.Text.text = unitStats.Offence.BaseDamage.ToString() + "\n" + (unitStats.Offence.AttackHitRate).ToString() + "%";
        Defence.Text.text = unitStats.Defence.ArmorValue.ToString();
        Range.Text.text = unitStats.RangeToAttractEnemies.ToString() + "\n" + unitStats.RangeToAttack.ToString();
        Health.Text.text = unitStats.Health.ToString();
        Speed.Text.text = unitStats.Speed.ToString();

        Attack.Image.sprite = MonoHelper.Instance.GetSpriteForAttackType(unitStats.Offence.AttackType);
        Defence.Image.sprite = MonoHelper.Instance.GetSpriteForArmorType(unitStats.Defence.ArmorType);
                               
        CreateOrUpdateTooltipCanvas(Attack, GetAttackText(unitStats.Offence.AttackType));
        CreateOrUpdateTooltipCanvas(Defence, GetArmorText(unitStats.Defence.ArmorType));

        CreateOrUpdateTooltipCanvas(Range, "AttractRange: Range to start walking towards enemy\nAttackRange: Range to start attacking");
        CreateOrUpdateTooltipCanvas(Health, "Total health of unit");
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
        var attackText = $"Base damage: Damage of unit (before calculations)\n" +
            $"Attacktype: {attackType.ToString().Capitalize()} -> Modifies the damage; depending on Armortype:";

        foreach (ArmorType armorType in Enum.GetValues(typeof(ArmorType)))
        {
            attackText += $"\n- {armorType.ToString().Capitalize()}: {DamageLookup.LookUp[attackType][armorType]}";
        }

        return attackText;
    }

    private string GetArmorText(ArmorType armorType)
    {
        var armorText = "Armor: Reduced the damage (between 1-100)\n" +
            $"Armortype: {armorType.ToString().Capitalize()} -> Modifies the damage; depending on Attacktype:";

        foreach (AttackType attackType in Enum.GetValues(typeof(AttackType)))
        {
            armorText += $"\n- {attackType.ToString().Capitalize()}: {DamageLookup.LookUp[attackType][armorType]}";
        }

        return armorText;
    }
}