using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteAnimations))]
public class Breakable : MonoBehaviour
{

	// Note: Breakables should always be placed with the
	// // Up direction facing where the player will be standing on break

	Collider2D col;
	SpriteAnimations anim;
	void Start() {
		col = GetComponent<Collider2D>();
		anim = GetComponent<SpriteAnimations>();
	}

	public float shatterSpeed = 2f;
	public string requiredLayer = "";
	public void Shatter() {
		col.enabled = false;
		anim.StartSingleDestroy();
	}

    void OnCollisionEnter2D(Collision2D info) {
		GameObject other = info.collider.gameObject;
		// Layer check
		if (requiredLayer != "" && LayerMask.LayerToName(other.layer) != requiredLayer) return;
		// Speed check
		float inSpeed = info.relativeVelocity.magnitude * Mathf.Abs(Vector2.Dot(transform.up, info.relativeVelocity.normalized));
		if (inSpeed >= shatterSpeed)
			Shatter();
    }

}
