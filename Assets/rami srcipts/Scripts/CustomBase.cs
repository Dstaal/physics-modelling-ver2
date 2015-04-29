using UnityEngine;
using System.Collections;

public abstract class CustomBase : Object {

	public CustomParticleSystem CustomParticleSystem { get; protected set; }

	public CustomParticle Particle1 { get; protected set; }
	public CustomParticle Particle2 { get; protected set; }

	public abstract void UpdateGizmos();
	public abstract void Delete();
}
