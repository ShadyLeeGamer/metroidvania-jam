using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class Movement : Entity
{

	// This script contains all the movement functions
	// // doesnt do anything by itself - must be called by PlayerMovement or EnemyMovement, etc

	[HideInInspector] public Rigidbody2D rb;
	public virtual void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	public float horizontalSpeed = 5;
	public float accelerateTime = 0.1f;
	public float decelerateTime = 1f;
	float hTimer = 0;
	float oldDeltaVX = 0; // resets the timer after a direction change
	public int SmoothMove(float hInput) { // =0 when constant speed, =1 when accelerating, =-1 when decelerating
		// Calculate the correct speed based on inputs
		float desiredVX = horizontalSpeed * hInput;
		float deltaVX = desiredVX - rb.velocity.x;
		// Reset timer when new acceleration / deceleration begins
		if (oldDeltaVX == 0) hTimer = 0;
		if ((deltaVX > 0 && oldDeltaVX < 0) ||
			(deltaVX < 0 && oldDeltaVX > 0)) {
			hTimer = 0;
		}
		oldDeltaVX = deltaVX;
		if (deltaVX == 0 || (deltaVX < 0.0001f && deltaVX > -0.0001f))
			return 0;

		// Linearly adjust player velocity when accelerating or decelerating
		float newVX;
		float maxTime = (deltaVX * hInput > 0)?
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
			JumpDirection(Vector2.up, jumpSpeed);
		}
	}
	public void Walljump() {
		if (jCharges <= 0) return;
		if (!onGround && onWall != 0) {
			jCharges--;
			if (onWall == 1) JumpDegrees(walljumpNormalDegrees, walljumpSpeed);
			if (onWall == -1) JumpDegrees(180-walljumpNormalDegrees, walljumpSpeed);
		}
	}
	void JumpDirection(Vector2 direction, float speed) {
		rb.velocity = speed * direction;
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

	// todo: wall + ground crawl for enemies
	// // hook
	// // driving



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
