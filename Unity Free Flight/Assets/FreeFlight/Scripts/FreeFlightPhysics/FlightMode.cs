using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityFreeFlight;

namespace UnityFreeFlight {

	/// <summary>
	/// Apply flight mechanics on a game object when enabled by the Mode Manager
	/// </summary>
	[Serializable]
	public class FlightMode : BaseMode {
	
		public FlightInputs flightInputs;
		public FlightMechanics flightMechanics;
		public FlightPhysics flightPhysics;

		public void setupMechanics() {

			foreach (Mechanic mech in mechanics) {
				mech.init (gameObject, flightPhysics, flightInputs);
			}
			
			if (defaultMechanic != null) {
				defaultMechanic.init (gameObject, flightPhysics, flightInputs);
				defaultMechanic.FFStart ();
			} else {
				Debug.LogError ("Default Flight Mechanic not setup!");
			}

			if (finishMechanic != null) {
				finishMechanic.init (gameObject, flightPhysics, flightInputs);
			}
			
			currentMechanic = null;

		}

		public override void init (GameObject go) {
			base.init (go);
			name = "Flight Mode";

			if (flightInputs == null)
				flightInputs = new FlightInputs ();
			if (flightMechanics == null)
				flightMechanics = new FlightMechanics ();
			if (flightPhysics == null)
				flightPhysics = new FlightPhysics ();

			flightMechanics.load<Mechanic> (defaultMechanicTypeName, ref defaultMechanic);
			flightMechanics.load<Mechanic> (mechanicTypeNames, ref mechanics);
			flightMechanics.load<Mechanic> (finishMechanicTypeName, ref finishMechanic);

			setupMechanics ();
		}


		public override void startMode () {
			//Make sure these properties are correctly set, otherwise
			//flight with rigidbodies is impossible.

			//Drag is based on a linear model, which can't be used alongside flight
			rigidbody.drag = 0.0f;
			//Don't use rigidbody based auto rotation, it fights with physics
			rigidbody.freezeRotation = true;
			rigidbody.isKinematic = false;

			defaultMechanic.FFStart ();
		}

		public override void finishMode () {
			flightInputs.resetInputs ();
			if (currentMechanic != null)
				currentMechanic.FFFinish ();
			defaultMechanic.FFFinish ();

			finishMechanic.FFStart ();
			finishMechanic.FFFinish ();		
		}

		public override void getInputs () {
			flightInputs.getInputs ();
		}

		protected override void applyPhysics ()
		{
			flightPhysics.applyPhysics(rigidbody);
		}

	}
}
