using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Linq;
using TMPro;

public class PlayerRealTimeCombat : MonoBehaviour
{
    public enum WeaponType { Machete, Gun }

    [Header("Current Status")]
    public WeaponType currentWeapon = WeaponType.Machete;
    public int currentAmmo = 10; // Budeš lootit
    public bool isWeaponEquipped = false;
    

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
    
    [Header("UI Ammo Settings")]
    public TextMeshProUGUI ammoText;
    
    [Header("Shoot Visuals")]
    public Color shootFlashColor = new Color(1f, 0.9f, 0f, 0.5f); // Žlutá/zlatá
    public float flashDuration = 0.05f; // Jak dlouho to problikne (50 ms je tak akorát)

    private float currentFlashTime = 0f; // Vnitřní časovač
    
    

    [Header("Aiming & Visuals")]
    public Transform firePoint; 
    public Color validAimColor = new Color(1f, 1f, 1f, 0.15f);
    public Color invalidAimColor = new Color(1f, 0f, 0f, 0.25f);
    public Color laserColor = Color.red;
    public LayerMask hitLayers;

    [Header("UI Weapon Cards")]
    public Image activeWeaponCard;
    public Image inactiveWeaponCard;
    public Sprite macheteIcon;
    public Sprite gunIcon;
    // NOVÉ: Barvy pro ztmavení kartiček
    public Color activeCardColor = Color.white;
    public Color inactiveCardColor = new Color(0.6f, 0.6f, 0.6f, 1f); 
    public Color holsteredCardColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Hodně tmavá a průhledná

    private float nextAttackTime = 0f;
    private Camera mainCam;
    private PlayerMovement playerMovement;

    // Dynamicky generované vizuály
    private MeshFilter aimMeshFilter;
    private MeshRenderer aimMeshRenderer;
    private LineRenderer laserSight;

    void Start()
    {
        mainCam = Camera.main;
        playerMovement = GetComponent<PlayerMovement>();
        
        SetupProceduralVisuals(); // Vygeneruje nám to vizuály
        UpdateWeaponUI();
    }

    void Update()
    {
        HandleWeaponSwitch();
        HandleAimingVisuals();

        // Přidali jsme podmínku isWeaponEquipped, aby nešlo útočit s holou rukou
        if (isWeaponEquipped && Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Pokud už máš mačetu a je vytažená, tak ji schováš
            if (currentWeapon == WeaponType.Machete && isWeaponEquipped)
            {
                isWeaponEquipped = false;
            }
            else // Jinak ji vytáhneš
            {
                currentWeapon = WeaponType.Machete;
                isWeaponEquipped = true;
            }
            UpdateWeaponUI();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // To samé pro gunu
            if (currentWeapon == WeaponType.Gun && isWeaponEquipped)
            {
                isWeaponEquipped = false;
            }
            else
            {
                currentWeapon = WeaponType.Gun;
                isWeaponEquipped = true;
            }
            UpdateWeaponUI();
        }
    }

    private void UpdateWeaponUI()
    {
        // Nastavení ikonky (stávající kód)
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

        // Barvy kartiček (stávající kód)
        if (!isWeaponEquipped)
        {
            activeWeaponCard.color = holsteredCardColor;
            inactiveWeaponCard.color = holsteredCardColor;
            if (ammoText != null) ammoText.enabled = false; // Schováme náboje, když je vše v "pouzdře"
        }
        else
        {
            activeWeaponCard.color = activeCardColor;
            inactiveWeaponCard.color = inactiveCardColor;

            // LOGIKA NÁBOJŮ:
            if (currentWeapon == WeaponType.Gun)
            {
                if (ammoText != null)
                {
                    ammoText.enabled = true;
                    ammoText.text = currentAmmo.ToString(); // Ukáže aktuální počet
                }
            }
            else
            {
                // Pokud máš mačetu, UI s náboji prostě "vypneš"
                if (ammoText != null) ammoText.enabled = false;
            }
        }
    }
    
    private void SetupProceduralVisuals()
    {
        // 1. Vygenerování objektu pro kužel/trojúhelník
        GameObject meshObj = new GameObject("ProceduralAimCone");
        meshObj.transform.SetParent(firePoint);
        meshObj.transform.localPosition = Vector3.zero;
        
        aimMeshFilter = meshObj.AddComponent<MeshFilter>();
        aimMeshRenderer = meshObj.AddComponent<MeshRenderer>();
        // Sprites/Default materiál podporuje průhlednost
        aimMeshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        aimMeshRenderer.sortingOrder = -1; // Ať to nekreslí přes hráče

        // 2. Vygenerování Laseru
        GameObject laserObj = new GameObject("LaserSight");
        laserObj.transform.SetParent(firePoint);
        laserObj.transform.localPosition = Vector3.zero;
        
        laserSight = laserObj.AddComponent<LineRenderer>();
        laserSight.material = new Material(Shader.Find("Sprites/Default"));
        laserSight.startColor = laserColor;
        laserSight.endColor = laserColor;
        laserSight.startWidth = 0.02f;
        laserSight.endWidth = 0.02f;
        laserSight.sortingOrder = 1;
        laserSight.positionCount = 2;
    }

    private void HandleAimingVisuals()
    {
        // NOVÉ: Pokud není zbraň vytažená, vypneme renderery a opustíme funkci
        if (!isWeaponEquipped)
        {
            if (aimMeshRenderer != null) aimMeshRenderer.enabled = false;
            if (laserSight != null) laserSight.enabled = false;
            return;
        }

        // Zbraň je v ruce, takže musíme zajistit, že je renderer zapnutý
        if (aimMeshRenderer != null) aimMeshRenderer.enabled = true;
        
        
        // Myš převedeme a rovnou zařízneme osu Z na nulu, ať nám laser nelítá do 3D prostoru
        Vector3 rawMousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseWorldPos = new Vector3(rawMousePos.x, rawMousePos.y, 0f);
    
        Vector2 aimDirection = (mouseWorldPos - firePoint.position).normalized;
        Vector2 facingDirection = playerMovement.GetFacingDirection(); // Směr z WASD

        float allowedAngle = currentWeapon == WeaponType.Machete ? macheteAngle : gunAngle;
        float currentRange = currentWeapon == WeaponType.Machete ? macheteRange : gunRange;
        int segments = currentWeapon == WeaponType.Machete ? 20 : 1; 

        // Pro mačetu je míření vždy OK, řešíme myš jen u zbraně
        bool isAimValid = true;
        if (currentWeapon == WeaponType.Gun)
        {
            float angleToMouse = Vector2.Angle(facingDirection, aimDirection);
            isAimValid = angleToMouse <= (allowedAngle / 2f);
        }

        Color currentColor = isAimValid ? validAimColor : invalidAimColor;

        // NOVÉ: Kontrola, jestli zrovna střílíme a máme blikat
        if (currentFlashTime > 0f)
        {
            currentColor = shootFlashColor; // Přepíšeme barvu na flash
            currentFlashTime -= Time.deltaTime; // Odečítáme čas
        }

        // Vykreslení kuželu vždy podle směru postavy
        DrawProceduralCone(currentRange, allowedAngle, facingDirection, currentColor, segments);

        // Laser pro zbraň
        if (currentWeapon == WeaponType.Gun && isAimValid)
        {
            laserSight.enabled = true;
            laserSight.SetPosition(0, firePoint.position);
        
            // Zastropujeme laser na max range zbraně, jinak končí PŘESNĚ na kurzoru
            float distToMouse = Vector2.Distance(firePoint.position, mouseWorldPos);
            if (distToMouse > gunRange)
            {
                laserSight.SetPosition(1, firePoint.position + (Vector3)(aimDirection * gunRange));
            }
            else
            {
                laserSight.SetPosition(1, mouseWorldPos);
            }
        }
        else
        {
            laserSight.enabled = false;
        }
    }
    
    // Matematická magie - generování 2D geometrie
    private void DrawProceduralCone(float range, float angle, Vector2 direction, Color color, int segments)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero; // Střed je firePoint

        // Získání rotace hráče ve stupních
        float facingAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angleStep = angle / segments;
        float currentAngle = -angle / 2f; // Začínáme od "spodní" hrany kuželu

        for (int i = 0; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (currentAngle + facingAngle);
            vertices[i + 1] = new Vector3(Mathf.Cos(rad) * range, Mathf.Sin(rad) * range, 0);
            currentAngle += angleStep;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        aimMeshFilter.mesh = mesh;
        aimMeshRenderer.material.color = color;
    }

    private void Attack()
    {
        Vector3 rawMousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseWorldPos = new Vector2(rawMousePos.x, rawMousePos.y);
    
        Vector2 aimDirection = (mouseWorldPos - (Vector2)firePoint.position).normalized;
        Vector2 facingDirection = playerMovement.GetFacingDirection(); // Směr WASD

        if (currentWeapon == WeaponType.Machete)
        {
            // Mačeta seká před panáčka, ignorujeme kurzor
            MeleeAttack(facingDirection);
        }
        else
        {
            // Pistole míří podle kurzoru
            if (Vector2.Angle(facingDirection, aimDirection) > gunAngle / 2f)
            {
                Debug.Log("Kokkotte, míříš mimo trojúhelník! Srovnej si to WASDčkem.");
                return; 
            }

            if (currentAmmo > 0)
                RangedAttack(aimDirection);
            else
                Debug.Log("Cvak. Cvak. Došly náboje!");
        }
    }

    private void MeleeAttack(Vector2 attackDirection) // Sem teď teče směr z WASD
    {
        nextAttackTime = Time.time + macheteCooldown;

        // Vizuál švihu natočíme tam, kam jdeš
        if (macheteSwipeEffect != null)
        {
            float rotZ = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            Instantiate(macheteSwipeEffect, transform.position, Quaternion.Euler(0, 0, rotZ));
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, macheteRange);
        foreach (var hit in hits)
        {
            Mob_combat mob = hit.GetComponent<Mob_combat>();
        
            if (mob != null && mob.canBeHitInRealTime) 
            {
                Vector2 dirToEnemy = (hit.transform.position - transform.position).normalized;
            
                // Úhel porovnáváme proti směru pohybu, ne myši
                if (Vector2.Angle(attackDirection, dirToEnemy) <= macheteAngle / 2f)
                {
                    if (!Physics2D.Raycast(transform.position, dirToEnemy, Vector2.Distance(transform.position, hit.transform.position), hitLayers))
                    {
                        mob.TakeDamage(macheteDamage);
                    
                        if (mob.startTurnBaseAfterHit)
                        {
                            Debug.Log($"[MELEE] Rozsekl jsi {mob.gameObject.name}! Jdeme do tahovky!");
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
        
        currentFlashTime = flashDuration;
        
        // Aktualizace UI po každém výstřelu
        if (ammoText != null) ammoText.text = currentAmmo.ToString();

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
    }
}