using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimations : MonoBehaviour
{

	SpriteRenderer sr;
	void Start() {
		sr = GetComponent<SpriteRenderer>();
	}

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
	float timer = -1;
	void Update() {
		if (timer < 0) return;
		else if (timer < swapTime)
			timer += Time.deltaTime;
		else {
			timer = 0;
			bool ended = NextFrame();
			if (ended && doingSingle) {
				timer = -1;
				if (destroy) Destroy(gameObject);
			}

		}
	}

	bool doingSingle = false;
	bool destroy = false;
	public void StartLoop() {
		doingSingle = false;
		destroy = false;
		timer = 0;
	}
	public void StartSingle() {
		doingSingle = true;
		destroy = false;
		timer = 0;
	}
	public void StartSingleDestroy() {
		doingSingle = true;
		destroy = true;
		timer = 0;
	}

}
