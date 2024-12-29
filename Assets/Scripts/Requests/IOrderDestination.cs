using UnityEngine;

public interface IOrderDestination
{
    public GameObject GetGO();
    public bool CanProcessOrder(SerfOrder serfOrder);
}