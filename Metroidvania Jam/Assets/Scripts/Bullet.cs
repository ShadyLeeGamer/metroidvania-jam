using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{

	// How much dmg is dealt by this bullet
	public float baseDamage = 1;

	Rigidbody2D rb;
	void Start() {
		rb = GetComponent<Rigidbody2D>();
	}
	void Update() {
		PointVelocity();
	}

	// Bullet should face in the direction it's moving
	void PointAngle(float angle) {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
    }
    void PointDirection(Vector2 direction) {
        if (direction == Vector2.zero) return;
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        PointAngle(angle);
    }
    void PointVelocity() {
    	if (rb.velocity == Vector2.zero) return;
    	PointDirection(rb.velocity);
    }

// extra todo: bouncing, globs of chemicals, destroy time, piercing
    public float destroyTime = 5f;
    public int numBounces = 0;
    public float bounceRestitution = 1;
    public bool stickOn = true;
    public bool piercing = false;
    void OnTriggerEnter2D(Collider2D other) {
    	// Destroy me when hit static object
        if (other.tag == "Ground" || other.tag == "Wall") {
        	if (numBounces == 0 && !stickOn)
            	Destroy(gameObject);
        }
        // (Robots / enemies destroy the bullets that hit them in their own scripts)
    }
}
