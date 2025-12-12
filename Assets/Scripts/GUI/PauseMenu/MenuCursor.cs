using UnityEngine;

public class MenuCursor : MonoBehaviour
{
    public static MenuCursor Instance { get; private set; }
    
    public Texture2D defaultMenuCursor;
    private readonly Vector2 hotSpot = Vector2.zero;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        if (defaultMenuCursor != null)
        {
            Cursor.SetCursor(defaultMenuCursor, hotSpot, CursorMode.Auto);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}