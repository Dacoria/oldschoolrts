using System;
using UnityEngine;
using UnityEngine.UI;

public class CardUICarouselHandler : MonoBehaviourCI
{
    [ComponentInject] 
    private ICardCarousselDisplay CardCarousselDisplay;

    private GameObject RightArrowCardCarouselGo;
    private GameObject LeftArrowCardCarouselGo;

    private GridLayoutGroup GridLayoutGroup;

    public int CarouselRows = 1;
    public int CardsPerRow = 4;
    public int StepsPerClick = 1;

    private int NumberOfCardShown => CardsPerRow * CarouselRows - 2; // 2 plekken nodig voor arrows

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
                InitiateCarousel();
            }

            CarouselIsLoaded = true;
        }
    }

    private Image ImageLeftArrow;
    private Image ImageRightArrow;

    private Text TextLeftArrow;
    private Text TextRightArrow;

    private void InitiateCarousel()
    {
        RightArrowCardCarouselGo = Instantiate(MonoHelper.Instance.RightArrowCarousselUiPrefab, transform);
        LeftArrowCardCarouselGo = Instantiate(MonoHelper.Instance.LeftArrowCarousselUiPrefab, transform);
        LeftArrowCardCarouselGo.transform.SetAsFirstSibling();

        ImageLeftArrow = LeftArrowCardCarouselGo.GetComponentInChildren<Image>();
        ImageRightArrow = RightArrowCardCarouselGo.GetComponentInChildren<Image>();
        ImageRightArrow.color = Color.black; // links updaten adhv waarde

        TextLeftArrow = LeftArrowCardCarouselGo.GetComponentInChildren<Text>();
        TextRightArrow = RightArrowCardCarouselGo.GetComponentInChildren<Text>();

        EnableVisibleCards(0);
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

        var lightGray = new Color(0.8f, 0.8f, 0.8f);
        ImageLeftArrow.color = startIndex == 0 ? lightGray : Color.black;

        TextLeftArrow.text = (startIndex).ToString();

        var cardCountRightNotVisible = CardCarousselDisplay.GetCount() - startIndex - NumberOfCardShown;

        // waarom 1? right arrow is ook een item; zo corrigeer je het als je nog 1 item hebt
        if (cardCountRightNotVisible > 1)
        {
            GridLayoutGroup.constraintCount = CarouselRows;

            RightArrowCardCarouselGo.SetActive(true);
            TextRightArrow.text = (CardCarousselDisplay.GetCount() - startIndex - NumberOfCardShown).ToString();
        }
        else
        {
            RightArrowCardCarouselGo.SetActive(false);

            // opnieuw bepalen --> constraints updaten! (om zo de weergave te fixen)
            var itemsLeft = CardCarousselDisplay.GetCount() - startIndex - 1 + 1; // 1 eraf voor index die bij 0 start, 1 erbij voor arrow

            var rowsToShow = (int)(itemsLeft / CardsPerRow) + 1;
            GridLayoutGroup.constraintCount = rowsToShow;

            // alles activeren vanaf index
            for (int i = startIndex; i < CardCarousselDisplay.GetCount(); i++)
            {
                CardCarousselDisplay.SetActiveStatusCardGo(i, true);               
            }
        }
    }
    

    public void CarouselLeft_LeftClick()
    {
        var currentIndex = CardCarousselDisplay.GetIndexFirstEnabledCard();
        if (currentIndex > 0)
        {
            EnableVisibleCards(Math.Max(currentIndex - StepsPerClick, 0));
        }
    }

    public void CarouselLeft_MiddleClick()
    {
        var currentIndex = CardCarousselDisplay.GetIndexFirstEnabledCard();
        if (currentIndex > 0)
        {
            EnableVisibleCards(0);
        }
    }

    public void CarouselRight_LeftClick()
    {
        var currentIndex = CardCarousselDisplay.GetIndexFirstEnabledCard();
        if (currentIndex < CardCarousselDisplay.GetCount() - NumberOfCardShown)
        {
            EnableVisibleCards(currentIndex + StepsPerClick);
        }
    }

    public void CarouselRight_MiddleClick()
    {
        var currentIndex = CardCarousselDisplay.GetIndexFirstEnabledCard();
        if (currentIndex < CardCarousselDisplay.GetCount() - NumberOfCardShown)
        {
            var page = (int)Math.Floor((CardCarousselDisplay.GetCount() - 2) / (decimal)StepsPerClick);
            EnableVisibleCards(StepsPerClick * page);
        }
    }
}
