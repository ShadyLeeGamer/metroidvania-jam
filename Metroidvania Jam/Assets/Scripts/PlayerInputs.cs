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
    void UpdateInputs() {
    	mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	hAxis = Input.GetAxis(HorizontalAxis);
    	dash |= Input.GetKey(DashCode);
    	jump |= Input.GetKey(JumpCode);
    	slam |= Input.GetKey(SlamCode);
    	jet |= Input.GetKey(JetCode);
    }
    public void Reset() {
    	hAxis = 0;
    	dash = false;
    	jump = false;
    	slam = false;
    	jet = false;
    }



}
