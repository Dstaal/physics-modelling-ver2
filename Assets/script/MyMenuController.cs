﻿using UnityEngine;
using System.Collections;

public enum ParticleOptions
{
	None,
	Moving,
	SpringSelection,
	AttractionSelection,
	DisplayConnections,
}

public class MyMenuController : MonoBehaviour {

	public GameObject mainCanvasPrefab; 
	public GameObject particleSystemPrefab;
	public GameObject particlePrefab;

	public ParticleOptions currentState = ParticleOptions.None;	

	private MyParticleSystem _particleSystem;
	private MyParticle _particle;
	private Vector3 _startPosition = new Vector3(0,0,1);
	private Vector3 _startVelocity = new Vector3(0,0,0);

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (this.currentState == ParticleOptions.None) {
			if (Input.GetMouseButtonDown (1)) {
				var hitParticle = GetParticleAtPos ();
				if (hitParticle != null) {
					this.currentState = ParticleOptions.Moving;
					_particle = hitParticle;
				}
			}
		} 

		else if (this.currentState == ParticleOptions.Moving) 
		{
			var pos = GetMousePos(LayerMask.NameToLayer("GroundLayer"));
			if(pos.HasValue)
			{
				_particle.transform.position = pos.Value;
			}
		
			if(Input.GetMouseButtonUp(1))
			{
				this.currentState = ParticleOptions.DisplayConnections;
				this.mainCanvasPrefab.SetActive(true);
			}


		}
	}


	public void ClickNewParticle()
	{
		if (_particleSystem == null)
		{
			var particleSystemGo = Instantiate(particleSystemPrefab) as GameObject; 
			_particleSystem = particleSystemGo.GetComponent<MyParticleSystem>().Initialize(new Vector3 (0,0,0),0);
			//Initialize(gravity , drag)
		}

		var newParticlePrefab = Instantiate(particlePrefab) as GameObject;
		var newParticle = newParticlePrefab.GetComponent<MyParticle>();
		
		newParticle.Initialize(_particleSystem, 1, _startPosition,_startVelocity, false, 0);

		//Initialize(MyParticleSystem parrnetParticleSystem, float startMass, Vector3 startPosition, Vector3 startVelocity, bool setPinned, float setLifeSpan) 
	}

	private RaycastHit? GetHitAtMousePos(LayerMask layer)
	{
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (!Physics.Raycast (ray, out hit, 1000f, layer)) 
		{
			return null;
		}
		return hit;
	}
	
	//vector3? means it nullable
	//exsample
	//vector3? nullablePos = getPos(layermask.nameForLayer("particle"));
	//if (nullablePos.HasValue)
	//{ Vector3 = nullablePos.value;}
	private Vector3? GetMousePos(LayerMask layer)
	{
		var hit = GetHitAtMousePos (layer);
		if (!hit.HasValue) 
		{
			return null;
		}

		return hit.Value.point;
	}

	private MyParticle GetParticleAtPos()
	{
		var hit = GetHitAtMousePos (LayerMask.NameToLayer ("ParticleLayer"));
		if(!hit.HasValue)
		{
			return null;
		}
		return hit.Value.transform.GetComponent<MyParticle> ();
	}
}
