using UnityEngine;
using UnityEngine.UI;

public enum ParticleOptions
{
    None,
    Moving,
    Lifteing,
    SpringSelection,
    AttractionSelection,
    DisplayConnections,
}

public class MyMenuController : MonoBehaviour
{
    public SpringOptions springObt;

    public GameObject mainCanvasPrefab;
    public GameObject optionsCanvesPrefab;
    public GameObject particleSystemPrefab;
    public GameObject particlePrefab;

    //ui elemetns
    public GameObject nameFieldPrefab;

    public GameObject pinnedField;

    public ParticleOptions currentState = ParticleOptions.None;

    public MyParticleSystem myParticleSystem;
    public MyParticle particle;
    public MyParticle targetOne;
    public MyParticle targetTwo;

    private Vector3 _startPosition = new Vector3(0, 1, 1);
    private Vector3 _startVelocity = new Vector3(0, 0, 0);
    private Vector3 _startGravity = new Vector3(0, -0.5f, 0);

    private float _startMass = 1f;
    private float _startDrag = 1f;

    private bool _yLocked = false;
    private float _middleMouseClickAdjust = 1f;

    private void Start()
    {
        springObt = this.gameObject.GetComponent<SpringOptions>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.currentState == ParticleOptions.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hitParticle = GetParticleAtPos();
                if (hitParticle != null)
                {
                    this.currentState = ParticleOptions.Moving;
                    particle = hitParticle;
                    particle.tempPinned = true;
                }
            }
            if (Input.GetMouseButtonDown(2))
            {
                var hitParticle = GetParticleAtPos();
                if (hitParticle != null)
                {
                    this.currentState = ParticleOptions.Lifteing;
                    particle = hitParticle;
                    _yLocked = true;
                    particle.tempPinned = true;
                }
            }
        }

        // --- what we wonna do then left mouse bnt is hold down ---
        else if (this.currentState == ParticleOptions.Moving)
        {
            var pos = GetMousePos("Ground");
            if (pos.HasValue)
            {
                particle.position = pos.Value;
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.currentState = ParticleOptions.None;
                particle.tempPinned = false;
            }
        }

        // --- what we wonna do in middel mouse in hold down ---
        else if (this.currentState == ParticleOptions.Lifteing)
        {
            var height = GetMousePos("Height");

            if (height.HasValue && _yLocked == true)
            {
                var pos = particle.transform.position;
                var heightY = (height.Value.y - pos.y) * Time.fixedDeltaTime + _middleMouseClickAdjust;

                Vector3 tempPos = new Vector3(pos.x, heightY, pos.z);

                particle.transform.position = tempPos;
                particle.tempPinned = true;
            }

            if (Input.GetMouseButtonUp(2))
            {
                this.currentState = ParticleOptions.None;
                _yLocked = false;
                particle.tempPinned = false;
            }
        }

        //--- toggleing the right mouse bnt ---

        if (Input.GetMouseButtonUp(1) && this.currentState != ParticleOptions.DisplayConnections)
        {
            this.currentState = ParticleOptions.DisplayConnections;
            this.optionsCanvesPrefab.SetActive(true);
            particle.tempPinned = true;
            if (particle != null)
            {
                nameFieldPrefab.GetComponent<Text>().text = particle.name.ToString();

                Debug.Log("partciles name : " + particle.name);
            }
            if (particle == null)
            {
                Debug.Log("_partcile was null");
            }
        }
        else if (Input.GetMouseButtonUp(1) && this.currentState == ParticleOptions.DisplayConnections)
        {
            this.currentState = ParticleOptions.None;
            this.optionsCanvesPrefab.SetActive(false);
            particle.tempPinned = false;
        }
    }

    public void ClickNewParticle()
    {
        if (myParticleSystem == null)
        {
            var particleSystemGo = Instantiate(particleSystemPrefab) as GameObject;
            myParticleSystem = particleSystemGo.GetComponent<MyParticleSystem>().Initialize(_startGravity, _startDrag);
        }

        var newParticlePrefab = Instantiate(particlePrefab) as GameObject;
        var newParticle = newParticlePrefab.GetComponent<MyParticle>();

        newParticle.Initialize(myParticleSystem, _startMass, _startPosition, _startVelocity, false, 0);

        // note  Initialize(MyParticleSystem parrnetParticleSystem, float startMass, Vector3 startPosition, Vector3 startVelocity, bool setPinned, float setLifeSpan)
    }

    public void ClickDeleteParticle()
    {
        if (particle != null)
        {
            particle.Delete();
        }
    }

    public void clickPineedBnt()
    {
        //toggling pinned on/off and updateing the text feild

        if (particle.pinned == false)
        {
            particle.pinned = true;
        }
        else if (particle.pinned == true)
        {
            particle.pinned = false;
        }
        pinnedField.GetComponent<Text>().text = particle.pinned.ToString();
        particle.SetPinned(particle.pinned);
    }

    public RaycastHit? GetHitAtMousePos(string tag)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        foreach (var hit in Physics.RaycastAll(ray, 1000f))
        {
            if (hit.transform.CompareTag(tag))
            {
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

    public Vector3? GetMousePos(string tag)
    {
        var hit = GetHitAtMousePos(tag);
        if (!hit.HasValue)
        {
            Debug.Log("GetMousePos retruned at null");
            return null;
        }

        return hit.Value.point;
    }

    public MyParticle GetParticleAtPos()
    {
        var hit = GetHitAtMousePos("Particle");
        if (!hit.HasValue)
        {
            return null;
        }

        return hit.Value.transform.GetComponent<MyParticle>();
    }
}