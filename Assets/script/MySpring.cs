using UnityEngine;
using System.Collections;

public class MySpring : MonoBehaviour {
	
	public MyParticleSystem parnetParticleSystem { get; protected set; }

	public MyParticle targetOne { get; protected set; }
	public MyParticle targetTwo { get; protected set; }
	public float rest = 1;
	public float strength = 1;
	public float damping = 0;

//	public abstract void UpdateGizmos();

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
	}

	public void UpdateGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(targetOne.transform.parent.transform.position + targetOne.position, targetTwo.transform.parent.transform.position + targetTwo.position);
	}
	
	public void Delete() {
		Destroy(this.gameObject, 0.01f);
		parnetParticleSystem.springs.Remove(this);
	}

}
