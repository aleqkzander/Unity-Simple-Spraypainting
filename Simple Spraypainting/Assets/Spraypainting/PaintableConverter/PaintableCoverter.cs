using UnityEngine;

public class PaintableCoverter : MonoBehaviour
{
    public Material SpraypaintMaterial;

    private void Awake()
    {
        GameObject parentGameObject = transform.parent.gameObject;
        PrepareForPaintingGameObject(parentGameObject);
    }

    void PrepareForPaintingGameObject(GameObject prepareObject)
    {
        Collider[] colliders = prepareObject.GetComponents<Collider>();

        foreach (Collider collider in colliders)
        {
            Destroy(collider);
        }

        prepareObject.AddComponent<MeshCollider>();
        prepareObject.GetComponent<MeshRenderer>().material = SpraypaintMaterial;
        prepareObject.tag = gameObject.tag;

        Destroy(this.gameObject);
    }
}