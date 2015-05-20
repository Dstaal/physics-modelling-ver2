using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyParticleSystem : MonoBehaviour 
{


	public Vector3 gravity = Vector3.zero;
	public float drag = 0;
	public float SamplingRate = 10f; // 10 ms 

	public List<MyParticle> particles;
	public List<MySpring> springs;
	public List<MyAttraction> attractions;

	public float SystemTime = 0f;

	public List<PhaseSpace> currentPhaseSpace = new List<PhaseSpace>();

	private float lastSample = 0f;


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

	void FixedUpdate () {
		updateParticleSystemTime();
	}


	void updateParticleSystemTime() 
	{
		if (Time.time - lastSample > SamplingRate/1000f) 
		{
			float deltaTime = Time.time - lastSample;
			advanceTime(deltaTime);
			
			lastSample = Time.time;
		}
		
		SystemTime += Time.fixedDeltaTime;
		//advanceParticlesAges(Time.fixedDeltaTime);
	}

	private void advanceTime(float deltaTime) 
	{
		if (this.currentPhaseSpace != null) 
		{
			//killOldParticles();
			
			List<PhaseSpace> newState = computeStateDerivate();
			this.currentPhaseSpace = newState;
			
			setPhaseSpace(newState);
		}
	}


	private void updateAllForces()
	{

		updateForces();
		UpdateLines();
		updateSprings();
		addGravity();

	}

	private void UpdateLines()
	{
		if (springs.Count > 0)
		{
			foreach (MySpring spring in springs) 
			{
				spring.drawLines();
			}
		}
		
	}

	private void addGravity() 
	{
		if (particles.Count > 0) 
		{
			foreach (MyParticle particle in particles) 
			{
				if (particle.transform.position.y > 0 )
				{
				Vector3 gravityForce = this.gravity * particle.mass;
				particle.AddForce(gravityForce);
				}
				else if (particle.transform.position.y < 0)
				{
					particle.ClearForce();
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
				particle.position = particle.position + particle.force;
			}
		}

	}

	private void updateSprings()
	{
		if (springs.Count > 0) 
		{
			foreach (MySpring spring in springs) 
			{

				Vector3 springCenter = spring.targetTwo.position - spring.targetOne.position;
				float springCenterNrm = springCenter.magnitude;

				Debug.Log(" springCenter : " + springCenter );
				
				if (springCenterNrm < 1f) 
				{
					springCenterNrm = 1f;
				}
				
				Vector3 springUnit = springCenter / springCenterNrm;
				
				Vector3 springForce = spring.strength * springUnit * (springCenterNrm - spring.rest);

				Debug.Log("springforce : " + springForce);

				spring.targetOne.AddForce(springForce);
				spring.targetTwo.AddForce(-springForce);

				Vector3 velocityDelta = spring.targetTwo.velocity - spring.targetOne.velocity;


				// add ref to the pdf here:
				Vector3 projectionVelocityDeltaOnPositionDelta = Vector3.Dot(springUnit, velocityDelta) * springUnit;
				Vector3 dampingForce = spring.damping * projectionVelocityDeltaOnPositionDelta;
				
				spring.targetOne.AddForce( dampingForce);
				spring.targetTwo.AddForce(-dampingForce);

			}
		}
	}


	/* Phase Space State */
	private void setPhaseSpace(List<PhaseSpace> phaseSpace) 
	{

		if (particles.Count > 0) 
		{
			for (int i = 0; i < particles.Count; i++) 
			{
				MyParticle particle = this.particles[i];
				
				particle.position += new Vector3(phaseSpace[i].x, phaseSpace[i].y, phaseSpace[i].z);
				particle.velocity = new Vector3(phaseSpace[i].x_v, phaseSpace[i].y_v, phaseSpace[i].z_v);
			}
		}
	}
	
	private List<PhaseSpace> getPhaseSpaceState() 
	{

		List<PhaseSpace> _phaseSpace = new List<PhaseSpace>();
		List<Vector3> _positions = new List<Vector3>();
		List<Vector3> _velocities = new List<Vector3>();

		if (particles.Count > 0) {
			
			foreach (MyParticle _particle in particles) 
			{
				_positions.Add(_particle.position);
			}
			
			foreach (MyParticle _particle in particles) 
			{
				if (_particle.pinned)
					_velocities.Add(Vector3.zero);
				else
					_velocities.Add(_particle.velocity);
			}
		}

		if ((_positions == null || _velocities == null) || (_positions.Count != this.particles.Count || _velocities.Count != this.particles.Count)) {
			Debug.LogWarning("ERROR: positions, velocities and Particles lists are not same length or null!!");
		}
		else {
			for (int i = 0; i < this.particles.Count; i++) {
				_phaseSpace.Add(new PhaseSpace());
				
				_phaseSpace[i].x = _positions[i].x;
				_phaseSpace[i].y = _positions[i].y;
				_phaseSpace[i].z = _positions[i].z;
				
				_phaseSpace[i].x_v = _velocities[i].x;
				_phaseSpace[i].y_v = _velocities[i].y;
				_phaseSpace[i].z_v = _velocities[i].z;
			}
		}
		
		return _phaseSpace;
	}


	// see the pdf page 56. quuestion about ode4. claims this is called 4 times by that.
	private List<PhaseSpace> computeStateDerivate() 
	{
		List<PhaseSpace> stateDerivate = null;
		
		if (this.currentPhaseSpace != null) 
		{
			updateAllForces(); 
			
			stateDerivate = new List<PhaseSpace>();

			List<Vector3> _velocities = new List<Vector3>();
			List<Vector3> _accelerations = new List<Vector3>();

			if (particles.Count > 0) 
			{
				
				foreach (MyParticle _particle in particles) 
				{
					if (_particle.pinned)
						_velocities.Add(Vector3.zero);
					else
						_velocities.Add(_particle.velocity);
				}
				
				foreach (MyParticle _particle in particles) 
				{
					Vector3 force = Vector3.zero;
					if (!_particle.pinned)
						force = _particle.force;
					
					_accelerations.Add(force/_particle.mass);
				}
			}
			
			if ((_velocities == null || _accelerations == null) || (_velocities.Count != this.particles.Count || _accelerations.Count != this.particles.Count))
			{
				Debug.LogWarning("ERROR: velocities, accelerations and Particles lists are not same length or null!!");
			}
			else 
			{
				for (int i = 0; i < this.particles.Count; i++) 
				{
					if (this.particles[i] != null) 
					{
						stateDerivate.Add(new PhaseSpace());
						
						stateDerivate[i].x = _velocities[i].x;
						stateDerivate[i].y = _velocities[i].y;
						stateDerivate[i].z = _velocities[i].z;
						
						stateDerivate[i].x_v = _accelerations[i].x;
						stateDerivate[i].y_v = _accelerations[i].y;
						stateDerivate[i].z_v = _accelerations[i].z;
					}
				}
			}
		}
		return stateDerivate;
	}

	//end of class
}
