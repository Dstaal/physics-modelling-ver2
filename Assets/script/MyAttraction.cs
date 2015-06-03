using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MyAttraction 
{
    public List<GameObject> childrenOfThisAttraction;

    public MyParticleSystem parnetParticleSystem { get; protected set; }

    public MyParticle targetOne { get; protected set; }

    public MyParticle targetTwo { get; protected set; }

    public float strength = 1;
    public float minimumDistance = 0;

    public LineRenderer lineRender;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public MyAttraction(MyParticleSystem parnetParticleSystem, MyParticle targetOne, MyParticle targetTwo, float minDis, float str)
    {
        if (parnetParticleSystem == null)
            throw new System.ArgumentNullException("particleSystem", "Cannot supply null as ParticleSystem to CustomSpring");
        if (targetOne == null)
            throw new System.ArgumentNullException("particle1", "Cannot supply null as Particle1 to CustomSpring");
        if (targetTwo == null)
            throw new System.ArgumentNullException("particle2", "Cannot supply null as Particle2 to CustomSpring");

        //add the string to the particle system
        this.parnetParticleSystem = parnetParticleSystem;
        this.parnetParticleSystem.attractions.Add(this);

        childrenOfThisAttraction = new List<GameObject>();

        this.minimumDistance = minDis;
        this.strength = str;

        this.targetOne = targetOne;
        this.targetTwo = targetTwo;

        GameObject lineHolder = new GameObject();

        lineHolder.transform.SetParent(targetOne.transform);

        childrenOfThisAttraction.Add(lineHolder);

        lineRender = lineHolder.gameObject.AddComponent<LineRenderer>();
        lineRender.material = new Material(Shader.Find("Particles/Additive"));
        lineRender.SetWidth(0.3F, 0.3F);
        lineRender.SetColors(Color.blue, Color.blue);
    }

    public void drawLines()
    {
        lineRender.SetPosition(0, targetOne.transform.position);
        lineRender.SetPosition(1, targetTwo.transform.position);
    }

    public void Delete()
    {
        parnetParticleSystem.attractions.Remove(this);
    }

    public void removeChildren()
    {
        foreach (GameObject child in childrenOfThisAttraction)
        {
            GameObject.Destroy(child);
        }
    }
}