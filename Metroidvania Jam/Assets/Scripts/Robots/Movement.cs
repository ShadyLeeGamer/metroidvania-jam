using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Movement : Entity
{

	// This script contains all the movement functions
	// // doesnt do anything by itself - must be called by PlayerMovement or EnemyMovement, etc

	[HideInInspector] public Rigidbody2D rb;
	Transform hooksParent;
	[HideInInspector] public Transform hookGunTip;
	public virtual void Start() {
		rb = GetComponent<Rigidbody2D>();
		hooksParent = GameObject.Find("Environment").transform.Find("Projectiles").Find("Hooks");
		hookGunTip = transform.Find("Sprites").Find("RoboGuns").Find("Hook Gun").Find("GunTip");
	}



	public float horizontalSpeed = 5;
	public float accelerateTime = 0.1f;
	public float decelerateTime = 1f;
	float hTimer = 0;
	float oldDeltaVX = 0; // resets the timer after a direction change
	//float oldDesiredVX = 0; // ensures full decelerations are performed
	public int SmoothMove(float hInput) { // =0 when constant speed, =1 when accelerating, =-1 when decelerating
		if (GetTurtle()) hInput = 0;
		// Find the direction to move
		/*Vector2 direction;
		if (GetTurtle()) direction = Vector2.zero;
		else if (onGround) direction = transform.right;
		else direction = Vector2.right;
		float dotDirection = Vector2.Dot(rb.velocity, direction);
		bool accOrDec = (hInput * dotDirection > 0);*/
		// Calculate the correct speed based on inputs
		float desiredVX = horizontalSpeed * hInput;
		float deltaVX = desiredVX - rb.velocity.x;
		bool accOrDec = (deltaVX * hInput > 0);
		
		// This code fixes the partial decelerations, but breaks other stuff
		// // Like when going down a hill, sometimes get stuck
		/*if (accOrDec) Debug.Log("acc");
		else Debug.Log("dec");
		if (Mathf.Abs(oldDeltaVX) > 0.5f && accOrDec) {
			desiredVX = oldDesiredVX;
			deltaVX = desiredVX - rb.velocity.x;
			accOrDec = (deltaVX * hInput > 0);
		}
		oldDesiredVX = desiredVX;*/
		// Reset timer when new acceleration / deceleration begins
		if (oldDeltaVX == 0) hTimer = 0;
		if ((deltaVX > 0 && oldDeltaVX < 0) ||
			(deltaVX < 0 && oldDeltaVX > 0)) {
			hTimer = 0;
		}
		//Debug.Log(desiredVX + " " + deltaVX + " " + oldDeltaVX);
		oldDeltaVX = deltaVX;
		// if at correct velocity, tell.
		if (deltaVX < 0.0001f && deltaVX > -0.0001f)
			return 0;

		// Linearly adjust player velocity when accelerating or decelerating
		float newVX;
		float maxTime = (accOrDec)?
			accelerateTime : decelerateTime;
		if (hTimer < maxTime) { // linearly adjust velocity
			float deltaT = maxTime - hTimer;
			deltaT = Mathf.Round(1000 * deltaT) / 1000; // will always be in 0.02 increments
			if (deltaT <= 0.001)
				newVX = desiredVX;
			else {
				float slope = deltaVX / deltaT;
				hTimer += Time.fixedDeltaTime;
				newVX = rb.velocity.x + slope * Time.fixedDeltaTime;
			}
		}
		else { // Reset timer when acceleration / deceleration has finished
			hTimer = 0;
			newVX = desiredVX;
		}
		rb.velocity = new Vector2(newVX, rb.velocity.y);
		if (maxTime == accelerateTime) return 1;
		return -1;
	}
	public bool GetTurtle() {
		bool turtled = Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) < 0.1f;
		turtled &= onGround;
		return turtled;
	}


	public float dashSpeed = 15f;
	public float dashTime = 0.2f;
	public float dashEndSpeed = 5f;
	public int dashCharges = 2;
	int dCharges = 0;
	float dTimer = 0;
	public int GetDashCharges() {
		return dCharges;
	}
	public bool Dash(Vector2 direction) {
		if (dCharges <= 0) return false;
		if (dTimer < dashTime) {
			dTimer += Time.fixedDeltaTime;
			rb.velocity = dashSpeed * direction;
			return true;
		}
		else {
			dTimer = 0;
			dCharges--;
			rb.velocity = dashEndSpeed * direction;
			return false;
		}
	}


	public float jumpSpeed = 5f;
	public int jumpCharges = 3;
	public float walljumpSpeed = 8f;
	public float walljumpNormalDegrees = 45;
	int jCharges = 0;
	bool onGround = false;
	int onWall = 0; // =1 for left wall, =-1 for right wall
	public int GetJumpCharges() {
		return jCharges;
	}
	public void ResetCharges() {
		jCharges = jumpCharges;
		dCharges = dashCharges;
		jetFuel = jetpackUseTime;
	}
	public void Jump() {
		//Debug.Log("Jump" + " " + onGround + " " + onWall);
		if (jCharges <= 0) return;
		if (onGround || onWall == 0) {
			jCharges--;
			rb.velocity = new Vector2(rb.velocity.x, 0);
			JumpDirection(Vector2.up, jumpSpeed);
		}
	}
	public void Walljump() {
		if (jCharges <= 0) return;
		if (!onGround && onWall != 0) {
			jCharges--;
			rb.velocity = Vector2.zero;
			if (onWall == 1) JumpDegrees(walljumpNormalDegrees, walljumpSpeed);
			if (onWall == -1) JumpDegrees(180-walljumpNormalDegrees, walljumpSpeed);
		}
	}
	void JumpDirection(Vector2 direction, float speed) {
		rb.velocity += speed * direction.normalized;
	}
	void JumpDegrees(float deg, float speed) {
		float rad = deg * Mathf.Deg2Rad;
		Vector2 fromDegrees = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
		JumpDirection(fromDegrees, speed);
	}

	public float wallslideSpeed = 1f;
	public int Wallslide() {
		//Debug.Log(onWall);
		if (onWall == 0) return 0;
		rb.velocity = new Vector2(rb.velocity.x, -wallslideSpeed);
		return onWall;
	}

	public float jetpackSpeed = 2f;
	public float jetpackUseTime = 2; // fuel
	float jetFuel = 0;
	public float Jetpack() {
		if (jetFuel <= 0) return 0;
		else {
			jetFuel -= Time.fixedDeltaTime;
			rb.velocity = new Vector2(rb.velocity.x, jetpackSpeed);
			return jetFuel;
		}
	}

	public float slamGravity = 3f;
	public bool Slam() {
		if (onGround) {
			rb.gravityScale = 1;
			return false;
		}
		else {
			rb.gravityScale = slamGravity;
			return true;
		}
	}



	public float hookThrowSpeed = 10;
	public float hookRetractSpeed = 10;
	public float hookRetractAcceleration = 5;
	public float hookLaunchSpeed = 10;
	public float hookGravity = 0.4f;
	public float hookDrag = 0.5f;
	public float maxChainLength = 4;
	public float hookRestitution = 0.6f;
	public float hookInsertion = 0.2f;
	public float playerRestitution = 0.95f;
	public GameObject hookPrefab;
	Rigidbody2D hook = null;
	GameObject hookAttachedTo = null;
	float chainLength = -1;
	public void ThrowHook(Vector2 direction) {
		DestroyHook();
		Transform h = Instantiate(hookPrefab).transform;
		h.parent = hooksParent;
		h.localPosition = hookGunTip.position;
		hook = h.GetComponent<Rigidbody2D>();
		hook.velocity = hookThrowSpeed * direction;
		hook.gravityScale = hookGravity;
		hook.drag = hookDrag;
		Physics2D.IgnoreCollision(transform.Find("Sprites").Find("Robot Outlet").GetComponent<Collider2D>(), hook.GetComponent<Collider2D>());
	}
	void DestroyHook() {
		if (hook != null) Destroy(hook.gameObject);
		hookAttachedTo = null;
		chainLength = maxChainLength;
		retracting = false;
	}
	public Rigidbody2D GetHook() {
		return hook;
	}
	public GameObject GetHookAttachedTo() {
		return hookAttachedTo;
	}
	GameObject SearchForCollision(Rigidbody2D r, string layerName) {
		// Detect collisions (without extra script)
		List<Collider2D> results = new List<Collider2D>();
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask(layerName));
		r.OverlapCollider(filter, results);
		// Find a suitable result
		for (int i = 0; i < results.Count; i++) {
			if (results[i].gameObject.layer == LayerMask.NameToLayer(layerName))
				if (results[i].transform.root != transform.root)
					return results[i].gameObject;
		}
		return null;
	}
 	public bool Hook() { // =true when hook still out
 		if (hook == null) return false;
 		Vector2 pToHook = hook.transform.position - transform.position;
		float hookDist = pToHook.magnitude;
		// Destroy when close enough to player
		if (hookDist < 0.3f && retracting) {
			DestroyHook();
			return false;
		}

		// Search for a hookable
 		if (!retracting)
 			hookAttachedTo = SearchForCollision(hook, "Hookable");
		//Debug.Log(hookAttachedTo);

		// Clamp hook / player within chain range
		bool outOfRange = (hookDist > 1.01f * chainLength);
		// Clamp hook to player
		if (hookAttachedTo == null) {
			hook.gravityScale = hookGravity;
			hook.constraints = RigidbodyConstraints2D.FreezeRotation;
			if (outOfRange) {
				//Debug.Log("Clamping hook");
				hook.transform.position = (Vector2)transform.position + chainLength * pToHook.normalized;
				hook.velocity = ReflectVelocityCircle(hook, rb, chainLength, hookRestitution);
			}
		}
		// Clamp player to hook, if attached to something
		else {
			hook.transform.position = hookAttachedTo.transform.position + hookAttachedTo.transform.up * hookInsertion;
			// Rotation handled by RobotAnimations
			hook.gravityScale = 0;
			hook.velocity = Vector2.zero;
			hook.constraints = RigidbodyConstraints2D.FreezeAll;
			if (outOfRange) {
				//Debug.Log("Clamping player");
				bool keepPosY = onGround;
				Vector2 pos = (Vector2)hook.transform.position - chainLength * pToHook.normalized;
				if (keepPosY) pos.y = transform.position.y;
				transform.position = pos;
				rb.velocity = ReflectVelocityCircle(rb, hook, chainLength, playerRestitution);
			}
		}
		return true;
	}
	// todo: doesnt come back to GunTip
	Vector2 ReflectVelocityCircle(Rigidbody2D rTarget, Rigidbody2D rBase, float distance, float restitution) {
		// Keeps rTarget within a certain distance of rBase
		// // by reflecting relative velocity when out of range
		Vector2 normal = rBase.transform.position - rTarget.transform.position;
		Vector2 relativeVelocity = rTarget.velocity - rBase.velocity;
		relativeVelocity *= restitution;
		float dot = Vector2.Dot(normal, relativeVelocity);
		if (dot < 0) { // if travelling outward
			float angle = Vector2.SignedAngle(Vector2.right, normal);
			float vOffsetAngle = Vector2.SignedAngle(normal, -relativeVelocity);
			angle -= vOffsetAngle;
			angle *= Mathf.Deg2Rad;
			return relativeVelocity.magnitude * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}
		return rTarget.velocity;
	}
	bool retracting = false;
	float rTimer = 0;
	public void RetractHook(bool launch) {
		if (hook == null) return;
		if (!retracting) {
			rTimer = 0;
			if (launch) {
				// Launch player up / towards hook
				Jump();
				Vector2 toHook = hookAttachedTo.transform.position - transform.position;
				JumpDirection(toHook.normalized, hookLaunchSpeed);
			}
			hookAttachedTo = null;
		}
		retracting = true;

		// Decrease chain length
		rTimer += Time.fixedDeltaTime;
		chainLength = maxChainLength - hookRetractSpeed*rTimer - 0.5f*hookRetractAcceleration*Mathf.Pow(rTimer, 2);
		chainLength = Mathf.Clamp(chainLength, 0, maxChainLength);
	}



	public virtual void Update() {
		//Debug.Log(onGround);
	}
	public bool GetOnGround() {
		return onGround;
	}
	public virtual void OnTriggerEnter2D(Collider2D info) {
		GameObject other = info.gameObject;
		if (other.tag == "Ground") {
			onGround = true;
			ResetCharges();
		}
		if (other.tag == "Wall") {
			if (other.transform.position.x < transform.position.x)
				onWall = 1;
			else onWall = -1;
			ResetCharges();
		}
	}
	// sloppy bug fix with Stay(): We have multiple Ground triggers in scene.
	// // when trigger leaves, sets onGround = false even though it's touching a different Ground
	public virtual void OnTriggerStay2D(Collider2D info) {
		GameObject other = info.gameObject;
		if (other.tag == "Ground") {
			onGround = true;
			ResetCharges();
		}
		if (other.tag == "Wall") {
			if (other.transform.position.x < transform.position.x)
				onWall = 1;
			else onWall = -1;
			ResetCharges();
		}
	}
	public virtual void OnTriggerExit2D(Collider2D info) {
		GameObject other = info.gameObject;
		if (other.tag == "Ground") {
			onGround = false;
		}
		if (other.tag == "Wall") {
			onWall = 0;
		}
	}

}
