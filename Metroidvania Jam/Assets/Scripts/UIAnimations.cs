using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIAnimations : MonoBehaviour
{

    Image img;
    void Start() {
        img = GetComponent<Image>();
    }

    // temp fix
    public bool startOrEnd = true;
    public UIAnimations nextAnim;

    public RenderDataUI data;
    float renderTimer = 0;
    void DoRenderAdjust() {
        renderTimer += Time.deltaTime;
        if (renderTimer >= data.MaxTime()) {
            renderTimer = 0;
            MenuSwapper m = GetComponent<MenuSwapper>();
            if (m != null) m.Swap();
            if (nextAnim != null)
                nextAnim.StartSingle();
            finished = true;
            // temp fix
            CameraController cc = Camera.main.GetComponent<CameraController>();
            if (startOrEnd) cc.StartGame();
            else cc.ExitGame();
        }
        img.color = data.GetColor(renderTimer);
    }
    void Update() {
        if (data.GetCount() > 0 && !finished)
            DoRenderAdjust();
    }

    [HideInInspector] public bool finished = false; // used to tell other scripts when a single loop has finished
    public void StartSingle() {
        Reset();
    }
    public void Reset() {
        renderTimer = 0;
        finished = false;
    }

}
[System.Serializable]
public class RenderDataUI {
    public List<float> times;
    public List<Color> colors;

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
}
