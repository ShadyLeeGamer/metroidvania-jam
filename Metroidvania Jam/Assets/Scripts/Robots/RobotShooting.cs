using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotShooting : MonoBehaviour
{
	RobotAnimations anim;
	Inputs inputs;
	Transform projectileParent;
    void Start() {
    	anim = GetComponent<RobotAnimations>();
    	inputs = GetComponent<Inputs>();
    	projectileParent = GameObject.Find("Environment").transform.Find("Projectiles");
    }

    public Transform hookGun; // in scene
    public float hookCooldown = 0.3f;
    public List<RoboGun> unlockedGuns;
    int currentGunIndex = -1; // 0,1,etc for index in unlockedGuns, otherwise hook
    void DisplayGun(bool active) {
    	if (currentGunIndex == -1)
    		hookGun.gameObject.SetActive(active);
    	else
    		unlockedGuns[currentGunIndex].gunObject.gameObject.SetActive(active);
    }
    public void CycleGun() {
    	DisplayGun(false);
    	currentGunIndex++;
    	if (currentGunIndex >= unlockedGuns.Count)
    		currentGunIndex = -1;
    	DisplayGun(true);
    }


    public Vector2[] shootDirections = new Vector2[8] {
    	new Vector2(1, -0.05f), // right
    	new Vector2(1, 1), // up/right
    	new Vector2(0, 1), // up
    	new Vector2(-1, 1), // up/left
    	new Vector2(-1, -0.05f), // left
    	new Vector2(-0.7f, -1f), // down/left
    	Vector2.zero, // down
    	new Vector2(0.7f, -1f), // down/right
    };
    Vector2 CalculateDirection(bool right, bool up, bool left, bool down) {
    	if (right && left) return Vector2.zero;
    	if (up && down) return Vector2.zero;

    	if (right && up) return shootDirections[1];
    	if (left && up) return shootDirections[3];
    	if (left && down) return shootDirections[5];
    	if (right && down) return shootDirections[7];

    	if (right) return shootDirections[0];
    	if (up) return shootDirections[2];
    	if (left) return shootDirections[4];
    	return Vector2.zero;
    }
    Vector2 CalculateDirection(Vector2 direction) {
    	// x: index (float)
    	// y: distance
    	Vector2 min = new Vector2(0, Vector2.Distance(direction.normalized, shootDirections[0].normalized));
    	for (int i = 1; i < shootDirections.Length; i++) {
    		float distance = Vector2.Distance(direction.normalized, shootDirections[i].normalized);
    		if (distance < min.y) {
    			min = new Vector2(i, distance);
    		}
    	}
    	return shootDirections[Mathf.RoundToInt(min.x)];
    }
    
    public float poolInputsTime = 0.2f;
    float poolTimer = 0;
    bool[] pooledInputs = new bool[4] { false, false, false, false };
    void ResetPooled() {
    	pooledInputs = new bool[4] { false, false, false, false };
    }
    void PoolInputs() {
    	pooledInputs[0] |= inputs.ShootRight;
    	pooledInputs[1] |= inputs.ShootUp;
    	pooledInputs[2] |= inputs.ShootLeft;
    	pooledInputs[3] |= inputs.ShootDown;
    }
    float shootCooldown = 0;
    // todo: fix facing, add charging anim, move transfer anim, mouse, default point direction
    void Update() {
    	//Debug.Log(pooledInputs[0] + " " + pooledInputs[1] + " " + pooledInputs[2] + " " + pooledInputs[3]);
    	PoolInputs();
    	if (shootCooldown > 0)
    		shootCooldown -= Time.deltaTime;
    	else if (canShoot)
    		Fire();

    	if (poolTimer < poolInputsTime)
    		poolTimer += Time.deltaTime;
    	else {
    		poolTimer = 0;
    		canShoot = false;
    		CoordinateShooting();
    		ResetPooled();
    	}
    }
    Vector2 oldCursor = Vector2.zero;
    Vector2 oldDirection = Vector2.zero;
    bool hookPrimed = false;
    void CoordinateShooting() {
    	// Calculate shoot direction
    	Vector2 direction = CalculateDirection(pooledInputs[0], pooledInputs[1], pooledInputs[2], pooledInputs[3]);
    	if (direction == Vector2.zero) {
    		// Mouse support
    		if ((Vector2)Input.mousePosition != oldCursor || inputs.Mouse1) {
    			direction = (inputs.Cursor - (Vector2)transform.position).normalized;
    			direction = CalculateDirection(direction);
    			if (direction != Vector2.zero)
    				anim.PointGunDirection(direction);
    			if (!inputs.Mouse1) direction = Vector2.zero;
    			oldCursor = Input.mousePosition;
    		}
    	}
    	if (direction != Vector2.zero)
    		anim.PointGunDirection(direction);
    	Debug.Log(direction);

    	// Shoot gun / hook
    	if (currentGunIndex == -1) {
    		// Always face hook
    		//if (anim.rm.GetHook() != null)
    		//	anim.FaceDirection(anim.rm.GetHook().transform.position - transform.position);

    		if (anim.rm.GetHook() == null) {
    			// Hook isnt out
    			if (hookPrimed) {
    				hookPrimed = false;
    				if (direction != Vector2.zero)
    					Shoot();
    			}
    			else if (direction != oldDirection && direction != Vector2.zero) {
    				Shoot();
    			}
    		}
    		else {
    			if (anim.rm.GetHookAttachedTo() == null) {
    				// Hook is out, not attached
    				if (direction != oldDirection) {
    					Shoot();
    					if (direction != Vector2.zero && oldDirection != Vector2.zero)
    						hookPrimed = true;
    				}
    			}
    			else {
    				// Hook is attached
    				if (direction != oldDirection && direction != Vector2.zero) {
    					Shoot();
    				}
    			}
    		}
    	}
    	else if (direction != Vector2.zero) {
    		// Shoot bullets
    		Shoot();
    	}
    	oldDirection = direction;
    }


    bool canShoot = false;
    void Shoot() {
    	canShoot = true;
    }
    void Fire() {
    	if (currentGunIndex >= 0 && currentGunIndex < unlockedGuns.Count) {
    		unlockedGuns[currentGunIndex].Shoot(projectileParent);
    		shootCooldown = unlockedGuns[currentGunIndex].cooldown;
    	}
    	else {
    		anim.rm.ShootHook();
    		shootCooldown = hookCooldown;
    	}
    }


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