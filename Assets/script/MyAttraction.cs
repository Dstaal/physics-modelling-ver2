﻿using UnityEngine;
using System.Collections;

public class MyAttraction : MonoBehaviour {

	
	public MyParticle targetOne { get; protected set; }
	public MyParticle targetTwo { get; protected set; }

	public float strength = 1;
	public float minimumDistance = 0;



	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
