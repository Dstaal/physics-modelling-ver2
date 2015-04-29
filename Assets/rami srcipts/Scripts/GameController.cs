using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	public List<CustomParticleSystem> ParticleSystems = new List<CustomParticleSystem>();

	public bool RenderSpringsInGame = true;
	
	public GameObject ParticleSystemPrefab = null;
	public GameObject ParticlePrefab = null;

	private GameObject playerRef = null;
	private Camera beamCamera = null;


	private CustomParticleSystem addNewParticleSystem(Vector3 systemGravity, float systemDrag, Vector3 initialPosition, float samplingRate) {
		if (ParticleSystemPrefab != null) {
			CustomParticleSystem particleSystem = (Instantiate(ParticleSystemPrefab) as GameObject).GetComponent<CustomParticleSystem>();
			particleSystem.Initialize(systemGravity, systemDrag, samplingRate, initialPosition);

			ParticleSystems.Add(particleSystem);
			return particleSystem;
		}
		else {
			throw new System.NullReferenceException("ERROR: No particle system prefab set on GameController!");
		}
	}

	private CustomParticle addNewParticle(CustomParticleSystem particleSystem, float mass, Vector3 position, Vector3 velocity, bool bFixed, float lifeSpan) {
		if (ParticlePrefab != null) {
			CustomParticle particle = (Instantiate(ParticlePrefab) as GameObject).GetComponent<CustomParticle>();
			particle.Initialize(particleSystem, mass, position, velocity, bFixed, lifeSpan);
			return particle;
		}
		else {
			throw new System.NullReferenceException("ERROR: No particle prefab set on GameController!");
		}
	}

	private CustomSpring addNewSpring(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float restLength, float strength, float damping) {
		return new CustomSpring(particleSystem, particle1, particle2, restLength, strength, damping);
	}
	
	private CustomAttraction addNewAttraction(CustomParticleSystem particleSystem, CustomParticle particle1, CustomParticle particle2, float strength, float minimumDistance) {
		return new CustomAttraction(particleSystem, particle1, particle2, strength, minimumDistance);
	}



	void Start() {
		playerRef = GameObject.FindGameObjectWithTag("Player");
		if (playerRef == null)
			throw new System.NullReferenceException("Player GameObject could not be found by GameController");
	}

	
	void Update () {
		if (Input.GetKeyDown(KeyCode.B)) {
			addNewBeamSystem();
		}

		if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) {
			if (this.ParticleSystems.Count > 0) {
				Destroy(ParticleSystems[0].gameObject, 1f);
				this.ParticleSystems.RemoveAt(0);
				Debug.Log("DELETED 0th particle system in list");
			}
		}


		if (ParticleSystems.Count > 0) {
			foreach (CustomParticleSystem beamSystem in ParticleSystems) {
				if (beamSystem.Particles.Count > 0) {
					for (int i = 0; i < beamSystem.Particles.Count; i++) {
						CustomParticle particle = beamSystem.Particles[i];

						if (!particle.Fixed) {
							particle.Velocity += Random.insideUnitSphere;
						}
						else {
							if (!particle.bProhibitMovement)
								particle.transform.position += particle.transform.forward.normalized/4f;
						}
					}
				}
			}

			CustomParticleSystem lastBeam = ParticleSystems[ParticleSystems.Count-1];
			if (lastBeam != null && lastBeam.Particles.Count > 0) {
				CustomParticle leaderParticle = lastBeam.Particles[0];
				if (leaderParticle != null) {
					Vector3 leaderPos = leaderParticle.Position;
					if (lastBeam.GetComponentInChildren<Camera>() == null) {
						beamCamera = (Instantiate(Resources.Load("BeamCamera")) as GameObject).GetComponent<Camera>();
						beamCamera.transform.parent = lastBeam.transform;

						beamCamera.transform.localPosition = leaderPos + new Vector3(5f, 20f, 0f);
						beamCamera.transform.LookAt(leaderParticle.transform.position);
						beamCamera.transform.localPosition += new Vector3(0f, 0f, 20f);
					}
					else {
						beamCamera.transform.localPosition = leaderPos + new Vector3(5f, 20f, 0f);
						beamCamera.transform.LookAt(leaderParticle.transform.position);
						beamCamera.transform.localPosition += new Vector3(0f, 0f, 20f);
					}
				}
			}
		}
	}
	
	private void addNewBeamSystem() {
		Vector3 gravity = playerRef.transform.forward.normalized;
		float drag = 0.1f;

		int particleCount = 30;
		float particleMass = 10f,
			  particleLifeSpan = 60f;

		float samplingRate = 10f; // 10 ms

		float springRestLength = 3f,
			  springStrength = 3f,
			  springDamping = 0.75f;


		CustomParticleSystem beamSystem = addNewParticleSystem(gravity, drag, playerRef.transform.position, samplingRate);
		beamSystem.name = "BEAM SYSTEM";

		CustomParticle leaderParticle = addNewParticle(beamSystem, particleMass, beamSystem.Position + new Vector3(0f, 1f, 0f), Vector3.zero, true, particleLifeSpan);
		leaderParticle.name = "Leader Particle";
		leaderParticle.transform.rotation = playerRef.transform.rotation;

		List<CustomParticle> 
			stream1 = new List<CustomParticle>(),
			stream2 = new List<CustomParticle>(),
			stream3 = new List<CustomParticle>(),
			stream4 = new List<CustomParticle>();


	
		for (int i = 0; i < Mathf.RoundToInt(particleCount/4f); i++) {
			Vector3 pos = beamSystem.Position - playerRef.transform.forward.normalized * (float)(i+1);

			CustomParticle particle = addNewParticle(beamSystem, particleMass, pos, Vector3.zero, false, particleLifeSpan);
			particle.name = "Stream_1-" + i.ToString();
			stream1.Add(particle);


			CustomParticle particle2 = addNewParticle(beamSystem, particleMass, pos, Vector3.zero, false, particleLifeSpan);
			particle2.name = "Stream_2-" + i.ToString();
			stream2.Add(particle2);


			CustomParticle particle3 = addNewParticle(beamSystem, particleMass, pos, Vector3.zero, false, particleLifeSpan);
			particle3.name = "Stream_3-" + i.ToString();
			stream3.Add(particle3);


			CustomParticle particle4 = addNewParticle(beamSystem, particleMass, pos, Vector3.zero, false, particleLifeSpan);
			particle4.name = "Stream_4-" + i.ToString();
			stream4.Add(particle4);

			if (i > 0) {
				addNewSpring(beamSystem, stream1[i], stream1[i-1], springRestLength, springStrength, springDamping);
				addNewSpring(beamSystem, stream2[i], stream2[i-1], springRestLength, springStrength, springDamping);
				addNewSpring(beamSystem, stream3[i], stream3[i-1], springRestLength, springStrength, springDamping);
				addNewSpring(beamSystem, stream4[i], stream4[i-1], springRestLength, springStrength, springDamping);
			}
			else {
				addNewSpring(beamSystem, leaderParticle, particle,  springRestLength, springStrength*1.5f, springDamping);
				addNewSpring(beamSystem, leaderParticle, particle2, springRestLength, springStrength*1.5f, springDamping);
				addNewSpring(beamSystem, leaderParticle, particle3, springRestLength, springStrength*1.5f, springDamping);
				addNewSpring(beamSystem, leaderParticle, particle4, springRestLength, springStrength*1.5f, springDamping);
			}
		}
	}
	

}
