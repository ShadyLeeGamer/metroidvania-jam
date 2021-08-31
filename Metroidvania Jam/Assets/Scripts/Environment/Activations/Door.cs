using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Activated
{

    Transform door;
    void Start() {
    	door = transform.Find("Door");
    }
    public float moveTime = 0.5f;
    float timer = 0;
    public override void Update() {
    	base.Update();

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






}
