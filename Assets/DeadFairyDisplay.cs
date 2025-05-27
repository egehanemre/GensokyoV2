using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadFairyDisplay : MonoBehaviour
{
    public FairyData fairyData;
    public GameObject fairyDisplay;
    public GameObject objectParent;

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public void Init()
    {
        if (fairyData == null || fairyData.FairyPrefab == null || objectParent == null) return;

        if (!fairyData.FairyPrefab.TryGetComponent<Fairy>(out var fairy)) return;

        if (fairyDisplay != null)
            Destroy(fairyDisplay);

        if (fairy.fairyImageForShop != null)
        {
            fairyDisplay = Instantiate(fairy.fairyImageForShop, objectParent.transform);
            fairyDisplay.transform.localPosition = Vector3.zero;
            fairyDisplay.transform.localRotation = Quaternion.identity;
            fairyDisplay.transform.localScale = Vector3.one;

            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer != -1)
                SetLayerRecursively(fairyDisplay, uiLayer);
        }
    }
}