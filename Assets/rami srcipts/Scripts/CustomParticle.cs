using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CustomParticle : MonoBehaviour {
	
	public float Mass = 1f;
	public Vector3 Position {
		get { return this.transform.localPosition; }
		set { if (value.sqrMagnitude > 0f) this.transform.localPosition = value; }
	}
	
	public Vector3 Velocity = Vector3.zero;
	
	public bool Fixed = false;
	public float LifeSpan = 0f;
	public float Age = 0f;
	
	public Vector3 Force = Vector3.zero;

	public CustomParticleSystem CustomParticleSystem { get; private set; }

	public bool bProhibitMovement = true;


	public CustomParticle Initialize(CustomParticleSystem particleSystem) {
		return Initialize(particleSystem, Mass, Vector3.zero, Vector3.zero, Fixed, LifeSpan);
	}
	
	public CustomParticle Initialize(CustomParticleSystem particleSystem, float mass, Vector3 position, Vector3 velocity, bool bFixed, float lifeSpan) {
		if (particleSystem == null)
			throw new System.ArgumentNullException("particleSystem", "Cannot supply null as ParticleSystem to CustomParticle");

		this.CustomParticleSystem = particleSystem;
		this.CustomParticleSystem.Particles.Add(this);

		this.Mass = mass;
		this.Position = position;
		this.Velocity = velocity;
		this.SetFixed(bFixed);
		this.LifeSpan = lifeSpan;
		this.Age = 0f;

		this.ClearForce();

		this.transform.parent = this.CustomParticleSystem.transform;
		this.name = "Particle " + this.CustomParticleSystem.Particles.IndexOf(this).ToString();

		return this;
	}
	
	public void ClearForce() {
		this.Force = Vector3.zero;
	}
	
	public void AddForce(Vector3 force) {
		if (!this.Fixed) 
			this.Force += force;
	}
	
	public void SetFixed(bool bFixed) {
		this.Fixed = bFixed;
		
		if (this.Fixed) {
			this.GetComponent<Rigidbody>().isKinematic = true;
			this.transform.localScale = new Vector3(2f, 2f, 2f);
		}
		else {
			this.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	public void Delete(){		
		if (this.gameObject != null) 
			Destroy(this.gameObject, 0.01f);
	}
	
}
