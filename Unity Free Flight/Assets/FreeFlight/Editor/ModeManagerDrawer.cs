﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityFreeFlight;

[CustomPropertyDrawer (typeof(ModeManager))]
public class ModeManagerDrawer : PropertyDrawer {

	bool flightModeFoldout;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		EditorGUILayout.PropertyField (property.FindPropertyRelative("_activeMode"));

		//TODO Ground mode still uses a generic property field, as it hasn't been refactored yet.
		EditorGUILayout.PropertyField (property.FindPropertyRelative ("groundMode"), true);


		flightModeFoldout = EditorPrefs.GetBool ("flightModeFoldout");
		if(flightModeFoldout = EditorGUILayout.Foldout (flightModeFoldout, "Flight Mode"))
			displayMode (position, property.FindPropertyRelative ("flightMode"), label);
		EditorPrefs.SetBool ("flightModeFoldout", flightModeFoldout);
	}


	public void displayMode(Rect position, SerializedProperty mode, GUIContent label) {
		EditorGUILayout.PropertyField (mode.FindPropertyRelative ("alwaysApplyPhysics"));

		List<string> mechanicNames = getMechanicNames ();
		SerializedProperty modeMechs = mode.FindPropertyRelative ("flightMechanics");


		EditorGUILayout.LabelField ("Default Mechanic:");
		EditorGUILayout.BeginHorizontal ();
		EditorGUI.indentLevel++;
		SerializedProperty flightModeDefaultMechanicName = mode.FindPropertyRelative ("defaultMechanicTypeName");
		int theNewValue = EditorGUILayout.Popup(mechanicNames.IndexOf (
			flightModeDefaultMechanicName.stringValue), mechanicNames.ToArray());
		if (theNewValue > -1)
			flightModeDefaultMechanicName.stringValue = mechanicNames[theNewValue];
		EditorGUI.indentLevel--;
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.LabelField ("Mechanics");
		EditorGUI.indentLevel++;
		SerializedProperty flightModeMechTypeNames = mode.FindPropertyRelative ("mechanicTypeNames"); 

		for (int i = 0; i < flightModeMechTypeNames.arraySize; i++) {
			EditorGUILayout.BeginHorizontal();
			SerializedProperty curMech = flightModeMechTypeNames.GetArrayElementAtIndex(i);

			int newValue = EditorGUILayout.Popup(mechanicNames.IndexOf (
				curMech.stringValue), mechanicNames.ToArray());
			if (newValue > -1)
				curMech.stringValue = mechanicNames[newValue];

			if (GUILayout.Button ('\u2193'.ToString()))
				flightModeMechTypeNames.MoveArrayElement(i, i+1);
			if (GUILayout.Button ("+")) 
				flightModeMechTypeNames.InsertArrayElementAtIndex(i);
			if (GUILayout.Button ("-")) 
				flightModeMechTypeNames.DeleteArrayElementAtIndex(i);

			if (curMech.stringValue != "")
				EditorGUILayout.PropertyField(modeMechs.FindPropertyRelative(curMech.stringValue.ToLower())
			                              .FindPropertyRelative("enabled"));

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField("Chain Rules");
			SerializedProperty chain = modeMechs.FindPropertyRelative(curMech.stringValue.ToLower ()).FindPropertyRelative ("chainRules");
			//Only lower precedence chains are allowed, so get rid of everything above this (minus one for the current mechanic)
			int chainSize = flightModeMechTypeNames.arraySize - i - 1;
			for (int j = 0; j < chainSize; j++) {
				int chainRule = flightModeMechTypeNames.arraySize - chainSize + j;
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField(flightModeMechTypeNames.GetArrayElementAtIndex(chainRule).stringValue);
				setChainForMechanicIndex(chainRule, chain, EditorGUILayout.Toggle(getChainMechanicIndex(chainRule, chain)));
				EditorGUILayout.EndHorizontal ();
			}

		}

		if (flightModeMechTypeNames.arraySize == 0) 
			if (GUILayout.Button ("+")) 
				flightModeMechTypeNames.InsertArrayElementAtIndex(0);

		EditorGUI.indentLevel--;

		EditorGUILayout.LabelField ("Finish Mechanic:");
		EditorGUILayout.BeginHorizontal ();
		EditorGUI.indentLevel++;
		SerializedProperty flightModeFinishMechanicName = mode.FindPropertyRelative ("finishMechanicTypeName");
		theNewValue = EditorGUILayout.Popup(mechanicNames.IndexOf (
			flightModeFinishMechanicName.stringValue), mechanicNames.ToArray());
		if (theNewValue > -1)
			flightModeFinishMechanicName.stringValue = mechanicNames[theNewValue];
		EditorGUI.indentLevel--;
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.LabelField ("Mechanics Configuration");
		EditorGUI.indentLevel+=1;
		foreach (string mechName in mechanicNames) {
			EditorGUILayout.PropertyField(modeMechs.FindPropertyRelative(mechName.ToLower()), true);
		}
		EditorGUI.indentLevel-=1;

		EditorGUILayout.PropertyField (mode.FindPropertyRelative ("flightPhysics"), true);

	}


	public bool getChainMechanicIndex(int chainInt, SerializedProperty serializedIntArray) {
		for (int i = 0; i < serializedIntArray.arraySize; i++) {
			if (serializedIntArray.GetArrayElementAtIndex(i).intValue == chainInt) {
				return true;
			}
		}
		return false;
	}

	public void setChainForMechanicIndex(int mechanicIndex, SerializedProperty serializedChainIntArray, bool state) {
		int i;
		for (i = 0; i < serializedChainIntArray.arraySize; i++) {
			if (serializedChainIntArray.GetArrayElementAtIndex(i).intValue == mechanicIndex) {
				if (state == false) {
					//Delete chain 
					serializedChainIntArray.DeleteArrayElementAtIndex(i);
					return;
				} else {
					//we need the value to be present and it is. Nothing to do. 
					return;
				}
			}

			if (serializedChainIntArray.GetArrayElementAtIndex(i).intValue > mechanicIndex) {
				break;
			}
			
		}

		if (state == true) {
			//insert chain
			serializedChainIntArray.InsertArrayElementAtIndex(i);
			serializedChainIntArray.GetArrayElementAtIndex(i).intValue = mechanicIndex;
			return;
		} else {
			//We need the value to not exist and it doesn't. Nothing to do.
			return;
		}
	}

	public List<string> getMechanicNames() {
		List<Type> mechanicTypes = UnityTypeResolver.GetAllSubTypes (typeof(Mechanic));
		List<string> names = new List<string> ();
		foreach (Type t in mechanicTypes) {
			names.Add (t.Name);
		}
		
		return names;
	}

}
