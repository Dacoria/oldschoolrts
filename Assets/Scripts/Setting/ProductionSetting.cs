using Assets.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class ProductionSetting
{
    public List<ItemAmountBuffer> ItemsConsumedToProduce;
    public new abstract Enum GetType();
    public abstract Sprite GetIcon();
}

