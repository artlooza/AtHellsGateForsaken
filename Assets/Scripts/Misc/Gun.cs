using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float range = 20f;
    public float verticalRange = 20f;
    public float gunShotRadius = 20f;

    public float bigDamage = 2f;
    public float smallDamage = 1f;

    public float fireRate = 1f; // bullets per second
    private float nextTimeToFire;

    public int maxAmmo;
    private int ammo;

    public LayerMask raycastLayerMask;
    public LayerMask enemyLayerMask;

    private BoxCollider gunTrigger;
    public EnemyManager enemyManager;

    public Animator gunAnim; // Assign your shotgun's Animator in Inspector
    public PlayerMove playerMove; // Assign the player to check movement state

    private bool isShooting = false;
    private float shootAnimDuration = 0.5f; // Adjust to match your shooting animation length
    private float shootAnimEndTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunTrigger = GetComponent<BoxCollider>();
        gunTrigger.size = new Vector3(1, verticalRange, range);
        gunTrigger.center = new Vector3(0, 0, range * .5f);

        ammo = maxAmmo;

        // Auto-find PlayerMove if not assigned
        if (playerMove == null)
        {
            playerMove = FindFirstObjectByType<PlayerMove>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if shooting animation has finished
        if (isShooting && Time.time >= shootAnimEndTime)
        {
            isShooting = false;
        }

        // Handle shooting
        if (Input.GetMouseButton(0) && Time.time > nextTimeToFire && ammo > 0)
        {
            Fire();
        }
        // Handle walking animation (only when not shooting)
        else if (gunAnim != null && !isShooting)
        {
            bool isMoving = playerMove != null &&
                (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                 Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));

            if (isMoving)
            {
                // Play walking animation if not already playing
                if (!gunAnim.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
                {
                    gunAnim.Play("Walking", 0, 0f);
                }
            }
            else
            {
                // Return to idle when not moving
                if (!gunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shotgun|Idle"))
                {
                    gunAnim.Play("Shotgun|Idle", 0, 0f);
                }
            }
        }
    }

    void Fire()
    {
        // simulate gun shot radius
        Collider[] enemyColliders;
        enemyColliders = Physics.OverlapSphere(transform.position, gunShotRadius, enemyLayerMask);

        // alert any enemy in earshot
        foreach (var enemycollider in enemyColliders)
        {
            // Check for regular enemy awareness
            EnemyAwareness awareness = enemycollider.GetComponent<EnemyAwareness>();
            if (awareness != null)
            {
                awareness.isAggro = true;
                continue;
            }

            // Check for Boss awareness
            Boss boss = enemycollider.GetComponent<Boss>();
            if (boss != null)
            {
                boss.isAggro = true;
            }
        }

        //play test audio
        GetComponent<AudioSource>().Stop();

        // Play shooting animation
        if (gunAnim != null)
        {
            gunAnim.Play("Shooting", 0, 0f);
            isShooting = true;
            shootAnimEndTime = Time.time + shootAnimDuration;
        }
        GetComponent<AudioSource>().Play();





        // damage enemies
        foreach (var enemy in enemyManager.enemiesInTrigger)
        {
            var dir = enemy.transform.position - transform.position;
            RaycastHit hit;
            Vector3 rayStart = transform.position + transform.forward * 1f; // Start ray in front of gun
            if (Physics.Raycast(rayStart, dir, out hit, range * 1.5f, raycastLayerMask))
            {
                if(hit.transform == enemy.transform)
                {
                    //range check
                    float dist = Vector3.Distance(enemy.transform.position, transform.position);

                    if (dist > range * .5f)
                    {
                        //damage enemy small. Damages enemy if they're far with smaller damage.
                        enemy.TakeDamage(smallDamage);


                    }
                    else {
                        //damage enemy big. Damages enemy if they're close with full damage.
                        enemy.TakeDamage(bigDamage);
                    }

                    // Show hit marker on reticle
                    CanvasManager.Instance.ShowHitMarker();

                    //Debug.DrawRay(transform.position, dir, Color.green);
                    //Debug.Break();
                }

            }


        }

        // damage bosses
        //Debug.Log($"[GUN] Bosses in trigger: {enemyManager.bossesInTrigger.Count}");
        foreach (var boss in enemyManager.bossesInTrigger)
        {
            Debug.Log($"[GUN] Attempting to shoot boss: {boss.name}");
            var dir = boss.transform.position - transform.position;
            RaycastHit hit;
            Vector3 rayStart = transform.position + transform.forward * 1f; // Start ray in front of gun
            if (Physics.Raycast(rayStart, dir, out hit, range * 1.5f, raycastLayerMask))
            {
                //Debug.Log($"[GUN] Raycast hit: {hit.transform.name} (Boss transform: {boss.transform.name})");
                if(hit.transform == boss.transform)
                {
                    //range check
                    float dist = Vector3.Distance(boss.transform.position, transform.position);

                    if (dist > range * .5f)
                    {
                        //damage boss small. Damages boss if they're far with smaller damage.
                        boss.TakeDamage(smallDamage, transform.position);
                    }
                    else {
                        //damage boss big. Damages boss if they're close with full damage.
                        boss.TakeDamage(bigDamage, transform.position);
                    }

                    // Show hit marker on reticle
                    CanvasManager.Instance.ShowHitMarker();
                }
                else
                {
                    Debug.Log($"[GUN] Raycast hit wrong object! Hit {hit.transform.name} instead of {boss.transform.name}");
                }
            }
            else
            {
                Debug.Log($"[GUN] Raycast missed! Check raycastLayerMask or Boss layer");
            }
        }

        //reset timer
        nextTimeToFire = Time.time + fireRate;

        // deduct ammo
        ammo--;
        CanvasManager.Instance.UpdateAmmo(ammo);
    }


    public void GiveAmmo(int amount, GameObject pickup)
    {
        ammo += amount;
        if (ammo < maxAmmo)
        {
            ammo += maxAmmo;
            Destroy(pickup);
        }
       
        if(ammo > maxAmmo)
        {
            ammo = maxAmmo;

        }
        CanvasManager.Instance.UpdateAmmo(ammo);
    }

    private void OnTriggerEnter(Collider other)
    {
        // add enemy to shoot
        Enemy enemy = other.GetComponent<Enemy>();
        if(enemy)
        {
            enemyManager.AddEnemy(enemy);
        }

        // Add boss to shoot list
        Boss boss = other.GetComponent<Boss>();
        if (boss)
        {
            //Debug.Log($"[GUN] Boss entered trigger: {boss.name}");
            enemyManager.AddBoss(boss);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // remove enemy from shoot list
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy)
        {
            enemyManager.RemoveEnemy(enemy);
        }

        // Remove boss from shoot list
        Boss boss = other.GetComponent<Boss>();
        if (boss)
        {
            enemyManager.RemoveBoss(boss);
        }
    }


}
