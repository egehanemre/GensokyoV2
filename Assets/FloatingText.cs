using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 1f;
    public Vector3 randomizedIntensity = new Vector3(1f,0.3f,0);

    public TextMeshPro text;
    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        Destroy(gameObject, destroyTime);
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            transform.position += new Vector3(
                Random.Range(-randomizedIntensity.x, randomizedIntensity.x), 
                Random.Range(-randomizedIntensity.y, randomizedIntensity.y), 
                Random.Range(-randomizedIntensity.z, randomizedIntensity.z));
        }
    }
}
