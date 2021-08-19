using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputs))]
public class RobotMovement : Movement
{

	public Transform faceParent;

	Inputs inputs;
	public override void Start() {
		base.Start(); // GET RB

		inputs = GetComponent<Inputs>();
	}

	// todo: move this into a RobotAnimations script
	void FaceTowardsVelocity() {
		if (rb.velocity.x > 0.0001f)
			FaceRight(true);
		if (rb.velocity.x < -0.0001f)
			FaceRight(false);
	}
	void FaceRight(bool right) {
		if (right) {
			faceParent.eulerAngles = Vector2.zero;
		}
		else {
			faceParent.eulerAngles = 180 * Vector2.up;
		}
	}

	public float dashCooldown = 0.3f;
	public float wallCooldown = 0.2f;
	float dCooldown = 0;
	float wCooldown = 0;
	bool dashing = false;
	bool slamming = false;
	bool hooking = false;
	bool retractingHook = false;
	void FixedUpdate() {
		//Debug.Log(sliding + " " + dashing + " " + slamming + " " + Time.time + " " + GetJumpCharges() + " " + GetDashCharges());
		inputs.CalculateKeyDown();
		inputs.CalculateExtra();

		// Start ability - dash, slam, or jump
		if (dCooldown > 0) dCooldown -= Time.fixedDeltaTime;
		if (wCooldown > 0) wCooldown -= Time.fixedDeltaTime;
		if (!dashing && (inputs.DoubleLeft || inputs.DoubleRight) && dCooldown <= 0) {
			dashing = true;
			if (hooking) {
				retractingHook = true;
				RetractHook(false);
			}
		}
		if (!slamming && inputs.DownGetDown) {
			slamming = true;
			sliding = false;
			if (hooking) {
				retractingHook = true;
				RetractHook(false);
			}
		}
		if (inputs.Mouse1GetDown) {
			if (!hooking) {
				hooking = true;
				ThrowHook((inputs.Cursor - (Vector2)transform.position).normalized);
			}
			else {
				retractingHook = true;
			}
		}


		if (!dashing && !slamming) {
			// Midair move horizontally
			if (!sliding) {
				if (wCooldown <= 0) {
					SmoothMove(inputs.Horizontal);
					FaceTowardsVelocity();
				}
			}
			else {
				int onWall = Wallslide();
				// Move off wall
				if (onWall == 1) {
					FaceRight(true);
					if (inputs.Horizontal > 0)
						SmoothMove(inputs.Horizontal);
				}
				if (onWall == -1) {
					FaceRight(false);
					if (inputs.Horizontal < 0)
						SmoothMove(inputs.Horizontal);
				}
			}
			// Jump / walljump
			if (inputs.UpGetDown) {
				if (!sliding) {
					Jump();
				}
				else {
					wCooldown = wallCooldown;
					Walljump();
				}
				if (hooking) {
					retractingHook = true;
					RetractHook(false);
				}
			}
			// Jetpack
			if (inputs.Jump) {
				Jetpack();
				if (hooking) {
					retractingHook = true;
					RetractHook(false);
				}
			}

			// Hook
			if (hooking) {
				if (retractingHook) {
					GameObject attachedTo = GetHookAttachedTo();
					// Transfer to new robot if attachedTo another robot
					RetractHook(attachedTo != null);
				}
				hooking = Hook();
				if (!hooking) retractingHook = false;
			}

		}
		else if (dashing) {
			if (inputs.DoubleRight) dashing = Dash(Vector2.right);
			else if (inputs.DoubleLeft) dashing = Dash(-Vector2.right);
			else dashing = false;
			
			if (!dashing) dCooldown = dashCooldown;
			if (hooking) retractingHook = true;
		}
		else if (slamming) {
			slamming = Slam();
			if (hooking) retractingHook = true;
		}
		
		inputs.ResetKeyDown();
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
