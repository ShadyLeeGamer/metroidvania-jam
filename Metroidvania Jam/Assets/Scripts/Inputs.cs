using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    // This script contains bools that are read by RobotMovement, RobotCombat, etc
    // // and modified by PlayerInput, EnemyInput, BossInput, etc

	// Raw inputs
	// These are modified by PlayerInputs to store Input.GetKey
	// // or by EnemyInputs for Enemy AI
	public bool Up = false;
	public bool Down = false;
	public bool Left = false;
	public bool Right = false;

	public bool ShootUp = false; // pan camera up / down, character looks up / down
	public bool ShootDown = false;
	public bool ShootLeft = false; // diagonal up / down when combined with ShootUp and ShootDown
	public bool ShootRight = false;

	public bool Jump = false;
	public bool SwapTool = false; // round-robin for now, todo NextTool PrevTool with E and Q

	public Vector2 Cursor = Vector2.zero;
	public bool Mouse1 = false;
	public bool Mouse2 = false;

	// KeyDown inputs
	public bool UpGetDown = false;
	public bool DownGetDown = false;
	public bool LeftGetDown = false; // for UI nav, double-tap
	public bool RightGetDown = false; // for UI nav, double-tap
	public bool JumpGetDown = false;
	public bool SwapGetDown = false;
	public bool Mouse1GetDown = false;
	public bool Mouse2GetDown = false;
	bool upWasPressed = false;
	bool downWasPressed = false;
	bool leftWasPressed = false;
	bool rightWasPressed = false;
	bool jumpWasPressed = false;
	bool swapWasPressed = false;
	bool mouse1WasPressed = false;
	bool mouse2WasPressed = false;
	void CalculateKeyDown() {
		if (upWasPressed) UpGetDown = false;
		else UpGetDown |= Up;
		if (downWasPressed) DownGetDown = false;
		else DownGetDown |= Down;
		if (leftWasPressed) LeftGetDown = false;
		else LeftGetDown |= Left;
		if (rightWasPressed) RightGetDown = false;
		else RightGetDown |= Right;
		if (jumpWasPressed) JumpGetDown = false;
		else JumpGetDown |= Jump;
		if (swapWasPressed) SwapGetDown = false;
		else SwapGetDown |= SwapTool;
		if (mouse1WasPressed) Mouse1GetDown = false;
		else Mouse1GetDown |= Mouse1;
		if (mouse2WasPressed) Mouse2GetDown = false;
		else Mouse2GetDown |= Mouse2;
	}
	public void ResetKeyDown() {
		// KeyDown
		if (Up) upWasPressed = true;
		else upWasPressed = false;
		if (Down) downWasPressed = true;
		else downWasPressed = false;
		if (Left) leftWasPressed = true;
		else leftWasPressed = false;
		if (Right) rightWasPressed = true;
		else rightWasPressed = false;
		if (Jump) jumpWasPressed = true;
		else jumpWasPressed = false;
		if (SwapTool) swapWasPressed = true;
		else swapWasPressed = false;
		if (Mouse1) mouse1WasPressed = true;
		else mouse1WasPressed = false;
		if (Mouse2) mouse2WasPressed = true;
		else mouse2WasPressed = false;
		// Call this in RobotMovement to simulate GetKeyDown
		UpGetDown = false;
		DownGetDown = false;
		LeftGetDown = false;
		RightGetDown = false;
		JumpGetDown = false;
		SwapGetDown = false;
		Mouse1GetDown = false;
		Mouse2GetDown = false;
		// Extra
		DoubleLeft = false;
		DoubleRight = false;
	}


	// Extra inputs
	public float Horizontal = 0;
	public bool DoubleLeft = false;
	public bool DoubleRight = false;
	bool doingLeft = false;
	bool reset = false;
	float dtTimer = -1;
	public float doubleTapTime = 0.4f;
	void CalculateExtra() {
		Horizontal = 0;
        if (Left && !Right) Horizontal = -1;
        if (Right && !Left) Horizontal = 1;
        
        // Double-tap
        if (dtTimer >= 0) {
        	// set / reset timer
        	if (dtTimer < doubleTapTime) {
        		dtTimer += Time.deltaTime;
        		// finish dash
        		if (!LeftGetDown && !RightGetDown) reset = true;
        		if (LeftGetDown && reset) {
        			DoubleLeft = true;
        			dtTimer = -1;
        		}
        		if (RightGetDown && reset) {
        			DoubleRight = true;
        			dtTimer = -1;
        		}
        	}
        	else dtTimer = -1;
        	if (doingLeft && RightGetDown) dtTimer = -1;
        	if (!doingLeft && LeftGetDown) dtTimer = -1;
        }
        // Initiate the dash
        if (dtTimer < 0) {
        	if (LeftGetDown) {
        		dtTimer = 0;
        		doingLeft = true;
        		reset = false;
        	}
        	if (RightGetDown) {
        		dtTimer = 0;
        		doingLeft = false;
        		reset = false;
        	}
        }
	}

	
	void Update() {
		CalculateKeyDown();
		CalculateExtra();
	}


}
