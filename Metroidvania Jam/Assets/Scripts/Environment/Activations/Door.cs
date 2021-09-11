using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Activated
{

    Collider2D door;
    void Start() {
    	door = transform.Find("Door").GetComponent<Collider2D>();
    }
    public float moveTime = 0.5f;
    float timer = 0;
    public override void Update() {
    	base.Update();
        if (blocker != null && FoundCollision(door, blocker)) return;

    	if (active) {
    		if (timer < moveTime) {
    			float progress = timer / moveTime;
    			MoveDoor(progress);
    			timer += Time.deltaTime;
    		}
    		else {
    			MoveDoor(1);
    			timer = moveTime;
    		}
    	}
    	else {
    		if (timer > 0) {
    			float progress = timer / moveTime;
    			MoveDoor(progress);
    			timer -= Time.deltaTime;
    		}
    		else {
    			MoveDoor(0);
    			timer = 0;
    		}
    	}

    }
    public float upY = 1;
    void MoveDoor(float progress) {
    	door.transform.localPosition = upY * progress * Vector2.up;
    }


    public Collider2D blocker;
    bool FoundCollision(Collider2D col, Collider2D cTarget) {
        // Detect collisions (without extra script)
        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        col.OverlapCollider(filter, results);
        // Find a suitable result
        for (int i = 0; i < results.Count; i++) {
            if (results[i] == cTarget)
                return true;
        }
        return false;
    }

}
