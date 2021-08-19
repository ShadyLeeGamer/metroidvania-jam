using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{

	[HideInInspector] public Vector2 mouseWorld = Vector2.zero;
	[HideInInspector] public float hAxis = 0;
	[HideInInspector] public bool dash = false;
	[HideInInspector] public bool jump = false;
	[HideInInspector] public bool slam = false;
	[HideInInspector] public bool jet = false;
    [HideInInspector] public bool mouse1 = false;
    [HideInInspector] public bool mouse2 = false;
	[HideInInspector] public bool shoot = false;
	[HideInInspector] public bool lookUp = false;
	[HideInInspector] public bool lookDown = false;
	void Start() {
		Reset();
	}
    void Update() {
        UpdateInputs();
    }



    string HorizontalAxis = "Horizontal";
    KeyCode DashCode = KeyCode.LeftShift;
    KeyCode JumpCode = KeyCode.W;
    KeyCode SlamCode = KeyCode.S;
    KeyCode JetCode = KeyCode.Space;
	KeyCode ShootCode = KeyCode.Z;
	KeyCode LookUpCode = KeyCode.UpArrow;
	KeyCode LookDownCode = KeyCode.DownArrow;
	
    void UpdateInputs() {
    	mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	hAxis = Input.GetAxis(HorizontalAxis);
    	dash |= Input.GetKey(DashCode);
    	jump |= Input.GetKey(JumpCode);
    	slam |= Input.GetKey(SlamCode);
    	jet |= Input.GetKey(JetCode);
		shoot |= Input.GetKey(ShootCode);
        mouse1 |= Input.GetMouseButtonDown(0);
        mouse2 |= Input.GetMouseButtonDown(1);
		lookUp |= Input.GetKey(LookUpCode);
		lookDown |= Input.GetKey(LookDownCode);
    }
    public void Reset() {
    	hAxis = 0;
    	dash = false;
    	jump = false;
    	slam = false;
    	jet = false;
		shoot = false;
        mouse1 = false;
        mouse2 = false;
		lookUp = false;
		lookDown = false;
    }



}
