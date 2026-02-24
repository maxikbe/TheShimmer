using UnityEngine;

public class PlayerHitTesting : MonoBehaviour
{
    [Header("Přetáhni sem mobku z hierarchie!")]
    public Mob_combat targetMob;

    [Header("Nastavení útoku")]
    public float attackCooldown = 1f; // Jak dlouho (v sekundách) musíš čekat do další rány
    private float nextAttackTime = 0f; // Naše interní stopky

    void Update()
    {
        // Když zmáčkneš H...
        if (Input.GetKeyDown(KeyCode.H))
        {
            // ...zkontrolujeme, jestli už uplynul čas do dalšího možného útoku
            if (Time.time >= nextAttackTime)
            {
                if (targetMob != null)
                {
                    Debug.Log("BAM! Instantní hit!");
                    targetMob.TakeDamage(5);

                    // Nyní resetujeme stopky. 
                    // Time.time je aktuální čas od spuštění hry. Přičteme k němu tvůj cooldown.
                    nextAttackTime = Time.time + attackCooldown;
                }
                else
                {
                    Debug.LogWarning("Kokkotte, zase nemáš přiřazenou mobku v Inspectoru!");
                }
            }
            else
            {
                // Tohle se stane, když se snažíš tlačítko spamovat moc rychle
                Debug.Log("Zbraň má ještě cooldown, brzdi!");
            }
        }
    }
}