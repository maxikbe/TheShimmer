using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Linq;

public class PlayerRealTimeCombat : MonoBehaviour
{
    public enum WeaponType { Machete, Gun }

    [Header("Current Status")]
    public WeaponType currentWeapon = WeaponType.Machete;
    public int currentAmmo = 10; // Budeš lootit

    [Header("Machete Settings")]
    public int macheteDamage = 5;
    public float macheteRange = 1.5f;
    [Range(0, 360)] public float macheteAngle = 150f;
    public float macheteCooldown = 0.5f;
    public GameObject macheteSwipeEffect; // Prefab pro ten "švih"

    [Header("Gun Settings")]
    public int gunDamage = 10;
    public float gunRange = 8f;
    [Range(0, 360)] public float gunAngle = 120f;
    public float gunCooldown = 0.8f;
    public GameObject bulletTrailEffect; // LineRenderer nebo prefab pro trasu kulky

    [Header("Aiming & Visuals")]
    public Transform firePoint; 
    public SpriteRenderer aimConeVisual; // Grafika kuželu (Sprite s poloprůhlednou barvou)
    public Color validAimColor = new Color(1f, 1f, 1f, 0.3f);
    public Color invalidAimColor = new Color(1f, 0f, 0f, 0.3f);
    public LayerMask hitLayers; // Zdi, stromy, mobky
    public LayerMask enemyLayer; // Jenom mobky

    [Header("UI Weapon Cards")]
    public Image activeWeaponCard;
    public Image inactiveWeaponCard;
    public Sprite macheteIcon;
    public Sprite gunIcon;

    private float nextAttackTime = 0f;
    private Camera mainCam;
    private PlayerMovement playerMovement;

    void Start()
    {
        mainCam = Camera.main;
        playerMovement = GetComponent<PlayerMovement>();
        UpdateWeaponUI();
    }

    void Update()
    {
        HandleWeaponSwitch();
        HandleAimingVisuals();

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentWeapon != WeaponType.Machete)
        {
            currentWeapon = WeaponType.Machete;
            UpdateWeaponUI();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && currentWeapon != WeaponType.Gun)
        {
            currentWeapon = WeaponType.Gun;
            UpdateWeaponUI();
        }
    }

    private void UpdateWeaponUI()
    {
        // Klasické prohazování kartiček vpravo dole
        if (currentWeapon == WeaponType.Machete)
        {
            activeWeaponCard.sprite = macheteIcon;
            inactiveWeaponCard.sprite = gunIcon;
        }
        else
        {
            activeWeaponCard.sprite = gunIcon;
            inactiveWeaponCard.sprite = macheteIcon;
        }
    }

    private void HandleAimingVisuals()
    {
        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = (mouseWorldPos - (Vector2)transform.position).normalized;
        Vector2 facingDirection = playerMovement.GetFacingDirection();

        // Výpočet úhlu mezi tím, kam jdeš (WASD) a kam míříš myší
        float angleToMouse = Vector2.Angle(facingDirection, aimDirection);
        float allowedAngle = currentWeapon == WeaponType.Machete ? macheteAngle / 2f : gunAngle / 2f;
        float currentRange = currentWeapon == WeaponType.Machete ? macheteRange : gunRange;

        // Natočení vizuálního kuželu za myší
        if (aimConeVisual != null)
        {
            float rotZ = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            aimConeVisual.transform.rotation = Quaternion.Euler(0, 0, rotZ - 90f);
            
            // Škálování podle dosahu zbraně
            aimConeVisual.transform.localScale = new Vector3(currentRange, currentRange, 1f);

            // Zbarvení podle toho, jestli míříš "za sebe" nebo do povoleného úhlu
            if (angleToMouse > allowedAngle)
                aimConeVisual.color = invalidAimColor; // Červená - nemůžeš střílet
            else
                aimConeVisual.color = validAimColor;   // Bílá/Zelená - vše OK
        }
    }

    private void Attack()
    {
        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = (mouseWorldPos - (Vector2)transform.position).normalized;
        Vector2 facingDirection = playerMovement.GetFacingDirection();

        float allowedAngle = currentWeapon == WeaponType.Machete ? macheteAngle / 2f : gunAngle / 2f;
        
        // Pokud klikneš mimo výseč, útok se neprovede
        if (Vector2.Angle(facingDirection, aimDirection) > allowedAngle)
        {
            Debug.Log("Kokkotte, míříš si na vlastní záda! Srovnej si to WASDčkem.");
            return; // Zastaví útok
        }

        if (currentWeapon == WeaponType.Machete)
        {
            MeleeAttack(aimDirection);
        }
        else
        {
            if (currentAmmo > 0)
                RangedAttack(aimDirection);
            else
                Debug.Log("Došly náboje, zkus házet kamení.");
        }
    }

    private void MeleeAttack(Vector2 aimDirection)
    {
        nextAttackTime = Time.time + macheteCooldown;

        // Vizuál švihu
        if (macheteSwipeEffect != null)
        {
            float rotZ = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            Instantiate(macheteSwipeEffect, transform.position, Quaternion.Euler(0, 0, rotZ));
        }

        // Projedeme všechno v dosahu
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, macheteRange);
        foreach (var hit in hits)
        {
            Mob_combat mob = hit.GetComponent<Mob_combat>();
        
            if (mob != null && mob.canBeHitInRealTime) 
            {
                Vector2 dirToEnemy = (hit.transform.position - transform.position).normalized;
                if (Vector2.Angle(aimDirection, dirToEnemy) <= macheteAngle / 2f)
                {
                    if (!Physics2D.Raycast(transform.position, dirToEnemy, Vector2.Distance(transform.position, hit.transform.position), hitLayers))
                    {
                        mob.TakeDamage(macheteDamage);
                    
                        // NOVÉ: Kontrola na spuštění tahovky
                        if (mob.startTurnBaseAfterHit)
                        {
                            Debug.Log($"[MELEE] Kokkott sekl do {mob.gameObject.name}! Obrazovka se tříští, spouštím Turn-Based Combat!");
                        }
                    }
                }
            }
        }
    }

    private void RangedAttack(Vector2 aimDirection)
    {
        nextAttackTime = Time.time + gunCooldown;
        currentAmmo--;

        // Střela proletí vším, seřadíme od nejbližšího
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, aimDirection, gunRange);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
    
        Vector2 endPos = (Vector2)firePoint.position + (aimDirection * gunRange);

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;

            Mob_combat mob = hit.collider.GetComponent<Mob_combat>();
        
            if (mob != null && mob.canBeHitInRealTime)
            {
                mob.TakeDamage(gunDamage);
                endPos = hit.point;
            
                // NOVÉ: Kontrola na spuštění tahovky
                if (mob.startTurnBaseAfterHit)
                {
                    Debug.Log($"[RANGED] BUM! Trefils {mob.gameObject.name}! Cue the Final Fantasy battle music, jdeme do tahovky!");
                }
            
                break; // Kulka končí v mobce
            }
        
            if (((1 << hit.collider.gameObject.layer) & hitLayers) != 0) 
            {
                endPos = hit.point;
                break; // Kulka končí ve zdi
            }
        }

        // Vykreslení trailu (stopy kulky)
        if (bulletTrailEffect != null)
        {
            GameObject trail = Instantiate(bulletTrailEffect, firePoint.position, Quaternion.identity);
            LineRenderer lr = trail.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.SetPosition(0, firePoint.position);
                lr.SetPosition(1, endPos);
            }
        }
    }
}