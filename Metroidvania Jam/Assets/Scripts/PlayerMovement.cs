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
	float oldDeltaVX = 0;
	float initialVX = 0;
	void MoveHorizontal(float hInput) {
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
			return;

		// Linearly adjust player velocity when accelerating or decelerating
		float newVX;
		float maxTime = (deltaVX * hInput > 0)?
			accelerateTime : decelerateTime;
		if (hTimer < maxTime) { // linearly adjust velocity
			float slope = deltaVX / (maxTime - hTimer);
			hTimer += Time.deltaTime;
			newVX = rb.velocity.x + slope * Time.deltaTime;
		}
		else { // Reset timer when acceleration / deceleration has finished
			hTimer = 0;
			newVX = desiredVX;
		}
		rb.velocity = new Vector2(newVX, rb.velocity.y);
	}

	void Update() {
		float h = Input.GetAxis("Horizontal");
		MoveHorizontal(h);
	}

}
