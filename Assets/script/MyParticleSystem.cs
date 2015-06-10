using System.Collections.Generic;
using UnityEngine;

public class MyParticleSystem : MonoBehaviour
{
    public float drag = 0;
    public float SamplingRate = 10f;
    public float systemTime = 0f;
    public Vector3 gravity = Vector3.zero;

    public List<MyParticle> particles;
    public List<MySpring> springs;
    public List<MyAttraction> attractions;

    private float lastSample = 0f;

    public MyParticleSystem Initialize(Vector3 startGravity, float startDrag)
    {
        this.particles = new List<MyParticle>();
        this.springs = new List<MySpring>();
        this.attractions = new List<MyAttraction>();

        this.gravity = startGravity;
        this.drag = startDrag;

        return this;
    }

    private void FixedUpdate()
    {
        updateParticleSystemTime();
    }

    private void updateParticleSystemTime()
    {
        if (Time.time - lastSample > SamplingRate / 1000f)
        {
            float deltaTime = Time.time - lastSample;
            advanceTime(deltaTime);

            lastSample = Time.time;
        }

        systemTime += Time.fixedDeltaTime;
    }

    private void advanceTime(float deltaTime)
    {
        var timeStart = systemTime;
        var timeEnd = timeStart + deltaTime;

        var newState = computeStateDerivate(deltaTime);

        this.setPhaseSpace(newState);

        this.systemTime = timeEnd;
    }

    private void updateAllForces()
    {
        clearSysForces();

        UpdateLines();
        updateSprings();
        updateAttractions();
        updateDrag();
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

        if (attractions.Count > 0)
        {
            foreach (MyAttraction attraction in attractions)
            {
                attraction.drawLines();
            }
        }
    }

    private void addGravity()
    {
        if (particles.Count > 0)
        {
            foreach (MyParticle particle in particles)
            {
                if (particle.transform.position.y > 0)
                {
                    Vector3 gravityforce = this.gravity * particle.mass;
                    particle.AddForce(gravityforce);
                }
                else if (particle.transform.position.y < 0)
                {
                    var pos = particle.position;

                    var gourndlevel = 0f;

                    Vector3 dontdropbelow = new Vector3(pos.x, gourndlevel, pos.z);

                    particle.position = dontdropbelow;
                }
            }
        }
    }

    private void updateSprings()
    {
        if (springs.Count > 0)
        {
            foreach (MySpring spring in springs)
            {
                Vector3 position_delta = spring.targetTwo.position - spring.targetOne.position;

                float position_delta_Nrm = position_delta.magnitude;

                if (position_delta_Nrm < 1f)
                {
                    position_delta_Nrm = 1f;
                }

                Vector3 position_delta_unit = position_delta / position_delta_Nrm;

                Vector3 springForce = spring.strength * position_delta_unit * (position_delta_Nrm - spring.rest);

                spring.targetOne.AddForce(springForce);
                spring.targetTwo.AddForce(-springForce);

                Vector3 velocityDelta = spring.targetTwo.velocity - spring.targetOne.velocity;

                Vector3 projectionVelocityDeltaOnPositionDelta = Vector3.Dot(position_delta_unit, velocityDelta) * position_delta_unit;
                Vector3 dampingForce = spring.damping * projectionVelocityDeltaOnPositionDelta;

                spring.targetOne.AddForce(dampingForce);
                spring.targetTwo.AddForce(-dampingForce);
            }
        }
    }

    private void updateDrag()
    {
        if (particles.Count > 0)
        {
            foreach (MyParticle particle in particles)
            {
                Vector3 dragForce = -this.drag * particle.velocity;

                particle.AddForce(dragForce);
            }
        }
    }

    private void updateAttractions()
    {
        foreach (MyAttraction attreaction in attractions)
        {
            Vector3 position_delta = attreaction.targetTwo.position - attreaction.targetOne.position;

            float position_delta_Nrm = position_delta.magnitude;

            if (position_delta_Nrm < attreaction.minimumDistance)
            {
                position_delta_Nrm = attreaction.minimumDistance;
            }

            Vector3 attreactionForce = attreaction.strength * attreaction.targetOne.mass * attreaction.targetTwo.mass * position_delta / position_delta_Nrm / position_delta_Nrm / position_delta_Nrm; // why the hell  /position_delta_Nrm 3times?

            if (attreactionForce.x <= 0.01 && attreactionForce.y <= 0.01 && attreactionForce.z <= 0.01 || attreactionForce.x >= -0.01 && attreactionForce.y >= -0.01 && attreactionForce.z >= -0.01)
            {
                attreaction.lineRender.SetColors(Color.red, Color.red);
            }
            if (attreactionForce.x >= 0.01 || attreactionForce.y >= 0.01 || attreactionForce.z >= 0.01 || attreactionForce.x <= -0.01 || attreactionForce.y <= -0.01 || attreactionForce.z <= -0.01)
            {
                attreaction.lineRender.SetColors(Color.blue, Color.blue);
            }

            attreaction.targetOne.AddForce(attreactionForce);
            attreaction.targetTwo.AddForce(-attreactionForce);
        }
    }

    public void clearSysForces()
    {
        foreach (MyParticle particle in particles)
        {
            particle.clearForce();
        }
    }

    /* Phase Space State */

    public List<Vector3> getParticlesPositions()
    {
        if (particles.Count == 0)
        {
            return null;
        }

        List<Vector3> positions = new List<Vector3>(particles.Count);
        for (int i = 0; i < particles.Count; i++)
        {
            positions.Add(particles[i].position);
        }

        return positions;
    }

    public List<Vector3> getParticlesVelocities()
    {
        if (particles.Count == 0)
        {
            return null;
        }

        List<Vector3> velocities = new List<Vector3>();
        for (int i = 0; i < particles.Count; i++)
        {
            var particle = particles[i];
            if (particle.pinned)
                velocities.Add(Vector3.zero);
            else
                velocities.Add(particle.velocity);
        }

        return velocities;
    }

    public List<Vector3> getParticlesAccelerations()
    {
        if (particles.Count == 0)
        {
            return null;
        }

        List<Vector3> accelerations = new List<Vector3>();
        foreach (MyParticle particle in particles)
        {
            Vector3 force = Vector3.zero;
            if (!particle.pinned)
                force = particle.force;

            accelerations.Add(force / particle.mass);
        }

        return accelerations;
    }

    private void setPhaseSpace(List<PhaseSpace> phaseSpace)
    {
        if (particles.Count > 0)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                MyParticle particle = this.particles[i];

                particle.position = new Vector3(phaseSpace[i].x, phaseSpace[i].y, phaseSpace[i].z);
                particle.velocity = new Vector3(phaseSpace[i].dx, phaseSpace[i].dy, phaseSpace[i].dz);
            }
        }
    }

    private List<PhaseSpace> computeStateDerivate(float deltaTime)
    {
        List<PhaseSpace> stateDerivate = null;

        {
            updateAllForces();

            stateDerivate = new List<PhaseSpace>();

            List<Vector3> positions = getParticlesPositions();
            List<Vector3> velocities = getParticlesVelocities();
            List<Vector3> accelerations = getParticlesAccelerations();

            if ((velocities == null || accelerations == null) || (velocities.Count != this.particles.Count || accelerations.Count != this.particles.Count))
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

                        stateDerivate[i].x = positions[i].x + velocities[i].x * deltaTime;
                        stateDerivate[i].y = positions[i].y + velocities[i].y * deltaTime;
                        stateDerivate[i].z = positions[i].z + velocities[i].z * deltaTime;

                        stateDerivate[i].dx = velocities[i].x + accelerations[i].x * deltaTime;
                        stateDerivate[i].dy = velocities[i].y + accelerations[i].y * deltaTime;
                        stateDerivate[i].dz = velocities[i].z + accelerations[i].z * deltaTime;
                    }
                }
            }
        }

        return stateDerivate;
    }
}