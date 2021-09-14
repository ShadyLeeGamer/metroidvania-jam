using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inputs), typeof(RobotAnimations), typeof(RobotShooting))]
public class RobotMovement : Movement
{

	Inputs inputs;
	RobotAnimations anim;
	public override void Start() {
		base.Start(); // GET RB

		inputs = GetComponent<Inputs>();
		anim = GetComponent<RobotAnimations>();
	}


	public bool canMoveInAir = false;
	public bool canJump = false;
	public bool canWallJump = false;
	public bool canWallSlide = false;
	public bool canDash = false;
	public bool canSlam = false;
	public bool canJetpack = false;

	public float dashCooldown = 0.3f;
	public float wallCooldown = 0.2f;
	float dCooldown = 0;
	float wCooldown = 0;
	bool dashing = false;
	bool slamming = false;
	bool hooking = false;
	bool retractingHook = false;
	void FixedUpdate() {
		if (GetTurtle()) {
			SmoothMove(0);
			return;
		}


		//Debug.Log(sliding + " " + dashing + " " + slamming + " " + Time.time + " " + GetJumpCharges() + " " + GetDashCharges());
		inputs.CalculateKeyDown();
		inputs.CalculateExtra();

		// When using the GetDown feature, must call from here
		if (inputs.SwapGetDown || inputs.Mouse2GetDown)
			GetComponent<RobotShooting>().CycleGun();

		// Start ability - dash, slam, or jump
		if (dCooldown > 0) dCooldown -= Time.fixedDeltaTime;
		if (wCooldown > 0) wCooldown -= Time.fixedDeltaTime;
		if (!dashing && (inputs.DoubleLeft || inputs.DoubleRight) && dCooldown <= 0 && canDash) {
			dashing = true;
			if (hooking) {
				retractingHook = true;
				RetractHook(false);
			}
		}
		if (!slamming && inputs.DownGetDown && canSlam) {
			slamming = true;
			sliding = false;
			if (hooking) {
				retractingHook = true;
				RetractHook(false);
			}
		}


		if (!dashing && !slamming) {
			// Midair move horizontally
			if (!sliding) {
				if (wCooldown <= 0) {
					if (GetOnGround() || canMoveInAir) SmoothMove(inputs.Horizontal);
					if (!hooking || retractingHook) anim.FaceVelocity();
				}
			}
			else {
				int onWall = (canWallSlide)? Wallslide() : 0;
				// Move off wall
				if (onWall == 1) {
					anim.FaceRight(true);
					if (inputs.Horizontal > 0)
						if (GetOnGround() || canMoveInAir) SmoothMove(inputs.Horizontal);
				}
				if (onWall == -1) {
					anim.FaceRight(false);
					if (inputs.Horizontal < 0)
						if (GetOnGround() || canMoveInAir) SmoothMove(inputs.Horizontal);
				}
			}
			// Jump / walljump
			if (inputs.UpGetDown) {
				if (!sliding) {
					if (canJump) Jump();
				}
				else {
					wCooldown = wallCooldown;
					if (canWallJump) Walljump();
				}
				if (hooking) {
					retractingHook = true;
					RetractHook(false);
				}
			}
			// Jetpack
			if (inputs.Jump) {
				if (canJetpack) {
					Jetpack();
					transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
				}
				if (hooking) {
					retractingHook = true;
					RetractHook(false);
				}
			}

			// Hook
			if (hooking) {
				GameObject attachedTo = GetHookAttachedTo();
				// prevent launch from self-insertion - blocked by SearchForCollision in Movement
				if (attachedTo != null && attachedTo.transform.root == transform.root)
					attachedTo = null;
				if (attachedTo != null) {
					// Charge up from active outlets
					if (attachedTo.name == "Activated Outlet") {
						if (attachedTo.GetComponent<Activated>().active)
							anim.Charge();
					}
				}
				if (retractingHook) {
					anim.DestroyCharges();
					// Transfer to new robot if attachedTo another robot
					if (attachedTo != null && attachedTo.name == "Robot Outlet") {
						anim.Transfer();
					}
					else RetractHook(attachedTo != null);
				}
				hooking = Hook();
				anim.UpdateChain("Parabola", true, true);
				if (!hooking) {
					anim.DestroyChain();
					anim.DestroyCharges();
					retractingHook = false;
				}
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
			if (canWallSlide) sliding = true;
		}
	}
	public override void OnTriggerExit2D(Collider2D info) {
		base.OnTriggerExit2D(info);

		GameObject other = info.gameObject;
		if (other.tag == "Wall") {
			sliding = false;
		}
	}

	// Called by RobotShooting
	// hook retracts when unpressed shoot keys, unless attached
	public void ShootHook() {
		if (!hooking) {
			float angle = anim.guns.localEulerAngles.z * Mathf.Deg2Rad;
			Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			if (anim.spritesParent.localEulerAngles.y == 180)
				direction.x = -direction.x;
			hooking = true;
			ThrowHook(direction);
		}
		else {
			retractingHook = true;
		}
	}





}
