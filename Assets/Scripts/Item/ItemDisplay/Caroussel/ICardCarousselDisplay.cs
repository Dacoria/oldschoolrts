public interface ICardCarousselDisplay
{
    int GetCount();
    bool CardsAreLoaded();
    void SetActiveStatusCardGo(int indexOfCard, bool activeYN);
    int GetIndexFirstEnabledCard();
}