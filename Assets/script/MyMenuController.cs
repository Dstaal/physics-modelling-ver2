using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum ParticleOptions
{
	None,
	Moving,
	Lifteing,
	SpringSelection,
	AttractionSelection,
	DisplayConnections,
}

public class MyMenuController : MonoBehaviour {

	public GameObject mainCanvasPrefab; 
	public GameObject optionsCanvesPrefab;
	public GameObject particleSystemPrefab;
	public GameObject particlePrefab;
	public GameObject nameFieldPrefab;

	public ParticleOptions currentState = ParticleOptions.None;	

	private MyParticleSystem _particleSystem;
	private MyParticle _particle;
	private Vector3 _startPosition = new Vector3(0,1,1);
	private Vector3 _startVelocity = new Vector3(0,0,0);
	private Vector3 _startGravity = new Vector3(0,-0.005f,0);
	private float _startMass = 1;
	private float _startDrag = 1;
	private bool yLocked = false;
	private float middleMouseClickAdjust = 1 ;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

		if (this.currentState == ParticleOptions.None) 
		{

			if (Input.GetMouseButtonDown (0))
			{
				var hitParticle = GetParticleAtPos ();
				if (hitParticle != null) 
				{
					this.currentState = ParticleOptions.Moving;
					_particle = hitParticle;
					_particle.pinned = true;

				}
			}
			if (Input.GetMouseButtonDown(2))
			{
				var hitParticle = GetParticleAtPos ();
				if (hitParticle != null) 
				{
					this.currentState = ParticleOptions.Lifteing;
					_particle = hitParticle;
					yLocked = true;
					_particle.pinned = true;
				}
			}
		} 
		// what we wonna do theleft mouse bnt id hold down
		else if (this.currentState == ParticleOptions.Moving) 
		{
			var pos = GetMousePos("Ground");
			if(pos.HasValue)
			{
				_particle.transform.position = pos.Value;
			}
		
			if(Input.GetMouseButtonUp(0))
			{
				//Debug.Log("setteing State back To none");
				this.currentState = ParticleOptions.None;
				_particle.pinned = false;
			}
		}

		//what we wonna do in middel mouse in hold down
		else if (this.currentState == ParticleOptions.Lifteing) 
		{
		
			var height = GetMousePos("Height");

			if(height.HasValue && yLocked == true)
			{
				var pos = _particle.transform.position;
				var heightY = ( height.Value.y - pos.y )  * Time.fixedDeltaTime + middleMouseClickAdjust  ; 

				Vector3 tempPos = new Vector3(pos.x, heightY, pos.z);

				//Debug.Log("tempPos =" + tempPos);

				_particle.transform.position = tempPos;

			}

			if(Input.GetMouseButtonUp(2))
			{
				//Debug.Log("setteing State back To none");
				this.currentState = ParticleOptions.None;
				yLocked = false;
				_particle.pinned = false;
			}
		}

		//toggleing the right mouse bnt
		if(Input.GetMouseButtonUp(1) && this.currentState != ParticleOptions.DisplayConnections)
		{
			this.currentState = ParticleOptions.DisplayConnections;
			this.optionsCanvesPrefab.SetActive(true);
			if (_particle != null)
			{
			
			nameFieldPrefab.GetComponent<Text>().text = _particle.name.ToString();

			Debug.Log("partciles name : "+ _particle.name);
			}
			if (_particle == null)
			{
				Debug.Log(" the dammed _partcile was null" );
			}
			//Add stuff here later
		}
		else if (Input.GetMouseButtonUp(1) && this.currentState == ParticleOptions.DisplayConnections)
		{
			this.currentState = ParticleOptions.None;
			this.optionsCanvesPrefab.SetActive(false);
		}
	}


	public void ClickNewParticle()
	{
		if (_particleSystem == null)
		{
			var particleSystemGo = Instantiate(particleSystemPrefab) as GameObject; 
			_particleSystem = particleSystemGo.GetComponent<MyParticleSystem>().Initialize(_startGravity, _startDrag);
		}

		var newParticlePrefab = Instantiate(particlePrefab) as GameObject;
		var newParticle = newParticlePrefab.GetComponent<MyParticle>();
		
		newParticle.Initialize(_particleSystem, _startMass, _startPosition,_startVelocity, false, 0);

		// note  Initialize(MyParticleSystem parrnetParticleSystem, float startMass, Vector3 startPosition, Vector3 startVelocity, bool setPinned, float setLifeSpan) 
	}

	public void ClickDeleteParticle()
	{
		if (_particle != null)
		{
			_particle.Delete();
		}
	}

	private RaycastHit? GetHitAtMousePos(string tag)
	{

		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		foreach (var hit in Physics.RaycastAll(ray,1000f))
		{
			if (hit.transform.CompareTag(tag))
			{
				//Debug.Log("reteurned hit");
					return hit;
			}
		}
		return null;
	}
	
	//vector3? means it nullable
	//exsample
	//vector3? nullablePos = getPos(layermask.nameForLayer("particle"));
	//if (nullablePos.HasValue)
	//{ Vector3 = nullablePos.value;}


	private Vector3? GetMousePos(string tag)
	{

		var hit = GetHitAtMousePos (tag);
		if (!hit.HasValue) 
		{
			Debug.Log("GetMousePos retruned at null");
			return null;
		}
		//Debug.Log("mousepos" + hit.Value.point);
		return hit.Value.point;
	}

	private MyParticle GetParticleAtPos()
	{

		var hit = GetHitAtMousePos ("Particle");
		if(!hit.HasValue)
		{
			//Debug.Log("GetPartcileAtpos retruned null ");
			return null;
		}
		//Debug.Log("GetPartcileAtpos retruned after detecting a partcile" );
		return hit.Value.transform.GetComponent<MyParticle> ();
	}
}
