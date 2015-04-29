using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyParticleSystem : MonoBehaviour {


	public Vector3 gravity = Vector3.zero;
	public float drag = 0;
	public List<MyParticle> particles;
	public List<MySpring> springs;
	public List<MyAttraction> attractions;


	public MyParticleSystem Initialize(Vector3 startGravity, float startDrag) 
	{
		this.particles = new List<MyParticle>();
		this.springs = new List<MySpring>();
		this.attractions = new List<MyAttraction>();

		this.gravity = startGravity;
		this.drag = startDrag;

		//didn't add sampleing raet and system time. do i need those?
				
		return this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
