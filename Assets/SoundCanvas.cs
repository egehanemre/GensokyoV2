using UnityEngine;

public class SoundCanvas : MonoBehaviour
{
    public static SoundCanvas Instance { get; private set; }

    private Canvas canvas;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get the Canvas component (if attached to this GameObject)
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            // Try to find a Canvas in children if not on this GameObject
            canvas = GetComponentInChildren<Canvas>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (canvas != null)
        {
            canvas.enabled = !canvas.enabled;
        }
        else
        {
            // If no Canvas component, toggle the GameObject itself
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}