using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    [SerializeField] float fireRate;
    float shotCooldown;
    bool reloading = false;
    // Start is called before the first frame update
    void Start()
    {
        shotCooldown = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (reloading) {
            shotCooldown -= Time.fixedDeltaTime;
            if (shotCooldown <= 0) {
                reloading = false;
                shotCooldown = fireRate;
            }
        }
    }

    public void Shoot(Vector2 direction) {
        if (reloading) return;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
        reloading = true;
    }
}
