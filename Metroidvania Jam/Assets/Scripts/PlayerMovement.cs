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
		if (m.rb.velocity.x > 0.0001f) {
			facingR = true;
			faceParent.eulerAngles = Vector2.zero;
		}
		if (m.rb.velocity.x < 0.0001f) {
			facingR = false;
			faceParent.eulerAngles = 180 * Vector2.up;
		}
	}

	float dashCooldown = 0;
	bool dashing = false;
	bool slamming = false;
	bool jumping = false;
	void FixedUpdate() {
		//Debug.Log(sliding + " " + dashing + " " + slamming + " " + Time.time + " " + m.GetCharges());

		if (dashCooldown > 0) dashCooldown -= Time.fixedDeltaTime;
		if (!dashing && inputs.dash && dashCooldown <= 0)
			dashing = true;
		if (!slamming && inputs.slam)
			slamming = true;
		if (jumping && !inputs.jump)
			jumping = false;

		if (!dashing && !slamming) {
			if (!sliding) {
				m.SmoothMove(inputs.hAxis);
				FaceTowardsVelocity();
			}
			if (inputs.jump && !jumping) {
				if (!sliding) {
					jumping = true;
					m.Jump();
				}
				else {
					slideCooldown = 0.5f;
					m.Walljump();
				}
			}
			if (inputs.jet)
				m.Jetpack();
		}
		else if (dashing) {
			Vector2 direction = (facingR)? Vector2.right : -Vector2.right;
			dashing = m.Dash(direction.normalized);
			if (!dashing) dashCooldown = 0.5f;
		}
		else if (slamming) {
			slamming = m.Slam();
		}
		
		inputs.Reset();
	}


	bool sliding = false;
	float slideCooldown = 0;
	void OnTriggerStay2D(Collider2D info) {
		GameObject other = info.gameObject;
		if (slideCooldown >= 0) {
			sliding = false;
			slideCooldown -= Time.fixedDeltaTime;
			return;
		}
		if (other.tag == "Wall") {
			sliding = true;
			m.Wallslide();
		}
	}
	void OnTriggerExit2D(Collider2D info) {
		GameObject other = info.gameObject;
		if (other.tag == "Wall") {
			sliding = false;
		}
	}


}
