using UnityEngine;
using UnityEngine.UI;

public enum MoveOptions
{
    None,
    Moving,
    Lifteing,
   
}

public enum ParticleOptions
{
    None,
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
    public GameObject massFieldValue;
    public GameObject pinnedField;

    public Slider massSlider;

    public MoveOptions currentMovemnet = MoveOptions.None;
    public ParticleOptions currentState = ParticleOptions.None;

    public MyParticleSystem myParticleSystem;
    public MyParticle particle;
    public MyParticle targetOne;
    public MyParticle targetTwo;

    private Vector3 _startPosition = new Vector3(20, 1, 1);
    private Vector3 _startVelocity = new Vector3(0, 0, 0);
    private Vector3 _startGravity = new Vector3(0, -2.5f, 0);

    private float _startMass = 1f;
    private float _startDrag = 1.2f;

    private bool _yLocked = false;

    private void Start()
    {
        springObt = this.gameObject.GetComponent<SpringOptions>();
    }

    // Update is called once per frame
    private void Update()
    {
        //update seleceted particle name field
        if (particle != null)
        {
            nameFieldPrefab.GetComponent<Text>().text = particle.name.ToString();

        }

        if (this.currentMovemnet == MoveOptions.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hitParticle = GetParticleAtPos();
                if (hitParticle != null)
                {
                    this.currentMovemnet = MoveOptions.Moving;
                    particle = hitParticle;
           
                }
            }
            if (Input.GetMouseButtonDown(2))
            {
                var hitParticle = GetParticleAtPos();
                if (hitParticle != null)
                {
                    this.currentMovemnet = MoveOptions.Lifteing;
                    particle = hitParticle;
                    _yLocked = true;
                
                }
            }
        }

        // --- what we wonna do then left mouse bnt is hold down ---
        else if (this.currentMovemnet == MoveOptions.Moving)
        {
            var pos = GetMousePos("Ground");
            if (pos.HasValue)
            {
                Debug.Log(pos);
                particle.position = pos.Value;
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.currentMovemnet = MoveOptions.None;  
            }
        }

        // --- what we wonna do in middel mouse in hold down ---
        else if (this.currentMovemnet == MoveOptions.Lifteing)
        {
            var height = GetMousePos("Height");

            if (height.HasValue && _yLocked == true)
            {
                var pos = particle.transform.position;

                var heightY = (height.Value.y - pos.y + Screen.height/3) * Time.fixedDeltaTime ;

                Vector3 tempPos = new Vector3(pos.x, heightY, pos.z);

                particle.transform.position = tempPos;
      
            }

            if (Input.GetMouseButtonUp(2))
            {
                this.currentMovemnet = MoveOptions.None;
                _yLocked = false;
      
            }
        }

        //--- toggleing the right mouse bnt ---

        if (Input.GetMouseButtonUp(1) && this.currentState != ParticleOptions.DisplayConnections)
        {
            this.currentState = ParticleOptions.DisplayConnections;
            this.optionsCanvesPrefab.SetActive(true);
        

        }
        else if (Input.GetMouseButtonUp(1) && this.currentState == ParticleOptions.DisplayConnections)
        {
            this.currentState = ParticleOptions.None;
            this.optionsCanvesPrefab.SetActive(false);
         
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
        particle.SetPinnedColor(particle.pinned);
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

    public void massSliderOnChange()
    {
        massFieldValue.GetComponent<Text>().text = massSlider.value.ToString();
        _startMass = massSlider.value; 
    }

}