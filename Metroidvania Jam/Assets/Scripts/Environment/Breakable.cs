using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteAnimations))]
public class Breakable : MonoBehaviour
{
	Collider2D col;
	SpriteAnimations anim;
	void Start() {
		col = GetComponent<Collider2D>();
		anim = GetComponent<SpriteAnimations>();
	}

	public float shatterSpeed = 2f;
	public string requiredTag = "";
	public void Shatter() {
		col.enabled = false;
		anim.StartSingleDestroy();
	}

    void OnCollisionEnter2D(Collision2D info) {
		GameObject other = info.collider.gameObject;
		Debug.Log(other);
		if (requiredTag != "" && other.tag != requiredTag) return;
		Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
		if (rb == null) {
			rb = other.GetComponentInParent<Rigidbody2D>();
			if (rb == null) return;
		}
		Debug.Log(rb.velocity.magnitude);
		// Speed check
		Vector2 pos = rb.transform.position - transform.position;
		if (rb.velocity.magnitude * Vector2.Dot(pos, rb.velocity) >= shatterSpeed)
			Shatter();
    }

}
