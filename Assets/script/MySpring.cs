using UnityEngine;
using System.Collections;

public class MySpring  {
	
	public MyParticleSystem parnetParticleSystem { get; protected set; }

	public MyParticle targetOne { get; protected set; }
	public MyParticle targetTwo { get; protected set; }
	public float rest = 1;
	public float strength = 1;
	public float damping = 0;

	public LineRenderer lineRender;

	public MySpring(MyParticleSystem parnetParticleSystem, MyParticle targetOne, MyParticle targetTwo, float restLength, float strength, float damping) 
	{
		if (parnetParticleSystem == null)
			throw new System.ArgumentNullException("particleSystem", "Cannot supply null as ParticleSystem to CustomSpring");
		if (targetOne == null)
			throw new System.ArgumentNullException("particle1", "Cannot supply null as Particle1 to CustomSpring");
		if (targetTwo == null) 
			throw new System.ArgumentNullException("particle2", "Cannot supply null as Particle2 to CustomSpring");
		
		//add the string to the particle system
		this.parnetParticleSystem = parnetParticleSystem;
		this.parnetParticleSystem.springs.Add(this);
		
		this.targetOne = targetOne;
		this.targetTwo = targetTwo;
		
		this.rest = restLength;
		this.strength = strength;
		this.damping = damping;

		lineRender = targetOne.gameObject.AddComponent<LineRenderer>();
		lineRender.material  = new Material(Shader.Find("Particles/Additive"));
		lineRender.SetWidth(0.2F, 0.2F);

	}

	public void drawLines()
	{
		lineRender.SetPosition(0, targetOne.transform.position);
		lineRender.SetPosition(1, targetTwo.transform.position);
	}
	

	public void Delete() {
		//Destroy(this.gameObject, 0.01f);
		//Debug.Log("spring.delete was called " );
		parnetParticleSystem.springs.Remove(this);
	}

}
