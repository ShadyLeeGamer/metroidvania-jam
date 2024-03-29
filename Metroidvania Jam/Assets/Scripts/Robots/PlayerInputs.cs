using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inputs))]
public class PlayerInputs : MonoBehaviour
{

	// Todo: move to Settings
    [HideInInspector] public KeyCode UpCode = KeyCode.W;
    [HideInInspector] public KeyCode DownCode = KeyCode.S;
    [HideInInspector] public KeyCode LeftCode = KeyCode.A;
    [HideInInspector] public KeyCode RightCode = KeyCode.D;

    [HideInInspector] public KeyCode ShootUpCode = KeyCode.UpArrow;
    [HideInInspector] public KeyCode ShootDownCode = KeyCode.DownArrow;
    [HideInInspector] public KeyCode ShootLeftCode = KeyCode.LeftArrow;
    [HideInInspector] public KeyCode ShootRightCode = KeyCode.RightArrow;

// super weird bug: when this variable is called "JumpCode" it alternates between a value of KeyCode.W and KeyCode.Space
    [HideInInspector] public KeyCode JCode = KeyCode.Space;
    [HideInInspector] public KeyCode SwapCode = KeyCode.E;


    Inputs inp;
    CameraController cc;
    void Start() {
        inp = GetComponent<Inputs>();
        cc = Camera.main.GetComponent<CameraController>();
    }
    void Update() {
// super weird bug: see above
        //Debug.Log(JCode);
        //Debug.Log(Input.GetKey(JCode));
        if ((cc.sc != null && cc.sc.paused) || cc.titleScreen) return;
        UpdateRaw();
    }
    void UpdateRaw() {
        inp.Up = Input.GetKey(UpCode);
        inp.Down = Input.GetKey(DownCode);
        inp.Left = Input.GetKey(LeftCode);
        inp.Right = Input.GetKey(RightCode);

        inp.ShootUp = Input.GetKey(ShootUpCode);
        inp.ShootDown = Input.GetKey(ShootDownCode);
        inp.ShootLeft = Input.GetKey(ShootLeftCode);
        inp.ShootRight = Input.GetKey(ShootRightCode);

        inp.Jump = Input.GetKey(JCode);
        inp.SwapTool = Input.GetKey(SwapCode);

        inp.Cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        inp.Mouse1 = Input.GetMouseButton(0);
        inp.Mouse2 = Input.GetMouseButton(1);
    }



}
