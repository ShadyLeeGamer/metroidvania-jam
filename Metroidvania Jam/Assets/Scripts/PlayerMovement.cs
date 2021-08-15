using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class PlayerMovement : MonoBehaviour
{

	public Transform faceParent;

	Movement m;
	void Start() {
		m = GetComponent<Movement>();
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


	bool dashing = false;
	bool slamming = false;
	void Update() {
		//Debug.Log(sliding + " " + Time.time + " " + m.GetCharges());
		if (Input.GetKeyDown(KeyCode.LeftShift))
			dashing = true;
		if (Input.GetKeyDown(KeyCode.E))
			slamming = true;

		if (!dashing && !slamming) {
			if (!sliding) {
				float h = Input.GetAxis("Horizontal");
				m.SmoothMove(h);
				FaceTowardsVelocity();
			}
			if (Input.GetKeyDown(KeyCode.W)) {
				if (!sliding) m.Jump();
				else {
					slideCooldown = 0.5f;
					m.Walljump();
				}
			}
			if (Input.GetKey(KeyCode.Space))
				m.Jetpack();
		}
		else if (dashing) {
			Vector2 direction = (facingR)? Vector2.right : -Vector2.right;
			dashing = m.Dash(direction.normalized);
		}
		else if (slamming) {
			slamming = m.Slam();
		}
		

	}


	bool sliding = false;
	float slideCooldown = 0;
	void OnTriggerStay2D(Collider2D info) {
		GameObject other = info.gameObject;
		if (slideCooldown >= 0) {
			sliding = false;
			slideCooldown -= Time.deltaTime;
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
