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

public enum springSelectionStage
{
	None,
	selecting,
	finalizing,
}

public class MyMenuController : MonoBehaviour {

	public GameObject mainCanvasPrefab; 
	public GameObject optionsCanvesPrefab;
	public GameObject particleSystemPrefab;
	public GameObject particlePrefab;
	//ui elemetns
	public GameObject nameFieldPrefab;
	public GameObject pinnedField;
	public GameObject targetOneField;
	public GameObject targetTwoField;

	public ParticleOptions currentState = ParticleOptions.None;	
	public springSelectionStage currentSelectionStage = springSelectionStage.None;

	private MyParticleSystem _particleSystem;
	private MyParticle _particle;
	private MyParticle targetOne;
	private MyParticle targetTwo;

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
					_particle.tempPinned = true;

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
					_particle.tempPinned = true;
				}
			}
		} 

		// --- what we wonna do then left mouse bnt is hold down ---

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
				_particle.tempPinned = false;
			}
		}

		// --- what we wonna do in middel mouse in hold down ---

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
				_particle.tempPinned = false;
			}
		}

		//--- toggleing the right mouse bnt ---

		if(Input.GetMouseButtonUp(1) && this.currentState != ParticleOptions.DisplayConnections)
		{
			this.currentState = ParticleOptions.DisplayConnections;
			this.optionsCanvesPrefab.SetActive(true);
			_particle.tempPinned = true;
			if (_particle != null)
			{
			
			nameFieldPrefab.GetComponent<Text>().text = _particle.name.ToString();

			Debug.Log("partciles name : "+ _particle.name);
			}
			if (_particle == null)
			{
				Debug.Log("_partcile was null" );
			}
		
		}
		else if (Input.GetMouseButtonUp(1) && this.currentState == ParticleOptions.DisplayConnections)
		{
			this.currentState = ParticleOptions.None;
			this.optionsCanvesPrefab.SetActive(false);
			_particle.tempPinned = false;
		}

		//--- stages for spring selection -----


		if (this.currentSelectionStage == springSelectionStage.selecting)
		{
			if (targetOne != null)
			{
				// wait for the target two to be seleced.
				if(Input.GetMouseButtonDown (0))
				{
					targetTwo = GetParticleAtPos ();
					Debug.Log("trying to set targetTwo");
					targetTwoField.GetComponent<Text>().text = targetTwo.name.ToString();
					targetTwo.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
					this.currentSelectionStage = springSelectionStage.finalizing;
				}
			}
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

	public void clickPineedBnt()
	{
	//toggling pinned on/off and updateing the text feild

		if(_particle.pinned == false)
		{
			_particle.pinned= true;
		}
		else if (_particle.pinned == true)
		{
			_particle.pinned = false;
		}
		pinnedField.GetComponent<Text>().text = _particle.pinned.ToString();
		_particle.SetPinned(_particle.pinned);
		
		Debug.Log("clickPinnedBnt called");
	} 

	public void clickCreateSpring()
	{

		if (_particle != null)
		{
			//set the seleted particle as target one
			targetOne = _particle;
			targetOneField.GetComponent<Text>().text = targetOne.name.ToString();
			targetOne.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
			currentSelectionStage = springSelectionStage.selecting;

			// the rest of the selection, is handin in the update - 
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
