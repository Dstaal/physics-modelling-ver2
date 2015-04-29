using UnityEngine;
using System.Collections;

public class CustomAttraction : CustomBase {
	
	public float Strength = 0f;
	public float MinimumDistance = 0f;
	

	public CustomAttraction(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float strength, float minimumDistance) {
		if (particleSystem == null)
			throw new System.ArgumentNullException("particleSystem", "Cannot supply null as ParticleSystem to CustomAttraction");
		if (particle1 == null)
			throw new System.ArgumentNullException("particle1", "Cannot supply null as Particle1 to CustomAttraction");
		if (particle2 == null)
			throw new System.ArgumentNullException("particle2", "Cannot supply null as Particle2 to CustomAttraction");

		this.CustomParticleSystem = particleSystem;
		this.CustomParticleSystem.Attractions.Add(this);
		
		this.Particle1 = particle1;
		this.Particle2 = particle2;
		
		this.Strength = strength;
		this.MinimumDistance = minimumDistance;
	}

	public override void UpdateGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Particle1.transform.parent.transform.position + Particle1.Position, Particle2.transform.parent.transform.position + Particle2.Position);
	}

	public override void Delete() {
		Destroy(this, 0.01f);
	}
}
