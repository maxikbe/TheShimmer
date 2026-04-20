using UnityEngine;
using TMPro;

public class LocationGetter : MonoBehaviour
{
    public TextMeshProUGUI locationText;
    public string defaultText = "Wilderness";
    
    private TextMeshProUGUI EnterinPopUpText;
    private Animator EnterinPopUpAnimator;
    private GameObject EnterinPopUp;

    private void Start()
    {
        if (locationText == null) locationText = GetComponent<TextMeshProUGUI>();
        if (locationText != null) locationText.text = defaultText;
        EnterinPopUp = gameObject.transform.Find("PlayerInfoUICanvas/EnterinPopUp").gameObject;
        EnterinPopUpText = EnterinPopUp.GetComponentInChildren<TextMeshProUGUI>();
        EnterinPopUpAnimator = EnterinPopUp.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        LocationSetter location = other.GetComponent<LocationSetter>();
        if (location == null) location = other.GetComponentInParent<LocationSetter>();

        if (location != null)
        {
            if (locationText != null) locationText.text = location.locationName;

            if (EnterinPopUpText != null) EnterinPopUpText.text = "Entering:\n" + location.locationName;
            if (EnterinPopUp != null) EnterinPopUp.SetActive(true);
            EnterinPopUpAnimator.speed = 0.5f;
            if (EnterinPopUpAnimator != null) EnterinPopUpAnimator.Play("LocationPopUp", -1, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<LocationSetter>() != null || other.GetComponentInParent<LocationSetter>() != null)
        {
            if (locationText != null) locationText.text = defaultText;
        }

        LocationSetter location = other.GetComponent<LocationSetter>();
        if (location == null) location = other.GetComponentInParent<LocationSetter>();

        if (location != null)
        {
            if (locationText != null) locationText.text = location.locationName;

            if (EnterinPopUpText != null) EnterinPopUpText.text = "Leaving:\n" + location.locationName;
            if (EnterinPopUp != null) EnterinPopUp.SetActive(true);
            EnterinPopUpAnimator.speed = 0.5f;
            if (EnterinPopUpAnimator != null) EnterinPopUpAnimator.Play("LocationPopUp", -1, 0f);
        }
    }
}