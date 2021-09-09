using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Unlocker : RobotGun
{

    void OnTriggerEnter2D(Collider2D info) {
        GameObject other = info.gameObject;
        RobotMovement rm = other.GetComponent<RobotMovement>();
        if (rm == null)
            rm = other.GetComponentInParent<RobotMovement>();
        RobotShooting rs = other.GetComponent<RobotShooting>();
        if (rs == null)
            rs = other.GetComponentInParent<RobotShooting>();
            
        if (rm == null || rs == null) return;
        if (!rs.unlockedGuns.Contains(gun) && gun.gunObject != null)
            rs.AddGun(gun);
        rm.canJump |= canJump;
        rm.canWallJump |= canWallJump;
        rm.canWallSlide |= canWallSlide;
        rm.canDash |= canDash;
        rm.canSlam |= canSlam;
        rm.canJetpack |= canJetpack;
        rm.jumpCharges += jumpCharges;
        Destroy(gameObject);
    }

    public RoboGun gun; // will initially contain a prefab for gun
    public bool canJump = false;
    public bool canWallJump = false;
    public bool canWallSlide = false;
    public bool canDash = false;
    public bool canSlam = false;
    public bool canJetpack = false;
    public int jumpCharges = 0;
    
}
