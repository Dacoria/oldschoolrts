using System;
using UnityEngine;
using UnityEngine.UI;

public class CardUICarouselHandler : MonoBehaviourCI
{
    [ComponentInject] 
    private ICardCarousselDisplay CardCarousselDisplay;

    private GridLayoutGroup GridLayoutGroup;

    public int CarouselRows;
    public int CardsPerRow;

    private int NumberOfCardShown => CardsPerRow * CarouselRows;

    void Start()
    {
        GridLayoutGroup = this.GetComponent<GridLayoutGroup>();
    }

    private bool CarouselIsLoaded;
    
    void Update()
    {
        // niet via start, omdat card loaden tijd kost. vandaar op deze manier initialiseren
        TryToStartCarousel();
    }

    private void TryToStartCarousel()
    {
        if (!CarouselIsLoaded && CardCarousselDisplay.CardsAreLoaded())
        {
            if (CardCarousselDisplay.GetCount() > (CarouselRows * CardsPerRow))
            {
                EnableVisibleCards(0);
            }

            CarouselIsLoaded = true;
        }
    }
   

    private void EnableVisibleCards(int startIndex)
    {
        for (int i = 0; i < CardCarousselDisplay.GetCount(); i++)
        {
            if (i >= startIndex && i < startIndex + NumberOfCardShown)
            {
                CardCarousselDisplay.SetActiveStatusCardGo(i, true);
            }
            else
            {
                CardCarousselDisplay.SetActiveStatusCardGo(i, false);
            }
        }
        var cardCountRightNotVisible = CardCarousselDisplay.GetCount() - startIndex - NumberOfCardShown;      

        // opnieuw bepalen --> constraints updaten! (om zo de weergave te fixen)
        var itemsLeft = CardCarousselDisplay.GetCount() - startIndex - 1; // 1 eraf voor index die bij 0 start, 1 erbij voor arrow

        var rowsToShow = (int)(itemsLeft / CardsPerRow) + 1;
        GridLayoutGroup.constraintCount = rowsToShow;

        // alles activeren vanaf index
        for (int i = startIndex; i < CardCarousselDisplay.GetCount(); i++)
        {
            CardCarousselDisplay.SetActiveStatusCardGo(i, true);               
        }
        
    }
}