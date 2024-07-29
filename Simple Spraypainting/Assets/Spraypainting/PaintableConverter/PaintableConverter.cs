using UnityEngine;

public class PaintableConverter : MonoBehaviour
{
    [Header("Spraypaint material will be assigned during Awake")]
    public Material SpraypaintMaterial;

    [Header("Add the material to the specified element slot of the MeshRenderer")]
    public int MeshRenderElementSlot = 0;

    private void Awake()
    {
        GameObject parentGameObject = transform.parent.gameObject;
        PrepareForPaintingGameObject(parentGameObject);
    }

    void PrepareForPaintingGameObject(GameObject prepareObject)
    {
        // Remove existing colliders
        Collider[] colliders = prepareObject.GetComponents<Collider>();
        foreach (Collider collider in colliders)
            Destroy(collider);

        // Add a MeshCollider
        prepareObject.AddComponent<MeshCollider>();

        // Get the MeshRenderer
        if (!prepareObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            Debug.LogError("MeshRenderer component not found on the parent GameObject. " +
                "Ensure the PaintableConverter is added as a child of the object you want to make paintable.");

            return;
        }

        // Ensure the materials array is not null and the slot is within bounds
        Material[] materials = meshRenderer.materials;
        if (materials == null || MeshRenderElementSlot < 0 || MeshRenderElementSlot >= materials.Length)
        {
            Debug.LogError("Invalid MeshRenderElementSlot or materials array is null. " +
                "Check the MeshRenderer component of the object you want to make paintable and verify that you entered the correct index for the material element.");

            return;
        }

        // Replace the material at the specified slot
        materials[MeshRenderElementSlot] = SpraypaintMaterial;
        meshRenderer.materials = materials;

        // Set the tag of the parent GameObject to match the tag of this GameObject
        prepareObject.tag = gameObject.tag;

        // Destroy this GameObject
        Destroy(this.gameObject);
    }
}