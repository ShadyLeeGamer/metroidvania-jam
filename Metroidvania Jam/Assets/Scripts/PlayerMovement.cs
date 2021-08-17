using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(PlayerInputs))]
public class PlayerMovement : MonoBehaviour
{

	public Transform faceParent;

	Movement m;
	PlayerInputs inputs;
	void Start() {
		m = GetComponent<Movement>();
		inputs = GetComponent<PlayerInputs>();
	}

	// todo: move this into an animations script
	bool facingR = true;
	void FaceTowardsVelocity() {
		if (m.rb.velocity.x > 0.0001f)
			FaceRight(true);
		if (m.rb.velocity.x < 0.0001f)
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
	bool throwingHook = false;
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


		if (!dashing && !slamming) {
			// Walk horizontally
			if (!sliding) {
				if (wCooldown <= 0) {
					m.SmoothMove(inputs.hAxis);
					FaceTowardsVelocity();
				}
			}
			else {
				int onWall = m.Wallslide();
				// Move off wall
				if (onWall == 1) {
					FaceRight(true);
					if (inputs.hAxis > 0)
						m.SmoothMove(inputs.hAxis);
				}
				if (onWall == -1) {
					FaceRight(false);
					if (inputs.hAxis < 0)
						m.SmoothMove(inputs.hAxis);
				}
			}
			// Jump / walljump
			if (inputs.jump && !jumping) {
				jumping = true;
				if (!sliding) m.Jump();
				else {
					wCooldown = wallCooldown;
					m.Walljump();
				}
				
			}
			// Jetpack
			if (inputs.jet)
				m.Jetpack();
		}
		else if (dashing) {
			Vector2 direction = (facingR)? Vector2.right : -Vector2.right;
			dashing = m.Dash(direction.normalized);
			if (!dashing) dCooldown = dashCooldown;
		}
		else if (slamming) {
			slamming = m.Slam();
		}
		else if (throwingHook) {
			throwingHook = m.ThrowHook(inputs.mouseWorld.normalized);
		}
		else if (retractingHook) {
			retractingHook = m.RetractHook(true);
		}
		
		inputs.Reset();
	}


	bool sliding = false;
	void OnTriggerEnter2D(Collider2D info) {
		GameObject other = info.gameObject;
		if (other.tag == "Wall") {
			sliding = true;
		}
	}
	/*void OnTriggerStay2D(Collider2D info) {
		GameObject other = info.gameObject;
		if (slideCooldown >= 0 || slamming) {
			sliding = false;
			slideCooldown -= Time.fixedDeltaTime;
			return;
		}
		if (other.tag == "Wall") {
			sliding = true;
			onWall = m.Wallslide();
		}
	}*/
	void OnTriggerExit2D(Collider2D info) {
		GameObject other = info.gameObject;
		if (other.tag == "Wall") {
			sliding = false;
		}
	}


}
