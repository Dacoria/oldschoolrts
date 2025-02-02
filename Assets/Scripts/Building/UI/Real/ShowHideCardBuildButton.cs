using UnityEngine;

public class ShowHideCardBuildButton : MonoBehaviourSlowUpdateFramesCI
{
    [ComponentInject] private CardSelectProdUiHandler cardSelectProdUiHandler;

    protected override int FramesTillSlowUpdate => 25;

    protected override void SlowUpdate()
    {
        if(!cardSelectProdUiHandler.CardsAreLoaded)
        {
            return;
        }

        foreach(var cardUI in cardSelectProdUiHandler.UiCardBehaviours)
        {
            var canProcess = cardSelectProdUiHandler.CallingBuilding.CanProces(cardUI.Type);
            cardUI.BuildButtonGo.SetActive(canProcess);            
        }
    }
}