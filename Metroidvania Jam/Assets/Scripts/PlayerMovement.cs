using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{


	Rigidbody2D rb;
	void Start() {
		rb = GetComponent<Rigidbody2D>();
	}


	public float horizontalSpeed = 1;
	public float accelerateTime = 0.5f;
	public float decelerateTime = 0.5f;
	float hTimer = 0;
	void MoveHorizontal(float hInput) {
		// Calculate the correct speed based on inputs
		float desiredVelocityX = horizontalSpeed * hInput;
		float deltaVelocityX = desiredVelocityX - rb.velocity.x;
		if (deltaVelocityX == 0) return;

		// Linearly adjust player velocity when accelerating or decelerating
		float newVelocityX;
		float maxTime = (deltaVelocityX > 0)?
			accelerateTime : decelerateTime;
		if (hTimer < maxTime) { // adjust velocity
			hTimer += Time.deltaTime;
			newVelocityX = rb.velocity.x + deltaVelocityX * hTimer / maxTime;
		}
		else { // acceleration or deceleration has finished
			hTimer = 0;
			newVelocityX = desiredVelocityX;
		}
		rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
	}

	void Update() {
		float h = Input.GetAxis("Horizontal");
		MoveHorizontal(h);
	}

}
