using UnityEngine;

public class RegisterToRoadNavMeshBehaviour : MonoBehaviour
{
    void Start()
    {        
        if (GameManager.Instance == null) { 
            Debug.Log($"RegisterToRoadNavMeshBehaviour in {this.transform.gameObject.name} -> geen GameManager Instance!"); 
            return; 
        }
        if (this.transform.parent == null)
        {
            GameManager.Instance.RegisterRoad(this.transform);
        }
        else
        {
            GameManager.Instance.RegisterRoad(this.transform.parent);
        }        
    }
}