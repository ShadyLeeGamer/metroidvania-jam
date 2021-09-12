using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Activator), typeof(Collider2D))]
public class Lever : MonoBehaviour
{
    Activator a;
    Transform lever;
    BoxCollider2D tmp;
    void Start() {
        a = GetComponent<Activator>();
        lever = transform.Find("Lever");
        tmp = GetComponent<BoxCollider2D>();
    }

    public float moveTime = 0.5f;
    float timer = 0;
    void Update() {
    	Collider2D lc = lever.Find("Lever").GetComponent<Collider2D>();
    	//FitColliderTo(tmp.GetComponent<BoxCollider2D>(), lc.GetComponent<BoxCollider2D>());
    	if (!moving) {
    		if (timer < moveTime) {
    			float progress = timer / moveTime;
    			MoveLever(progress);
    			timer += Time.deltaTime;
    			a.active = false;
    		}
    		else {
    			MoveLever(1);
    			//moving = !moving; // waggle test
    			timer = moveTime;
    			tmp.enabled = true;
    			tmp.offset = new Vector2(-offsetX, tmp.offset.y);
    			a.active = false;
    		}
    	}
    	else {
    		//GameObject found = SearchForCollision(lc, requiredLayer);
    		//if (found == null)
    		//	OnColExit();
    		if (timer > 0) {
    			float progress = timer / moveTime;
    			MoveLever(progress);
    			timer -= Time.deltaTime;
    			a.active = false;
    		}
    		else {
    			MoveLever(0);
    			//moving = !moving; // waggle test
    			timer = 0;
    			tmp.enabled = true;
    			tmp.offset = new Vector2(offsetX, tmp.offset.y);
    			a.active = true;
    		}
    	}

    }
    public float offsetX = 0.3f;
    public float maxAngle = 30;
    void MoveLever(float progress) {
    	lever.transform.localEulerAngles = (-maxAngle + 2*maxAngle * progress) * Vector3.forward;
    }


    public float minSpeed = 2f;
	public string requiredLayer = "Robot";
    bool moving = false;
    void OnCollisionEnter2D(Collision2D info) {
		GameObject other = info.collider.gameObject;
		// Layer check
		if (requiredLayer != "" && LayerMask.LayerToName(other.layer) != requiredLayer) return;
		// Speed check
		float inSpeed = info.relativeVelocity.magnitude * Mathf.Abs(Vector2.Dot(lever.right, info.relativeVelocity.normalized));
		if (inSpeed >= minSpeed) {
			OnSwapDir();
		}
    }
    public bool canUnpress = false;
    bool firstSwap = true;
    void OnSwapDir() {
    	if (!canUnpress) {
    		if (!firstSwap) return;
    		firstSwap = false;
    	}
    	
		moving = !moving;
		tmp.enabled = false;
		a.active = false;
    }



    GameObject SearchForCollision(Collider2D c, string layerName) {
		// Detect collisions (without extra script)
		List<Collider2D> results = new List<Collider2D>();
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask(layerName));
		c.OverlapCollider(filter, results);
		// Find a suitable result
		for (int i = 0; i < results.Count; i++) {
			if (results[i].gameObject.layer == LayerMask.NameToLayer(layerName))
				return results[i].gameObject;
		}
		return null;
	}
// todo: fix this
	/*void FitColliderTo(BoxCollider2D c1, BoxCollider2D c2) {
		c1.offset = c2.offset;
		c1.size = c2.size;
	}*/


}
