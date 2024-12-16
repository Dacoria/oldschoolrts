using UnityEngine;

public class RegisterToRoadNavMeshBehaviour : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance == null) { Debug.Log("RegisterToRoadNavMeshBehaviour in " + this.transform.gameObject.name + " -> geen GameManager Instance!"); return; }
        GameManager.Instance.RegisterRoad(this.transform);
    }
}
