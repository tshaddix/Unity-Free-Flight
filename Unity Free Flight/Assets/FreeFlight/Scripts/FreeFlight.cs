using UnityEngine;
using UnityEngine.Internal;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityFreeFlight;

/// <summary>
/// Free Flight -- a Unity Component that adds flight to any unity object. 
/// </summary>
[RequireComponent (typeof(Rigidbody))]
public class FreeFlight : MonoBehaviour {
	
	public ModeManager modeManager;

	//=============
	//Unity Events
	//=============

	public void OnEnable () {
		if (modeManager == null)
			modeManager = new ModeManager ();
		modeManager.init (gameObject);
	}

	void SwitchModes(MovementModes newmode) {
		modeManager.switchModes (newmode);
	}

	void Start() {
		modeManager.start ();
	}
	
	/// <summary>
	/// Get input from the player 
	/// </summary>
	void Update() {
		modeManager.getInputs ();
	}
	
	/// <summary>
	/// In relation to Update() this is where we decide how to act on the user input, then
	/// compute the physics and animation accordingly
	/// </summary>
	void FixedUpdate () {	
		modeManager.applyInputs ();
	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		modeManager.switchModes (MovementModes.Ground);
	}


}
