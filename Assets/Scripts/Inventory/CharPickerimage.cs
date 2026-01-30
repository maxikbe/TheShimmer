using UnityEngine;
using UnityEngine.UI; 

public class CharPickerimage : MonoBehaviour
{
    [SerializeField] private Sprite imagePostavy; 
    private Image komponentaImage; 

    void Awake()
    {
        komponentaImage = GetComponent<Image>();
    }

    void Start()
    {
        if (komponentaImage != null && imagePostavy != null)
        {
            komponentaImage.sprite = imagePostavy;
        }
    }
}
