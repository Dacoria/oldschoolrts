using UnityEngine;

public class AddMeshColliderBehaviour : MonoBehaviour
{
    void Awake()
    {
        AddMeshCollider(gameObject);
    }

    private void AddMeshCollider(GameObject containerModel)
    {
        // Add mesh collider
        MeshFilter meshFilter = containerModel.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            MeshCollider meshCollider = containerModel.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
        // Add mesh collider (convex) for each mesh in child elements.
        Component[] meshes = containerModel.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mesh in meshes)
        {
            MeshCollider meshCollider = containerModel.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh.sharedMesh;
        }
    }
}