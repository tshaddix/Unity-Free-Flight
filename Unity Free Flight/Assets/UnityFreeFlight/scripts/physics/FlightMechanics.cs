﻿using UnityEngine;
using System.Collections;

/*
 * Flight Mechanics is responsible for taking user input and 
 * manipulating the wing for various physics effects.
 * 
 * It handles the math behind flapping, flarring, diving,
 * to change how physics effects the flight object.
 * 
 * Example: In order to do a 'dive', this class reduces the
 * wing area. flight physics then calculates smaller lift forces,
 * allowing the flight object to plummet.
 */ 

public class FlightMechanics : FlightPhysics {

	public FlightMechanics(Rigidbody rb) : base(rb) {

	}


	public void wingFlare() {
		return;
	}

	public void wingFold(float left, float right) {
		setWingPosition (left, right);

	}

	public void thrust(float forceNewtons) {
//		rigidbody.AddForce(new Vector3(0.0f, 0.0f, forceNewtons); 
	}


	//Get (in degrees) angle of attack for zero lift coefficient
	private float getAOAForZeroLCOF() {
		return 0.0f;
	}




}