using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGun : MonoBehaviour
{

[System.Serializable]
public class RoboGun {
    public Transform gunObject; // in scene, not prefab
    public GameObject bulletPrefab;
    public float bulletSpeed = 1; // set grav + drag in bullet prefab
    public float cooldown = 0.2f;
    public float bulletsPerShot = 1;
    public float angleVariation = 0;
    public float addRoboVelocity = 1;
    public float recoilVelocityScale = 0.2f;
    public float energyCost = 0.05f;

    public void Shoot(Transform bulletParent) {
        Transform tip = gunObject.Find("GunTip");
        Vector2 shootDirection = (tip.position - gunObject.position).normalized;
        Rigidbody2D robot = gunObject.root.GetComponent<Rigidbody2D>();

        for (int i = 0; i < bulletsPerShot; i++) {
            GameObject bullet = Instantiate(bulletPrefab, tip.position, Quaternion.identity, bulletParent);
            Vector2 direction = shootDirection;
            if (angleVariation != 0) {
                float offsetAngle = (2*Random.value-1) * angleVariation;
                float angle = Vector2.SignedAngle(Vector2.right, direction);
                angle += offsetAngle;
                angle *= Mathf.Deg2Rad;
                direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }
            Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();
            brb.velocity = addRoboVelocity * robot.velocity;
            brb.velocity += bulletSpeed * direction;
            robot.velocity -= recoilVelocityScale * brb.velocity;
        }
    }
}

}
