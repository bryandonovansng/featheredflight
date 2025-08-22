using UnityEngine;

public class AddCollidersToChildren : MonoBehaviour
{
    public bool useBoxCollider = true; // Or set to false for MeshCollider

    void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Collider>() == null)
            {
                if (useBoxCollider)
                    child.gameObject.AddComponent<BoxCollider>();
                else
                    child.gameObject.AddComponent<MeshCollider>();
            }
        }

        Debug.Log("✅ Colliders added to all children of " + gameObject.name);
    }
}
