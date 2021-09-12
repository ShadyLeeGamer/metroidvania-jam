using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Activator), typeof(Collider2D))]
public class Button : MonoBehaviour
{
    Activator a;
    Transform button;
    Collider2D tmp;
    void Start() {
        a = GetComponent<Activator>();
        button = transform.Find("Button");
        tmp = GetComponent<Collider2D>();
    }

    public float moveTime = 0.5f;
    float timer = 0;
    void Update() {
    	if (!pressed) {
    		if (timer < moveTime) {
    			float progress = timer / moveTime;
    			MoveButton(progress);
    			timer += Time.deltaTime;
    			a.active = false;
    		}
    		else {
    			MoveButton(1);
    			timer = moveTime;
    			tmp.enabled = true;
    			a.active = false;
    		}
    	}
    	else {
    		GameObject found = SearchForCollision(button.GetComponent<Collider2D>(), requiredLayer);
    		//Debug.Log(found);
    		if (found == null && canUnpress) {
    			OnColExit();
    			//return;
    		}
    		if (timer > 0) {
    			float progress = timer / moveTime;
    			MoveButton(progress);
    			timer -= Time.deltaTime;
    			a.active = false;
    		}
    		else {
    			MoveButton(0);
    			if (canUnpress && found == null) timer = 0;
    			//Debug.Log("down");
    			a.active = true;
    		}
    	}

    }
    public float upY = 0.6f;
    void MoveButton(float progress) {
    	button.transform.localPosition = upY * progress * Vector2.up;
    }

    public float minSpeed = 2f;
	public string requiredLayer = "Robot";
    bool pressed = false;
    void OnCollisionEnter2D(Collision2D info) {
		if (pressed) return;
		GameObject other = info.collider.gameObject;
		// Layer check
		if (requiredLayer != "" && LayerMask.LayerToName(other.layer) != requiredLayer) return;
		// Speed check
		float inSpeed = info.relativeVelocity.magnitude * Mathf.Abs(Vector2.Dot(button.up, info.relativeVelocity.normalized));
		if (inSpeed >= minSpeed) {
			pressed = true;
			tmp.enabled = false;
		}
    }
    public bool canUnpress = false;
    void OnColExit() {
    	if (!canUnpress || !a.active) return;
		pressed = false;
		a.active = false;
    }



    GameObject SearchForCollision(Collider2D c, string layerName) {
		// Detect collisions (without extra script)
		List<Collider2D> results = new List<Collider2D>();
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask(layerName));
		c.OverlapCollider(filter, results);
		//Debug.Log(results.Count);
		// Find a suitable result
		for (int i = 0; i < results.Count; i++) {
			//Debug.Log(results[i].gameObject);
			if (results[i].gameObject.layer == LayerMask.NameToLayer(layerName))
				return results[i].gameObject;
		}
		return null;
	}


	// todo: unpress is bugged for wide, works fine for elevator tho
	// cam discovered is disabled
	// cant transfer to mark-two
	// lobby cam is low, left-lobby is high

}
