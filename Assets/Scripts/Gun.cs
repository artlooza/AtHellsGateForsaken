using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float range = 20f;
    public float verticalRange = 20f;
    public float fireRate = 1f; // bullets per second
    public float bigDamage = 2f;
    public float smallDamage = 1f;
    private float nextTimeToFire;

    private BoxCollider gunTrigger;

    public EnemyManager enemyManager;
    
    public LayerMask raycastLayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunTrigger = GetComponent<BoxCollider>();
        gunTrigger.size = new Vector3(1, verticalRange, range);
        gunTrigger.center = new Vector3(0, 0, range * .5f);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > nextTimeToFire)
        {
            Fire();
        }

    }

    void Fire()
    {
        // damage enemies
        foreach(var enemy in enemyManager.enemiesInTrigger)
        {
            var dir = enemy.transform.position - transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dir, out hit, range * 1.5f, raycastLayerMask))
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


        

                    //Debug.DrawRay(transform.position, dir, Color.green);
                    //Debug.Break();
                }

            }


        }

        //reset timer
        nextTimeToFire = Time.time + fireRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        // add enemy enemy to shoot
        Enemy enemy = other.GetComponent<Enemy>();

        if(enemy)
        {
            enemyManager.AddEnemy(enemy);

        }

    }

    private void OnTriggerExit(Collider other)
    {
        // remove enemy enemy to shoot
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy)
        {
            enemyManager.RemoveEnemy(enemy);

        }
    }
}
