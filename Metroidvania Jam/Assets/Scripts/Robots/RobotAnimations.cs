using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnimations : MonoBehaviour
{

    

    [HideInInspector] public Transform spritesParent;
    [HideInInspector] public Transform guns;
    [HideInInspector] public GameObject outletPulse;
    [HideInInspector] public RobotMovement rm;
    [HideInInspector] public RobotShooting rs;
	void Start() {
		rm = GetComponent<RobotMovement>();
        rs = GetComponent<RobotShooting>();
		spritesParent = transform.Find("Sprites");
        guns = spritesParent.Find("RoboGuns");
		chainParent = GameObject.Find("Environment").transform.Find("Projectiles").Find("Hooks");
        outletPulse = spritesParent.Find("Robot Outlet").Find("Black Pulse").gameObject;
        if (gameObject.name == "Player") outletPulse.SetActive(false);
	}
	void Update() {
		faceTimer += Time.deltaTime;
		CheckTurnOver();
	}



// extra: SpriteAnim lights or add actual lights
    public float Energy = 0.5f; // 0-1
    public float chargeTime = 15; // seconds until full charge
    public GameObject chargeLightPrefab;
    List<Transform> chargeLights = new List<Transform>();
    public float chargeLightCreateTime = 5f;
    float chcTimer = 0;
    public float chargeLightTravelTime = 5f;
    List<float> chargeLightTimers = new List<float>();
    void SpawnChargeLight() {
        Transform outlet = rm.GetHookAttachedTo().transform;
        if (outlet == null) {
            Debug.LogError("Cant spawn charge particle at null outlet.");
            return;
        }
        Transform cl = Instantiate(chargeLightPrefab, chainParent.parent).transform;
        cl.position = PosAlongChain(1);
        chargeLights.Add(cl);
        chargeLightTimers.Add(0);
    }
    public void Charge() {
        if (rm.GetHook() == null) return;
        if (Energy < 0) Energy = 0;
        if (Energy < 1) {
            Energy += Time.fixedDeltaTime / chargeTime;
            Energy = Mathf.Clamp(Energy, 0, 1);
            // Spawn purple light
            if (chcTimer < chargeLightCreateTime)
                chcTimer += Time.fixedDeltaTime;
            else {
                chcTimer = 0;
                SpawnChargeLight();
            }
        }
        // Move purple lights along chain
        for (int i = 0; i < chargeLights.Count; i++) {
            chargeLightTimers[i] += Time.fixedDeltaTime;
            if (chargeLightTimers[i] >= chargeLightTravelTime) {
                DestroyChargeIndex(i);
                i--;
                continue;
            }
            chargeLights[i].position = PosAlongChain(1 - chargeLightTimers[i] / chargeLightTravelTime);
        }
    }
    public void DestroyCharges() {
        for (int i = 0; i < chargeLights.Count; i++)
            DestroyChargeIndex(i);
    }
    void DestroyChargeIndex(int i) {
        // todo: chargeLights[i].GetComponent<SpriteAnimations>().SingleDestroy();
        Destroy(chargeLights[i].gameObject);
        chargeLights.RemoveAt(i);
        chargeLightTimers.RemoveAt(i);
    }

// bug: Transfer / Energy. doesnt move along wire at constant speed: short wire = slow
    public float transferTime = 1;
    float transferTimer = -1;
    GameObject transferLight;
    public GameObject transferLightPrefab;
    public bool Transfer() {
        if (transferTimer < 0) {
            transferTimer = 0;
            // Create blue light
            transferLight = Instantiate(transferLightPrefab, chainParent.parent);
            transferLight.transform.position = rs.GetGun().Find("GunTip").position;
            gameObject.name = robotName;
            transferLight.name = "Player";
            Camera.main.GetComponent<CameraController>().FindPlayer();
            // Cannot move during transfer
            GameObject robot = rm.GetHookAttachedTo().transform.parent.parent.gameObject;
            GetComponent<PlayerInputs>().enabled = false;
            if (robot.GetComponent<RobotAnimations>().robotName == "Enemy")
                robot.GetComponent<EnemyRobotInputs>().enabled = false;
            rm.Freeze();
            robot.GetComponent<RobotMovement>().Freeze();
            outletPulse.SetActive(false);
            robot.GetComponent<RobotAnimations>().outletPulse.SetActive(false);
        }
        else if (transferTimer < transferTime) {
            transferTimer += Time.fixedDeltaTime;
            // Move blue light along hook chain
            transferLight.transform.position = PosAlongChain(transferTimer / transferTime);
        }
        else {
            transferTimer = -1;
            // Transfer to robot
            Destroy(transferLight);
            GameObject robot = rm.GetHookAttachedTo().transform.parent.parent.gameObject;
            robot.name = "Player";
            Camera.main.GetComponent<CameraController>().FindPlayer();
            Destroy(GetComponent<PlayerInputs>());
            robot.AddComponent<PlayerInputs>();
            rm.UnFreeze();
            robot.GetComponent<RobotMovement>().UnFreeze();
            // Enable / disable to preserve settings
            if (robotName == "Enemy")
                GetComponent<EnemyRobotInputs>().enabled = true;
            if (robot.GetComponent<RobotAnimations>().robotName == "Enemy")
                robot.GetComponent<EnemyRobotInputs>().enabled = false;
            outletPulse.SetActive(true);
            robot.GetComponent<RobotAnimations>().outletPulse.SetActive(false);
            // Prevent infinite transfer
            if (rm.GetHook() != null) {
                rm.ShootHook();
            }
            rm.RetractHook(false);
            return false;
        }
        return true;
    }
    public string robotName = "Robot";


    void PointGunAngle(float angle) {
        guns.localEulerAngles = new Vector3(guns.localEulerAngles.x, guns.localEulerAngles.y, angle);
    }
    public void PointGunDirection(Vector2 direction) {
        if (direction == Vector2.zero) {
            Debug.LogError("gun direction is invalid");
            return;
        }
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        ResetFaceTimer();
        // gun trigger always facing down
        if (angle < -90) {
            FaceRight(false);
            angle = -180 - angle;
        } 
        else if (angle > 90) {
            FaceRight(false);
            angle = 180 - angle;
        }
        else if (angle != 90 && angle != -90) {
            FaceRight(true);
        }
        PointGunAngle(angle);
    }
    void PointGunTowards(Transform t) {
        PointGunDirection(t.position - transform.position);
    }


	// If upside down, start turnover anim
	public float turnOverTime = 0.3f;
	float tOTimer = 0;
	bool onceTurtled = false;
	void CheckTurnOver() {
		// If upside-down and onGround and not moving
		bool turtled = rm.GetTurtle() && rm.rb.velocity.magnitude < 0.1f;
		onceTurtled |= turtled;
		if (!onceTurtled) return;
		// Once turtled, wait for turnOverTime
		if (tOTimer < turnOverTime) {
			tOTimer += Time.deltaTime;
		}
		else {
			// Reset rotation
			tOTimer = 0;
			onceTurtled = false;
			transform.eulerAngles = Vector3.zero;
		}
	}


	// Reflect sprites to change look direction
	public float faceTime = 0.3f;
	float faceTimer = 0;
	public void FaceRight(bool right) {
		if (faceTimer < faceTime) return;
		faceTimer = 0;
		// Rotate aroud y-axis (reflect)
		if (right)
			spritesParent.localEulerAngles = Vector2.zero;
		else
			spritesParent.localEulerAngles = 180 * Vector2.up;
	}
	public void ResetFaceTimer() {
		faceTimer = faceTime;
	}
	public float minFaceSpeed = 0.01f;
	public void FaceVelocity() {
		if (rm.rb.velocity.x > minFaceSpeed)
			FaceRight(true);
		if (rm.rb.velocity.x < -minFaceSpeed)
			FaceRight(false);
	}
	public void FaceDirection(Vector2 direction) {
		if (direction.x > 0) FaceRight(true);
		if (direction.x < 0) FaceRight(false);
	}


	// Hook for wire
	public float minHookAngle = 10;
	public void HookFaceAngle(float angle) {
		Transform hook = rm.GetHook().transform;
		if (hook == null && angle < minHookAngle) return;
		hook.eulerAngles = new Vector3(0, 0, angle);
	}
	public void HookFaceVelocity() {
		HookFaceDirection(rm.GetHook().velocity);
	}
	public void HookFaceDirection(Vector2 direction) {
		HookFaceAngle(Vector2.SignedAngle(Vector2.right, direction));
	}

	// Chain for the grappling hook
    public GameObject chainLinkPrefab;
    public float distBetweenLinks = 0.3f;
    public float amplitude = 0.4f;
    public float stretchAmplitude = 0.01f;
    public float frequency = 5;
    public float stretchFreq = 1f;
    public float loopy = 0.2f;
    public float stretchLoopy = 0f;
    public float parabolaFall = 1;
    public float stretchPFall = 0;
    float numLinksMultiplier = 3; // smaller = better performance but less sensitive results
    Transform chainParent;
    public void UpdateChain(string curveName, bool addTrochoid, bool lerp) {
    	Vector2 pos2 = rm.hookGunTip.position;
    	Vector2 pos1 = rm.GetHook().transform.position;
    	if (pos1 == pos2) return;
    	if (rm.GetHookAttachedTo() == null) HookFaceDirection(pos1 - pos2);
    	else HookFaceDirection(-rm.GetHookAttachedTo().transform.up);
    	float length = rm.maxChainLength;
    	float distance = Vector2.Distance(pos1, pos2); // approx

    	// Evenly space chain links
    	int nLinks = Mathf.Clamp(Mathf.RoundToInt(numLinksMultiplier * length / distBetweenLinks), 1, 300);

    	// Calculate base chain curve
    	bool pOnLeft = (pos1.x >= pos2.x);
    	List<Vector2> curve = new List<Vector2>();
    	if (curveName == "Line")
    		curve = CreateLine(2, pos1, pos2);
    	else if (curveName == "Parabola") {
    		float pf = parabolaFall;
    		if (lerp) {
    			pf = Mathf.Lerp(parabolaFall, stretchPFall, distance / length);
    		}
    		if (pOnLeft) pf = -pf;
    		curve = CreateHangingParabola(nLinks, pos1, pos2, pf);
    	}
    	else
    		curve = CreateLine(2, pos1, pos2);

    	// Prep the Trochoid points, and add to curve
		List<Vector2> points;
		if (addTrochoid) {
			float a = amplitude;
			float l = loopy;
			float f = frequency;
			if (lerp) {
				a = Mathf.Lerp(amplitude, stretchAmplitude, distance / length);
				l = Mathf.Lerp(loopy, stretchLoopy, distance / length);
				f = Mathf.Lerp(frequency, stretchFreq, distance / length);
			}
			points = SampleTrochoid(nLinks+1, 0, l, f);
    		if (pOnLeft) a = -a;
    		ScalePoints(points, 1, a);
    		FitPointsToCurve(points, curve);
		}
		else points = curve;
		EvenlySpacePoints(points, distBetweenLinks);
    	
    	// Spawn new links at pos1 when extending length
    	// // Links are spawned between points
    	int missingLinks = points.Count - chainLinks.Count - 1;
    	for (int i = 0; i < missingLinks; i++) // spawn missing
    		AddLinkAt(0);
    	for (int i = 0; i < -missingLinks; i++) // delete extra
    		DestroyLinkAt(0);
    	// Move links to correct locations
    	for (int i = 0; i < chainLinks.Count; i++) {
    		chainLinks[i].localPosition = (points[i] + points[i+1]) / 2;
    		// Rotate the link to join up with others
    		float angle = Vector2.SignedAngle(Vector2.up, points[i+1] - points[i]);
    		chainLinks[i].eulerAngles = new Vector3(0, 0, angle);
    	}
    }
    List<Transform> chainLinks = new List<Transform>();
    void AddLinkAt(int index) {
    	GameObject link = Instantiate(chainLinkPrefab, chainParent);
    	chainLinks.Insert(index, link.transform);
    }
    void DestroyLinkAt(int index) {
    	Destroy(chainLinks[index].gameObject);
    	chainLinks.RemoveAt(index);
    }
    public void DestroyChain() {
    	for (int i = 0; i < chainLinks.Count; i++)
    		Destroy(chainLinks[i].gameObject);
    	chainLinks.Clear();
    }
    public Vector2 PosAlongChain(float progress) {
    	progress = 1-progress;
    	int index = Mathf.RoundToInt(Mathf.Clamp(progress * (chainLinks.Count-1), 0, chainLinks.Count-2));
        if (chainLinks.Count == 1) return chainLinks[0].position;
        if (chainLinks.Count == 0) return (rm.GetHook().position + (Vector2)transform.position) / 2;
    	float indexProgress = (float)index / (chainLinks.Count-1);
    	float nextIndexProgress = (float)(index+1) / (chainLinks.Count-1);
    	float progressBetween = (progress - indexProgress) / (nextIndexProgress - indexProgress);
    	return Vector2.Lerp(chainLinks[index].position, chainLinks[index+1].position, progressBetween);
    }
    List<Vector2> CreateLine(int n, Vector2 pos1, Vector2 pos2) {
    	List<Vector2> points = new List<Vector2>();
    	for (int i = 0; i < n; i++) {
    		float progress = (float)i / (n-1);
    		Vector2 PT = pos1 + progress * (pos2 - pos1);
    		points.Add(PT);
    	}
    	return points;
    }
    List<Vector2> CreateHangingParabola(int n, Vector2 pos1, Vector2 pos2, float scale) {
    	if (scale == 0) scale = 0.001f;
        if (Mathf.Abs(pos2.x - pos1.x) < 0.001f) pos2.x += 0.001f;
    	List<Vector2> points = new List<Vector2>();
    	float slope = (pos2.y - pos1.y) / (pos2.x - pos1.x);
    	float cx = 0.5f - slope/2/scale; // derived formula
    	for (int i = 0; i < n; i++) {
    		float progress = (float)i / (n-1);
    		float y = (pos2.x - pos1.x) * scale * Mathf.Pow(progress - cx, 2);
    		Vector2 PT = new Vector2(pos1.x + progress * (pos2.x - pos1.x), y);
    		points.Add(PT);
    	}
    	Vector2 minEndpoint = new Vector2(0, Mathf.Min(points[0].y, points[points.Count-1].y));
    	Vector2 minPos = new Vector2(0, Mathf.Min(pos1.y, pos2.y));
    	for (int i = 0; i < points.Count; i++)
    		points[i] += minPos - minEndpoint;
    	return points;
    }


    // Generates a single point at location t (0 <= t <= 1) on a Trochoid curve
    // // Returns Vector2s within range x: 0 to 1, y:-1 to 1
    Vector2 Trochoid(float t, float offset, float loopy, float frequency) {
    	float angle = frequency * Mathf.PI * (t + offset);
    	float x = t + loopy/frequency * Mathf.Sin(angle);
    	float y = Mathf.Cos(angle);
    	x = Mathf.Clamp(x, 0, 1);
    	y = Mathf.Clamp(y, -1, 1);
    	return new Vector2(x, y);
    }
    // Creates a list of points from the Trochoid
    List<Vector2> SampleTrochoid(int n, float offset, float loopy, float frequency) {
    	List<Vector2> points = new List<Vector2>();
    	for (int i = 0; i < n; i++) {
    		float t = (float)i / (n-1);
    		Vector2 pos = Trochoid(t, offset, loopy, frequency);
    		points.Add(pos);
    	}
    	return points;
    }
    void ScalePoints(List<Vector2> points, float xMax, float yMax) {
    	for (int i = 0; i < points.Count; i++) {
    		points[i] = new Vector2(points[i].x * xMax, points[i].y * yMax);
    	}
    }
    public float ampSmoothingPlayer = 0.3f;
    public float ampSmoothingHook = 0.1f;
    void FitPointsToCurve(List<Vector2> points, List<Vector2> curvePoints) {
    	if (points.Count < 0 || curvePoints.Count < 2) {
    		Debug.LogError("Need at least 2 curve points for curve match.");
    		return;
    	}
    	// assumes points list has x range: 0 to 1
    	for (int i = 0; i < points.Count; i++) {
    		float progress = points[i].x;
    		// Find the 2 curvePoints before and after this point
    		int curveIndex = Mathf.RoundToInt(progress * (curvePoints.Count-2)); // 0 to curvePoints.Count-2
    		Vector2 curveP = curvePoints[curveIndex];
    		Vector2 nextCurveP = curvePoints[curveIndex+1];
    		// Calculate the interpolated point
    		Vector2 pos = curveP + progress * (nextCurveP - curveP);
    		Vector2 perpDirection = Vector2.Perpendicular(nextCurveP - curveP).normalized;
    		float clampAmp = 1;
    		if (progress < ampSmoothingPlayer) clampAmp = progress / ampSmoothingPlayer;
    		if (progress > (1-ampSmoothingHook)) clampAmp = 1 - (progress - (1-ampSmoothingHook)) / ampSmoothingHook;
    		clampAmp = Mathf.Clamp(clampAmp, -1, 1);
    		perpDirection *= points[i].y * clampAmp;
    		pos += perpDirection;
    		points[i] = pos;
    	}
    }
    void EvenlySpacePoints(List<Vector2> points, float distance) {
    	float length = PathLength(points);
    	List<Vector2> even = new List<Vector2>();
    	even.Add(points[0]);
    	Vector2 firstWithinDistance = points[0];
    	for (int i = 0; i < points.Count-1; i++) {
    		Vector2 PT = points[i+1];
    		if (Vector2.Distance(firstWithinDistance, PT) > distance) {
    			Vector2 pos = firstWithinDistance + distance * (PT - firstWithinDistance).normalized;
    			even.Add(pos);
    			firstWithinDistance = pos;
    			i--;
    		}
    	}
    	even.Add(points[points.Count-1]);
    	points.Clear();
    	points.AddRange(even);
    }
    void RemoveDuplicatePoints(List<Vector2> points) {
    	Vector2 PT = points[0];
    	for (int i = 0; i < points.Count-1; i++) {
    		Vector2 nextPT = points[i+1];
    		if (Vector2.Distance(PT, nextPT) == 0) {
    			points.RemoveAt(i+1);
    			i--;
    		}
    		else PT = nextPT;
    	}
    }
    float PathLength(List<Vector2> points) {
    	if (points.Count < 2) {
    		Debug.LogError("Need at least 2 points for path length.");
    		return -1;
    	}
    	float length = 0;
    	Vector2 PT = points[0];
    	for (int i = 0; i < points.Count-1; i++) {
    		Vector2 nextPT = points[i+1];
    		float d = Vector2.Distance(PT, nextPT);
    		length += d;
    		PT = nextPT;
    	}
    	return length;
    }




    public bool debug = false;
    public DebugVars debugVariables;
    void OnDrawGizmos() {
    	if (!debug) return;
    	Gizmos.color = Color.black;
    	/*List<Vector2> points = SampleTrochoid(debugVariables.numPoints, 
    		debugVariables.t, debugVariables.l, debugVariables.f);
    	ScalePoints(points, 1, debugVariables.a);
    	List<Vector2> line = new List<Vector2>();
    	line.Add(Vector2.zero);
    	line.Add(debugVariables.endPos);
    	FitPointsToCurve(points, line);*/
    	List<Vector2> points = CreateHangingParabola(debugVariables.numPoints, debugVariables.startPos, debugVariables.endPos, debugVariables.parabolaScale);
    	for (int i = 0; i < points.Count; i++) {
    		Gizmos.DrawSphere(points[i], debugVariables.pointSize);
    	}
    }




    

}
[System.Serializable]
public class DebugVars {
	// Trochoid
    public float a = 1;
    public float l = 0;
    public float f = 1;
    public int numPoints = 100;
    public float pointSize = 0.1f;
    public Vector2 startPos = Vector2.zero;
    public Vector2 endPos = new Vector2(2, 1);
    public float t = 0;
    public float spaceDistance = 0.1f;
    public float parabolaScale = 1;
}