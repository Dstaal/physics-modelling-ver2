using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MyParticle : MonoBehaviour
{
    public bool pinned = false;
    public float mass = 1;
    public float lifespan = 0;
    public float age = 0;
    public Vector3 force = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    public MyParticleSystem targetParticleSystem { get; private set; }

    public Vector3 position
    {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }

    public MyParticle Initialize(MyParticleSystem parrnetParticleSystem, float startMass, Vector3 startPosition, Vector3 startVelocity, bool setPinned, float setLifeSpan)
    {
        this.targetParticleSystem = parrnetParticleSystem;
        this.targetParticleSystem.particles.Add(this);

        this.mass = startMass;
        this.position = startPosition;
        this.velocity = startVelocity;
        this.pinned = setPinned;
        this.lifespan = setLifeSpan;
        this.age = 0f;

        this.transform.localScale = new Vector3(startMass, startMass, startMass);

        this.transform.SetParent(this.targetParticleSystem.transform);
        this.name = "Particle " + this.targetParticleSystem.particles.IndexOf(this).ToString();

        return this;
    }

    public void clearForce()
    {
        this.force = Vector3.zero;
    }

    public void AddForce(Vector3 addedForce)
    {
        if (!this.pinned)
            this.force += addedForce;
    }

    public void Delete()
    {
        if (this.gameObject != null)
        {
            if (targetParticleSystem.springs.Count > 0)
            {
                for (int i = targetParticleSystem.springs.Count - 1; i >= 0; i--)
                {
                    MySpring spring = targetParticleSystem.springs[i];

                    if (this == spring.targetOne || this == spring.targetTwo)
                    {
                        spring.removeChildren();
                        spring.Delete();
                    }
                }
            }

            if (targetParticleSystem.attractions.Count > 0)
            {
                for (int i = targetParticleSystem.attractions.Count - 1; i >= 0; i--)
                {
                    MyAttraction attraction = targetParticleSystem.attractions[i];

                    if (this == attraction.targetOne || this == attraction.targetTwo)
                    {
                        attraction.removeChildren();
                        attraction.Delete();
                    }
                }
            }

            Destroy(this.gameObject, 0.01f);
            targetParticleSystem.particles.Remove(this);

        }
    }

    public void SetPinnedColor(bool bPinned)
    {
        this.pinned = bPinned;

        if (this.pinned)
        {
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        else
        {
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
    }
}