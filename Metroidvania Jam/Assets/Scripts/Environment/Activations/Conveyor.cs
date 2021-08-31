using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Activated
{

	
    
	// Move all objects along when active
	public float speed = 2f;
	List<Rigidbody2D> objects = new List<Rigidbody2D>();
    void MoveAlong() {
    	Vector3 delta = speed * Time.fixedDeltaTime * transform.right;
    	for (int i = 0; i < objects.Count; i++) {
    		objects[i].transform.position += delta;
    	}
    }
    void FixedUpdate() {
    	if (active) MoveAlong();
    }

    // Store references to the objects
    void OnCollisionEnter2D(Collision2D info) {
		GameObject other = info.collider.gameObject;
		Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
		if (rb == null) {
			rb = other.GetComponentInParent<Rigidbody2D>();
			if (rb == null) return;
		}
		if (!objects.Contains(rb)) objects.Add(rb);
	}
	void OnCollisionExit2D(Collision2D info) {
		GameObject other = info.collider.gameObject;
		Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
		if (rb == null) {
			rb = other.GetComponentInParent<Rigidbody2D>();
			if (rb == null) return;
		}
		if (objects.Contains(rb)) objects.Remove(rb);
	}

}
