using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputs))]
public class PlayerMovement : Movement
{

	// todo: generalize inputs script, then rename to RobotMovement

	public Transform faceParent;

	[SerializeField] Gun gun;

	PlayerInputs inputs;
	public override void Start() {
		base.Start(); // GET RB

		inputs = GetComponent<PlayerInputs>();
	}

	// todo: move this into a RobotAnimations script
	bool facingR = true;
	void FaceTowardsVelocity() {
		if (rb.velocity.x > 0.0001f)
			FaceRight(true);
		if (rb.velocity.x < -0.0001f)
			FaceRight(false);
	}
	void FaceRight(bool right) {
		if (right) {
			facingR = true;
			faceParent.eulerAngles = Vector2.zero;
		}
		else {
			facingR = false;
			faceParent.eulerAngles = 180 * Vector2.up;
		}
	}

	public float dashCooldown = 0.3f;
	public float wallCooldown = 0.2f;
	float dCooldown = 0;
	float wCooldown = 0;
	bool dashing = false;
	bool slamming = false;
	bool jumping = false;
	bool hooking = false;
	bool retractingHook = false;
	void FixedUpdate() {
		//Debug.Log(sliding + " " + dashing + " " + slamming + " " + Time.time + " " + m.GetJumpCharges());
		
		// Start ability - dash, slam, or jump
		if (dCooldown > 0) dCooldown -= Time.fixedDeltaTime;
		if (wCooldown > 0) wCooldown -= Time.fixedDeltaTime;
		if (!dashing && inputs.dash && dCooldown <= 0)
			dashing = true;
		if (!slamming && inputs.slam) {
			slamming = true;
			sliding = false;
		}
		if (jumping && !inputs.jump)
			jumping = false;
		if (inputs.mouse1) {
			if (!hooking) {
				hooking = true;
				ThrowHook((inputs.mouseWorld - (Vector2)transform.position).normalized);
			}
			else retractingHook = true;
		}




		if (inputs.shoot) {
			float xVelocity = facingR ? 1f : -1f;
			float yVelocity = inputs.lookUp ? 1f : inputs.lookDown ? -1f : 0f;
			gun.Shoot(new Vector2(xVelocity, yVelocity));
		}


		if (!dashing && !slamming) {
			// Midair move horizontally
			if (!sliding) {
				if (wCooldown <= 0) {
					SmoothMove(inputs.hAxis);
					FaceTowardsVelocity();
				}
			}
			else {
				int onWall = Wallslide();
				// Move off wall
				if (onWall == 1) {
					FaceRight(true);
					if (inputs.hAxis > 0)
						SmoothMove(inputs.hAxis);
				}
				if (onWall == -1) {
					FaceRight(false);
					if (inputs.hAxis < 0)
						SmoothMove(inputs.hAxis);
				}
			}
			// Jump / walljump
			if (inputs.jump && !jumping) {
				jumping = true;
				if (!sliding) {
					Jump();
				}
				else {
					wCooldown = wallCooldown;
					Walljump();
				}
				if (hooking) retractingHook = true;
			}
			// Jetpack
			if (inputs.jet) {
				Jetpack();
				if (hooking) retractingHook = true;
			}

			// Hook
			if (hooking) {
				hooking = Hook();
				if (retractingHook) {
					retractingHook = hooking;
					// Transfer to new robot or somethin
					//GameObject attachedTo = GetHookAttachedTo();
					RetractHook();
				}
			}

		}
		else if (dashing) {
			Vector2 direction = (facingR)? Vector2.right : -Vector2.right;
			dashing = Dash(direction.normalized);
			if (!dashing) dCooldown = dashCooldown;
			if (hooking) retractingHook = true;
		}
		else if (slamming) {
			slamming = Slam();
			if (hooking) retractingHook = true;
		}
		
		inputs.Reset();
	}


	bool sliding = false;
	public override void OnTriggerEnter2D(Collider2D info) {
		base.OnTriggerEnter2D(info);

		GameObject other = info.gameObject;
		if (other.tag == "Wall") {
			sliding = true;
		}
	}
	public override void OnTriggerExit2D(Collider2D info) {
		base.OnTriggerExit2D(info);

		GameObject other = info.gameObject;
		if (other.tag == "Wall") {
			sliding = false;
		}
	}


}
