using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyParticleSystem : MonoBehaviour {


	public Vector3 gravity = Vector3.zero;
	public float drag = 0;
	public List<MyParticle> particles;
	public List<MySpring> springs;
	public List<MyAttraction> attractions;

//	private Vector3 _bounce = new Vector3 (0,0.1f,0);


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

		addGravity();
		updateForces();

	}

	private void addGravity() 
	{
		if (particles.Count > 0) 
		{
			foreach (MyParticle particle in particles) 
			{
				if (particle.transform.position.y > 0)
				{
				Vector3 gravityForce = this.gravity * particle.mass;
				particle.AddForce(gravityForce);
				}
			}
		}
	}


	private void updateForces()
	{
		if (particles.Count > 0)
		{
			foreach (MyParticle particle in particles)
			{
				particle.transform.position = particle.transform.position + particle.force;
			}
		}

	}





}
