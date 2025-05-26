using UnityEngine;
using TMPro;

public class FloatingGold : MonoBehaviour
{
    public float destroyTime = 3f;
    public Vector3 randomizedIntensity = new Vector3(0, 0, 0);

    public TextMeshPro text;
    public void Show(int goldAmount)
    {
        text.text = $"+{goldAmount}G";
        Destroy(gameObject, destroyTime);
    }

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