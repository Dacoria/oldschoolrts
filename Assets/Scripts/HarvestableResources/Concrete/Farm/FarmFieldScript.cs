using System;
using UnityEngine;

public class FarmFieldScript : MonoBehaviour, IRetrieveResourceFromObject
{
    public GameObject ObjectGrownOnField;

    [HideInInspector] public bool HasObjectGrownOnField;
    [HideInInspector] public bool HasObjectGrownOnFieldFinishedGrowing;
    private IGrowOnFarmField growOnFarmFieldScript;

    void Update()
    {
        CheckHasObjectGrownOnField();
        if (HasObjectGrownOnField)
        {
            if (!HasObjectGrownOnFieldFinishedGrowing)
            {
                HasObjectGrownOnFieldFinishedGrowing = growOnFarmFieldScript.HasFinishedGrowing();
            }            
        }
    }

    private void CheckHasObjectGrownOnField()
    {
        HasObjectGrownOnField = ObjectGrownOnField != null && ObjectGrownOnField.activeSelf; // als bv een graan GO wordt gekoppeld is deze nog niet actief -> pas reageren als die op actief wordt gezet
        if (HasObjectGrownOnField && growOnFarmFieldScript == null)
        {
            growOnFarmFieldScript = ObjectGrownOnField.GetComponent<IGrowOnFarmField>();
            if (growOnFarmFieldScript == null)
            {
                throw new Exception("Elk Go op farmfield heeft een script met interface 'IGrowOnFarmField' nodig!");
            }
        }

        if(!HasObjectGrownOnField)
        {
            HasObjectGrownOnFieldFinishedGrowing = false;
            if(transform.childCount > 0)
            {
                ObjectGrownOnField = transform.GetChild(0).gameObject;
            }
        }
    }

    public HarvestMaterialResource ResourceIsRetrieved()
    {
        if (ObjectGrownOnField == null || !HasObjectGrownOnFieldFinishedGrowing) 
        {
            Debug.Log("FarmFieldScript -> ResourceIsRetrieved -> Alleen gegroeide resource kan geoogst worden; door andere farmer net verwijderd?");
            return null;
        }
        Destroy(ObjectGrownOnField); // todo coole animatie? (zoals boom omvallen)
        ObjectGrownOnField = null;
        growOnFarmFieldScript = null;

        return new HarvestMaterialResource(MaterialResourceType.WHEAT, 1);
    }

    public bool CanRetrieveResource()
    {
        return !IsBeingOrHasBeenRetrieved;
    }

    private bool IsBeingOrHasBeenRetrieved = false;
    public void StartRetrievingResource(int materialNumberRequestedToHarvest = 1)
    {
        IsBeingOrHasBeenRetrieved = true;
    }
}