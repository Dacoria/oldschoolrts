using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StockPilesManager : MonoBehaviour
{
    public static StockPilesManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private List<StockpileBehaviour> Stockpiles = new List<StockpileBehaviour>();
    public List<StockpileBehaviour> GetStockpiles() => Stockpiles;

    public void RegisterStockpile(StockpileBehaviour stockpileBehaviour)
    {
        Stockpiles.Add(stockpileBehaviour);
    }

    public void TryRemoveStockpile(StockpileBehaviour stockpileBehaviour)
    {
        var stockpile = GetStockpiles().FirstOrDefault(x => x == stockpileBehaviour);
        if (stockpile != null)
        {
            Stockpiles.Remove(stockpile);
        }
    }
}