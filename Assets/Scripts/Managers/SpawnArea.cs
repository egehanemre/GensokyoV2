using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public Vector3 GetRandomPointInArea()
    {
        Vector3 center = boxCollider.center + transform.position;
        Vector3 size = boxCollider.size * 0.5f;

        float x = Random.Range(center.x - size.x, center.x + size.x);
        float y = center.y;
        float z = Random.Range(center.z - size.z, center.z + size.z);

        return new Vector3(x, y, z);
    }
}