using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	public Vector3 offset = new Vector3(0, 0, -10);
    void Start() {
        FindPlayer();
    }
    Transform player;
    public void FindPlayer() {
		player = GameObject.Find("Player").transform;
	}

    public float moveSensitivity = 0.8f;
    public bool shaking = false;
    public float shakeIntensity = 1f;
    void Update() {
    	if (player == null) {
    		Debug.LogError("Camera couldn't find player!");
    		FindPlayer();
    		if (player == null)
				Debug.LogError("Camera couldn't find player!");
    	}

        Vector2 targetPos = player.position;
        int look = LookDirection();
        if (look != 0) {
        	targetPos += new Vector2(0, look*lookDistance);
        }
        targetPos += (Vector2)offset;
        targetPos = ClampWithinBorder(targetPos);
        if (shaking) {
        	targetPos += shakeIntensity * Random.insideUnitCircle;
        }
        // Apply position to camera
        Vector3 toPos = (Vector3)targetPos;
        toPos.z = offset.z;
        transform.position = Vector3.Lerp(transform.position, toPos, moveSensitivity);
    }


    // Pan up / down when ShootUp / ShootDown keys are held for lookTime
    public float lookDistance = 4f;
	public float lookTime = 0.5f;
	float lTimer = 0;
	int oldLook = 0;
	int LookDirection() {
		Inputs pInputs = player.GetComponent<Inputs>();
		int look = 0;
		if (pInputs.ShootUp && !pInputs.ShootDown)
			look = 1;
		if (pInputs.ShootDown && !pInputs.ShootUp)
			look = -1;
		bool stillPressed = (look != 0 && look == oldLook);
		oldLook = look;

		//Debug.Log(lTimer + " " + look + " " + oldLook);
		if (lTimer >= lookTime) {
			// Loosen the requirements once the timer is done
			if (stillPressed) return look;
			else look = 0;
		}
		else {
			bool nothingElse = !pInputs.ShootLeft && !pInputs.ShootRight;
			//Debug.Log(stillPressed + " " + nothingElse);
			if (stillPressed && nothingElse) {
				lTimer += Time.deltaTime;
				return 0;
			}
			else look = 0;
		}

		if (look == 0) lTimer = 0;
		return look;
	}

	// Keep camera within room border
	public List<Vector2> borderPoints;
	// Example on how to use. Camera can travel within Xs
	// 0,0 -> 1,4 -> 3,3 -> 2,2 -> 3,5 -> 1,7
	// X [X X]
	// X    X  [X X] X X
	// X             X X
	// X
	int clampIndex = -1;
	Vector2 ClampWithinBorder(Vector2 pos) {
		if (borderPoints.Count < 2) {
			Debug.LogError("Insufficient border points!");
			return pos;
		}
		Vector2 prevBP = borderPoints[0];
		for (int i = 0; i < borderPoints.Count-1; i++) {
			Vector2 BP = borderPoints[i+1];
			Vector2 clamped = ClampSingleRect(pos, prevBP, BP);
			// pos is in this region
			if (clamped == pos) {
				clampIndex = i;
				return pos;
			}
		}
		// If not inside any regions, clamp to the prev region.
		if (clampIndex == -1) return pos;
		return ClampSingleRect(pos, borderPoints[clampIndex], borderPoints[clampIndex+1]);
	}
	Vector2 ClampSingleRect(Vector2 pos, Vector2 v1, Vector2 v2) {
		if (v1.x <= v2.x)
			pos.x = Mathf.Clamp(pos.x, v1.x, v2.x);
		else
			pos.x = Mathf.Clamp(pos.x, v2.x, v1.x);
		if (v1.y <= v2.y)
			pos.y = Mathf.Clamp(pos.y, v1.y, v2.y);
		else
			pos.y = Mathf.Clamp(pos.y, v2.y, v1.y);
		return pos;
	}

}
