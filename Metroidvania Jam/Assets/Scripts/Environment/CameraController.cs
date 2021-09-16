using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	// A ref to SettingsController for easy access to Canvas / Audio, from robots / enemies / bullets
	// // gets assigned by Canvas
    [HideInInspector] public SettingsController sc;

	public Vector3 offset = new Vector3(0, 0, -10);
    void Start() {
        FindPlayer();
        ResetDiscovered();
    }
    Transform player;
    public void FindPlayer() {
		player = GameObject.Find("Player").transform;
	}

    public float moveSensitivity = 0.8f;
    public bool shaking = false;
    public float shakeIntensity = 1f;
    public float winY = 50;
    bool won = false;
    void Update() {
    	if (titleScreen) {
    		transform.position = (Vector3)titleScreenPos + offset;
    		return;
    	}
    	if (!won && transform.position.y >= winY) {
    		won = true;
    		sc.Win();
    	}


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
		if (pInputs == null) return 0;
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
	public List<bool> discovered;
	void ResetDiscovered() {
		discovered.Clear();
		for (int i = 0; i < borderPoints.Count-1; i++)
			discovered.Add(false); // false
	}
	Vector2 ClampWithinBorder(Vector2 pos) {
		if (borderPoints.Count < 2) {
			Debug.LogError("Insufficient border points!");
			return pos;
		}
		Vector2 minDist = Vector2.zero;
		Vector2 prevBP = borderPoints[0];
		for (int i = 0; i < borderPoints.Count-1; i++) {
			Vector2 BP = borderPoints[i+1];
			Vector2 delta = BP - prevBP;
			float area = delta.x * delta.y;
			//Debug.Log(prevBP + " " + BP + " " + area);
			if (area == 0) {
				prevBP = BP;
				continue;
			}
			Vector2 clamped = ClampSingleRect(pos, prevBP, BP);
			float distance = Vector2.Distance(clamped, pos);
			
			if (distance < 4) {
				discovered[i] = true;
				// pos is in this region - no need to clamp
				if (distance < 0.001f)
					return pos;
			}

			if (discovered[i] && (distance < minDist.y || i == 0))
				minDist = new Vector2(i, distance);
			prevBP = BP;
		}
		// If not inside any regions, clamp to the closest region.
		int index = Mathf.RoundToInt(minDist.x);
		return ClampSingleRect(pos, borderPoints[index], borderPoints[index+1]);
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


	// Title screen / cutscenes
	public bool titleScreen = true;
    public Vector2 titleScreenPos;
    public Vector2 startPos;
    public void StartGame() {
    	titleScreen = false;
    	transform.position = (Vector3)startPos + offset;
    }
    public void ExitGame() {
    	sc.LoadScene("World");
    }





	// To use room debug:
	// Enable Gizmos in game view window (there's a button)
	public bool debug = false;
	public bool gizmoCamPos = true;
	public bool gizmoRooms = true;
    void OnDrawGizmos() {
    	if (!debug) return;

    	float nonZero = 0;
    	Vector2 prevBP = borderPoints[0];
		for (int i = 0; i < borderPoints.Count-1; i++) {
			Vector2 BP = borderPoints[i+1];
			Vector2 center = (prevBP + BP) / 2;
			Vector2 size = (BP - prevBP);
			if (size.x < 0) size.x = -size.x;
			if (size.y < 0) size.y = -size.y;
			float progress = nonZero / (borderPoints.Count-1);
			
			if (size.x != 0 && size.y != 0) {
				Color c;
				if (gizmoRooms) {
					Vector3 screen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)) - Camera.main.ScreenToWorldPoint(Vector2.zero);
					//Debug.Log(screen + " " + Screen.width + " " + Screen.height + " " + Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)) + " " + Camera.main.ScreenToWorldPoint(Vector2.zero));
					c = Color.yellow + progress * (Color.red - Color.yellow);
					c.a = 0.1f;
					Gizmos.color = c;
					Gizmos.DrawCube(new Vector3(center.x, center.y, 5), new Vector3(size.x + screen.x, size.y + screen.y, 1));
				}
				if (gizmoCamPos) {
					c = Color.green + progress * (Color.blue - Color.green);
					c.a = 0.2f;
					Gizmos.color = c;
					Gizmos.DrawCube(new Vector3(center.x, center.y, -5), new Vector3(size.x, size.y, 1));
				}
			}
			else nonZero++;

			prevBP = BP;
    	}
    }





}
