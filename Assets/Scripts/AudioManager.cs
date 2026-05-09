using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource2D;

    [Header("UI Zvuky (2D)")]
    [SerializeField] private AudioClip clickSound;

    [Header("Světové Zvuky (Falešné 3D)")]
    [SerializeField] private AudioClip chestOpenSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource2D = GetComponent<AudioSource>();
        audioSource2D.spatialBlend = 0f; 
        audioSource2D.playOnAwake = false; // Pojistka, kdybys to v Inspectoru zapomněl odškrtnout
    }

    // 1. Zvuk v menu (využívá tvůj FinalSfxVolume z GameSettings)
    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            // Přidali jsme druhý parametr: hlasitost z tvého nastavení!
            audioSource2D.PlayOneShot(clickSound, GameSettings.FinalSfxVolume);
        }
    }

    // 2. Zvuk ve světě (Falešné 3D pro tvou izometrickou top-down kameru)
    public void PlayChestOpenSound(Vector3 position)
    {
        if (chestOpenSound != null)
        {
            // Zjistíme, kde visí kamera
            float cameraZ = Camera.main.transform.position.z;

            // Srovnáme Z-osu s kamerou, aby zvuk nezněl utopeně
            Vector3 fixedPos = new Vector3(position.x, position.y, cameraZ);

            // PlayClipAtPoint umí brát hlasitost jako 3. parametr!
            AudioSource.PlayClipAtPoint(chestOpenSound, fixedPos, GameSettings.FinalSfxVolume);
        }
    }
}