using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{

	public float hAxis = 0;
	public bool dash = false;
	public bool jump = false;
	public bool slam = false;
	public bool jet = false;

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
