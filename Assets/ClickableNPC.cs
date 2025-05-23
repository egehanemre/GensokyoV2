using UnityEngine;

public class ClickableNPC : MonoBehaviour
{
    private Outline outline;
    [SerializeField] private GameObject NPCText;
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private NPCType npcType;
    [SerializeField] private GameObject CombatPanel;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        NPCText.SetActive(false);
        UIPanel.SetActive(false);
        CombatPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && UIPanel.activeSelf)
        {
            ClosePanel();
        }
    }

    private void OnMouseEnter()
    {
        outline.enabled = true;
        NPCText.SetActive(true);
    }
    private void OnMouseExit()
    {
        outline.enabled = false;
        NPCText.SetActive(false);
    }

    private void OnMouseDown()
    {
        OpenPanel();
    }
    private void OpenPanel()
    {
        if (npcType == NPCType.Shop)
        {
            UIPanel.SetActive(true);
        }
        else if (npcType == NPCType.Combat)
        {
            CombatPanel.SetActive(true);
        }
    }
    private void ClosePanel()
    {
        UIPanel.SetActive(false);
    }

    public enum NPCType
    {
        Shop,
        Combat,
    }
}
