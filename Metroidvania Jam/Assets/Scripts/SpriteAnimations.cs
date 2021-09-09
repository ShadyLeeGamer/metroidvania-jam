using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimations : MonoBehaviour
{
	
	SpriteRenderer sr;
	void Start() {
		sr = GetComponent<SpriteRenderer>();

		if (loopOnStart) StartLoop();
	}
	public bool loopOnStart = false;

	public List<Sprite> animationFrames;
	int index = 0;
	bool NextFrame() {
		sr.sprite = animationFrames[index];
		index++;
		if (index >= animationFrames.Count) {
			index = 0;
			return true;
		}
		return false;
	}
	public float swapTime = 0.2f;
	float swapTimer = -1;
	void DoSpriteSwaps() {
		if (swapTimer < 0) return;
		else if (swapTimer < swapTime)
			swapTimer += Time.deltaTime;
		else {
			swapTimer = 0;
			finished = NextFrame();
			if (finished && doingSingle) {
				swapTimer = -1;
				if (disableCollider) {
					Collider2D col = GetComponent<Collider2D>();
					if (col != null) col.enabled = false;
				}
				if (destroy) Destroy(gameObject);
			}
		}
	}
	public RenderData data;
	float renderTimer = 0;
	void DoRenderAdjust() {
		renderTimer += Time.deltaTime;
		if (renderTimer >= data.MaxTime() && !doingSingle)
			renderTimer = 0;
		sr.color = data.GetColor(renderTimer);
		Vector2 scale = data.GetScale(renderTimer);
		transform.localScale = new Vector3(scale.x, scale.y, 1);
	}
	void Update() {
		if (animationFrames.Count > 0) DoSpriteSwaps();
		if (data.GetCount() > 0) DoRenderAdjust();
	}

	bool doingSingle = false;
	bool destroy = false;
	bool disableCollider = false;
	[HideInInspector] public bool finished = false; // used to tell other scripts when a single loop has finished
	public void StartLoop() {
		Reset();
		finished = true;
	}
	public void StartSingle() {
		Reset();
		doingSingle = true;
	}
	public void StartSingleDestroy() {
		Reset();
		doingSingle = true;
		destroy = true;
	}
	public void StartSingleCollider() {
		Reset();
		doingSingle = true;
		disableCollider = true;
	}

	
	public void Reset() {
		doingSingle = false;
		destroy = false;
		disableCollider = false;
		swapTimer = 0;
		renderTimer = 0;
		finished = false;
	}

}
[System.Serializable]
public class RenderData {
	public List<float> times;
	public List<Color> colors;
	public List<Vector2> scales;

	public int GetCount() {
		return times.Count;
	}
	public float MaxTime() {
		return times[times.Count-1];
	}
	// x: index
	// y: progress to next index
	public Vector2 GetIndex(float time) {
		int index = times.Count-2;
		for (int i = 1; i < times.Count; i++) {
			if (time < times[i]) {
				index = i-1;
				break;
			}
		}
		float progress = (time - times[index]) / (times[index+1] - times[index]);
		return new Vector2(index, progress);
	}
	public Color GetColor(float time) {
		Vector2 data = GetIndex(time);
		int index = Mathf.RoundToInt(data.x);
		Color c1 = colors[index];
		Color c2 = colors[index+1];
		return Color.Lerp(c1, c2, data.y);
	}
	public Vector2 GetScale(float time) {
		Vector2 data = GetIndex(time);
		int index = Mathf.RoundToInt(data.x);
		Vector2 v1 = scales[index];
		Vector2 v2 = scales[index+1];
		return Vector2.Lerp(v1, v2, data.y);
	}
}