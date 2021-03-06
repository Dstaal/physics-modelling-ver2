﻿using System.Collections.Generic;
using UnityEngine;

public class MyParticleSystem : MonoBehaviour
{
    public Vector3 gravity = Vector3.zero;
    public float drag = 0;
    public float SamplingRate = 10f; // 10 ms

    public List<MyParticle> particles;
    public List<MySpring> springs;
    public List<MyAttraction> attractions;

    public float systemTime = 0f;

    public List<PhaseSpace> currentPhaseSpace = new List<PhaseSpace>();

    private float lastSample = 0f;

    public delegate float func(float x, float y);

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
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
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
        //advanceParticlesAges(Time.fixedDeltaTime);
    }

    private void advanceTime(float deltaTime)
    {
        // TODO: kill old particles

        var timeStart = systemTime;
        var timeEnd = timeStart + deltaTime;

        var phaseSpaceState = getPhaseSpaceState();

        var newState = computeStateDerivate2(phaseSpaceState);
        this.setPhaseSpace(newState);

        this.systemTime = timeEnd;

        // TODO: advance particle ages

        //killOldParticles(); //havn't made. snice my partcile don't have a lifespan

        //this.currentPhaseSpace = getPhaseSpaceState(); //still no useing it for anything / need to be passed inot evaluation step

        //// hmm i thouth phaseSpace was supose to be one vector with all vaules.  but mine is making sevreal lists

        //List<PhaseSpace> newState = computeStateDerivate(this.currentPhaseSpace); // add new sruff but  // should be put into ode4

        //this.currentPhaseSpace = newState; // this doesn't do anything? // now it does, as we pass it in setPahseSpase below - but for not resaoen, other then we made the var

        //setPhaseSpace(this.currentPhaseSpace);

        ////for (int i = 0; i < newState.Count; i++)
        ////{
        ////    Debug.Log("newState nr : " + i + "  data : " + newState[i]);
        //}
    }

    private List<float> diff(List<float> list)
    {
        var diffed = new List<float>(list.Count);

        for (int i = 1; i < list.Count; i++)
        {
            diffed[i] = list[i] - list[i - 1];
        }

        return diffed;
    }

    private List<PhaseSpace> computeStateDerivate2(List<PhaseSpace> spaceState)
    {
        this.setPhaseSpace(spaceState);

        this.updateAllForces(); // aggregate forces

        var velocities = this.getParticlesVelocities();
        var accelerations = this.getParticlesAccelerations();

        var stateDerivate = new List<PhaseSpace>(this.particles.Count);
        for (int i = 0; i < this.particles.Count; i++)
        {
            var state = new PhaseSpace(velocities[i], accelerations[i]);
            stateDerivate.Add(state);
        }

        return stateDerivate;
    }

    //private List<PhaseSpace> ode4(Action computeStateDerivate, List<float> tspan, List<PhaseSpace> phaseSpaceState)
    //{
    //    var y0 = phaseSpaceState;
    //    var h = diff(tspan);
    //    var f0 = computeStateDerivate2(y0);

    //    var neq = y0.Count;
    //    var N = tspan.Count;
    //    var Y = new List<List<PhaseSpace>>(N);
    //    var F = new List<List<PhaseSpace>>(4);

    //    Y[0] = f0;
    //    for (int i = 1; i < N; i++)
    //    {
    //        var ti = tspan[i - 1];
    //        var hi = h[i - 1];
    //        var yi = Y[i - 1];

    //        F[0] = computeStateDerivate2(phaseSpaceState);

    //        var yiAndAHalf = Utils.AddToEachElement(yi, 0.5f);
    //        //var f1Ti = Utils.MultiplyEachElement(F[0], Utils.MultiplyEachElement(Utils.AddToEachElement(yi, 0.5f), hi));
    //        //F[1] = computeStateDerivate2()
    //    }
    //}

    /*  matlab avdance time to convert to c#
     *
     *   function advance_time (Particle_System, step_time)
            %ADVANCE_TIME  Advance particle system time.
            %
            %   Example
            %
            %   ADVANCE_TIME (PS, ST) increments the current time property
            %   of the particle system PS for step time ST.

            %   Copyright 2008-2008 buchholz.hs-bremen.de

            Particle_System.kill_old_particles;

            time_start = Particle_System.time;
            time_end = time_start + step_time;

--------------- from here ------------------

            phase_space_state = Particle_System.get_phase_space_state;

            phase_space_states = ode4 (...
                @compute_state_derivative, ...
                [time_start, time_end], ...
                phase_space_state, ...
                Particle_System);

            phase_space_state = phase_space_states(2,:);

--------------- to here, is whats missing in mine -----------------

            Particle_System.set_phase_space_state (phase_space_state);

            Particle_System.time = time_end;

            Particle_System.advance_particles_ages (step_time);

            Particle_System.update_graphics_positions;

    */

    private void updateAllForces()
    {
        clearSysForces();

        // updateForces(); // not during this here in the matlab
        UpdateLines();
        updateSprings();
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
    }

    private void addGravity()
    {
        //if (particles.Count > 0)
        //{
        //    foreach (MyParticle particle in particles)
        //    {
        //        if (particle.transform.position.y > 0)
        //        {
        //            Vector3 gravityForce = this.gravity * particle.mass;
        //            particle.AddForce(gravityForce);
        //        }
        //        //else if (particle.transform.position.y < 0)
        //        //{
        //        //    var pos = particle.position;

        //        //    var gourndLevel = 0;

        //        //    Vector3 dontDropBelow = new Vector3(pos.x, gourndLevel, pos.z);

        //        //    particle.position = dontDropBelow; // is this the best way?
        //        //}
        //    }
        //}
    }

    //private void updateForces()
    //{
    //    if (particles.Count > 0)
    //    {
    //        foreach (MyParticle particle in particles)
    //        {
    //            particle.position = particle.position + particle.force;
    //        }
    //    }
    //}

    private void updateSprings()
    {
        if (springs.Count > 0)
        {
            foreach (MySpring spring in springs)
            {
                Vector3 position_delta = spring.targetTwo.position - spring.targetOne.position;

                float position_delta_Nrm = position_delta.magnitude; // magnitude the same a normlizing? nope.

                if (position_delta_Nrm < 1f)  // and eps would be better then 1f
                {
                    position_delta_Nrm = 1f;
                }

                Vector3 position_delta_unit = position_delta / position_delta_Nrm; // so here im normalixing. but a few steps later then matlab koden

                Vector3 springForce = spring.strength * position_delta_unit * (position_delta_Nrm - spring.rest);

                //Debug.Log("springforce : " + springForce);

                spring.targetOne.AddForce(springForce);
                spring.targetTwo.AddForce(-springForce);

                Vector3 velocityDelta = spring.targetTwo.velocity - spring.targetOne.velocity;

                // add ref to the pdf here:
                Vector3 projectionVelocityDeltaOnPositionDelta = Vector3.Dot(position_delta_unit, velocityDelta) * position_delta_unit;
                Vector3 dampingForce = spring.damping * projectionVelocityDeltaOnPositionDelta;

                spring.targetOne.AddForce(dampingForce);
                spring.targetTwo.AddForce(-dampingForce);
            }
        }
    }

    /*
     *
     *
     *  function aggregate_springs_forces (Particle_System)
            %AGGREGATE_SPRINGS_FORCES  Aggregate spring forces.
            %
            %   Example
            %
            %   AGGREGATE_SPRINGS_FORCES (PS) aggregates the forces of all
            %   springs in the particle system PS on all particles they are connected to
            %   in the corresponding particle force accumulators.
            %
            %   See also aggregate_forces, aggregate_attraction_forces, aggregate_drag_forces,
            %   aggregate_gravity_forces.

            %   Copyright 2008-2008 buchholz.hs-bremen.de

            for i_spring = 1 : length (Particle_System.springs)

                Spring = Particle_System.springs(i_spring);

                Particle_1 = Spring.particle_1;
                Particle_2 = Spring.particle_2;

                position_delta = Particle_2.position - Particle_1.position;

                position_delta_norm = norm (position_delta);

                % If the user makes the initial positions of two particles identical
                % we have to avoid a "divide by zero" exception
                if position_delta_norm < eps

                    position_delta_norm = eps;

                end

                position_delta_unit = position_delta/position_delta_norm;

                spring_force = Spring.strength*position_delta_unit*(position_delta_norm - Spring.rest);

                Particle_1.add_force (spring_force);
                Particle_2.add_force (-spring_force);

                velocity_delta = Particle_2.velocity - Particle_1.velocity;

                projection_velocity_delta_on_position_delta = ...
                    dot (position_delta_unit, velocity_delta)*position_delta_unit;

                damping_force = Spring.damping*projection_velocity_delta_on_position_delta;

                Particle_1.add_force (damping_force);
                Particle_2.add_force (-damping_force);

            end

        end

        */

    public void updateDrag()
    {
        if (particles.Count > 0)
        {
            foreach (MyParticle particle in particles)
            {
                Vector3 dragForce = - this.drag * particle.velocity;

                particle.AddForce(dragForce);
            }
        }
    }

    /*

  function aggregate_drag_forces (Particle_System)
            %AGGREGATE_DRAG_FORCES  Aggregate drag forces.
            %
            %   Example
            %
            %   AGGREGATE_DRAG_FORCES (PS) aggregates the drag forces
            %   in the particle system PS on all particles
            %   in the corresponding particle force accumulators.
            %
            %   See also aggregate_forces, aggregate_attraction_forces, aggregate_gravity_forces,
            %   aggregate_spring_forces.

            %   Copyright 2008-2008 buchholz.hs-bremen.de

            for i_particle = 1 : length (Particle_System.particles)

                Particle = Particle_System.particles(i_particle);

                drag_force = - Particle_System.drag*Particle.velocity;

                Particle.add_force (drag_force);

            end

        end

*/

    public void updateAttractions() //not in use yet
    {
        foreach (MyAttraction _attreaction in attractions)
        {
            Vector3 position_delta = _attreaction.targetTwo.position - _attreaction.targetOne.position;

            float position_delta_Nrm = position_delta.magnitude; // magnitude the same a normlizing? nope. // this could be a problem, the matlabs is normalized

            if (position_delta_Nrm < _attreaction.minimumDistance)
            {
                position_delta_Nrm = _attreaction.minimumDistance;
            }

            Vector3 _attreactionForce = _attreaction.strength * _attreaction.targetOne.mass * _attreaction.targetTwo.mass * position_delta / position_delta_Nrm / position_delta_Nrm / position_delta_Nrm; // why the hell  /position_delta_Nrm 3times?

            _attreaction.targetOne.AddForce(_attreactionForce);
            _attreaction.targetTwo.AddForce(_attreactionForce);
        }
    }

    /*
     *         function aggregate_attractions_forces (Particle_System)
            %AGGREGATE_ATTRACTIONS_FORCES  Aggregate attraction forces.
            %
            %   Example
            %
            %   AGGREGATE_ATTRACTIONS_FORCES (PS) aggregates the forces of all
            %   attractions in the particle system PS on all particles they are connected to
            %   in the corresponding particle force accumulators.
            %
            %   See also aggregate_forces, aggregate_drag_forces, aggregate_gravity_forces,
            %   aggregate_spring_forces.

            %   Copyright 2008-2008 buchholz.hs-bremen.de

            for i_attraction = 1 : length (Particle_System.attractions)

                Attraction = Particle_System.attractions(i_attraction);

                Particle_1 = Attraction.particle_1;
                Particle_2 = Attraction.particle_2;

                position_delta = Particle_2.position - Particle_1.position;

                position_delta_norm = norm (position_delta);

                if position_delta_norm < Attraction.minimum_distance

                    position_delta_norm = Attraction.minimum_distance;

                end

                attraction_force = ...
                    Attraction.strength* ...
                    Particle_1.mass* ...
                    Particle_2.mass* ...
                    position_delta/ ...
                    position_delta_norm/ ...
                    position_delta_norm/ ...
                    position_delta_norm;

                Particle_1.add_force (attraction_force);
                Particle_2.add_force (-attraction_force);

            end

        end

*/

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

                // TODO: Should it plus ?
                particle.position = new Vector3(phaseSpace[i].x, phaseSpace[i].y, phaseSpace[i].z);
                particle.velocity = new Vector3(phaseSpace[i].x_v, phaseSpace[i].y_v, phaseSpace[i].z_v);
            }
        }
    }

    /*
     *
     *         function set_phase_space_state (Particle_System, phase_space_state)
            %SET_PHASE_SPACE_STATE  Set phase space state vector.
            %
            %   Example
            %
            %   SET_PHASE_SPACE_STATE (PS, SS) sets the
            %   phase space state vector of the particle system PS to SS.
            %   SS must be a 1-by-6*N vector. N is the number of particles.
            %
            %   See also get_phase_space_state.
            %
            %   Copyright 2008-2008 buchholz.hs-bremen.de

            n_particles = length (Particle_System.particles);

            for i_particle = 1 : n_particles

                Particle = Particle_System.particles(i_particle);

                Particle.position = ...
                    phase_space_state(3*i_particle - 2 : 3*i_particle);

                Particle.velocity = ...
                    phase_space_state(3*(i_particle + n_particles) - 2 : 3*(i_particle + n_particles));

            end

        end
     */

    private List<PhaseSpace> getPhaseSpaceState()
    {
        List<Vector3> positions = getParticlesPositions();
        List<Vector3> velocities = getParticlesVelocities();

        if ((positions == null || velocities == null) || (positions.Count != this.particles.Count || velocities.Count != this.particles.Count))
        {
            Debug.LogWarning("ERROR: positions, velocities and Particles lists are not same length or null!!");
            return null;
        }

        List<PhaseSpace> phaseSpace = new List<PhaseSpace>();
        for (int i = 0; i < this.particles.Count; i++)
        {
            phaseSpace.Add(new PhaseSpace());

            phaseSpace[i].x = positions[i].x;
            phaseSpace[i].y = positions[i].y;
            phaseSpace[i].z = positions[i].z;

            phaseSpace[i].x_v = velocities[i].x;
            phaseSpace[i].y_v = velocities[i].y;
            phaseSpace[i].z_v = velocities[i].z;
        }

        return phaseSpace;
    }

    /*
     *
     *        function phase_space_state = get_phase_space_state (Particle_System)
            %GET_PHASE_SPACE_STATE  Retrieve phase space state vector.
            %
            %   Example
            %
            %   SS = GET_PHASE_SPACE_STATE (PS) returns the current 1-by-6*N
            %   phase space state vector SS from the particle system PS.
            %   N is the number of particles.
            %
            %   See also set_phase_space_state.
            %
            %   Copyright 2008-2008 buchholz.hs-bremen.de

            positions = get_particles_positions (Particle_System);
            velocities = get_particles_velocities (Particle_System);

            phase_space_state = [positions, velocities];

        end

*/

    // see the pdf page 56. quuestion about ode4. claims this is called 4 times by that.

    //function state_derivative = compute_state_derivative ...
    //(time , phase_space_state , Particle_System )

    // so i supose i have to pass in tiem, parentsystem and phasestate too

    //private List<PhaseSpace> computeStateDerivate(List<PhaseSpace> phaseSpaceStates) //not sure time needed // herein lays the problem
    //{
    //    List<PhaseSpace> stateDerivate = null;

    //    //if (this.currentPhaseSpace != null)
    //    {
    //        //the matlab code sets phaseSpace here before updateing forces.

    //        /*

    //        The evaluation is initialized by transposing the state vector into a row30 vector

    //        phase_space_state = phase_space_state (:) ’;

    //        and inserting it (back) into the particle system

    //        Particle_System . set_phase_space_state ( phase_space_state );

    //        */

    //        //	this.setPhaseSpace(this.currentPhaseSpace); //added in atempt to fix the above // but sends the partilce flying into the air

    //        this.setPhaseSpace(phaseSpaceStates);

    //        updateAllForces(); // should this really update forces? i nkow the matlab does. but the pixar didn't // maybe it a problem i don't have attractions yet

    //        stateDerivate = new List<PhaseSpace>();

    //        List<Vector3> _velocities = getParticlesVelocities();
    //        List<Vector3> _accelerations = getParticlesAccelerations();

    //        if ((_velocities == null || _accelerations == null) || (_velocities.Count != this.particles.Count || _accelerations.Count != this.particles.Count))
    //        {
    //            Debug.LogWarning("ERROR: velocities, accelerations and Particles lists are not same length or null!!");
    //        }
    //        else
    //        {
    //            for (int i = 0; i < this.particles.Count; i++)
    //            {
    //                if (this.particles[i] != null)
    //                {
    //                    stateDerivate.Add(new PhaseSpace());

    //                    stateDerivate[i].x = _velocities[i].x;
    //                    stateDerivate[i].y = _velocities[i].y;
    //                    stateDerivate[i].z = _velocities[i].z;

    //                    stateDerivate[i].x_v = _accelerations[i].x;
    //                    stateDerivate[i].y_v = _accelerations[i].y;
    //                    stateDerivate[i].z_v = _accelerations[i].z; // that are thise used for anyway? can see where they are used / now they are for drag
    //                }
    //            }
    //        }
    //    }

    //    return stateDerivate;
    //}

    /*
     *   function state_derivative = compute_state_derivative ...
                (time, phase_space_state, Particle_System)
            %COMPUTE_STATE_DERIVATIVE  Compute state derivative vector.
            %
            %   Example
            %
            %   SD = COMPUTE_STATE_DERIVATIVE (T, SS, PS) returns the 6*N-by-1
            %   phase space state derivative vector SD of the particle system PS at time T.
            %   SS is the current 6*N-by-1 phase space state vector.
            %   N is the number of particles.
            %
            %   See also ode4, set_phase_space_state, aggregate_forces,
            %   get_particles_velocities, get_particles_accelerations.
            %
            %   Copyright 2008-2008 buchholz.hs-bremen.de

            phase_space_state = phase_space_state(:)';

            Particle_System.set_phase_space_state (phase_space_state);

            Particle_System.aggregate_forces;

            velocities = Particle_System.get_particles_velocities;

            accelerations = Particle_System.get_particles_accelerations;

            state_derivative = [velocities, accelerations]';

        end

*/

    public float rungeKutta(PhaseSpace devState, PhaseSpace currentState, float startTime, float endTime, func func)
    {
        // atm asuming its worknig only doing ode4 in x.pos

        float stepTime = endTime - startTime;

        float x0 = currentState.x;

        float t0 = startTime;

        float k1_x = stepTime * func(x0, t0);

        float k2_x = stepTime * func(x0 + (k1_x / 2), t0 + (stepTime / 2));

        float k3_x = stepTime * func(x0 + (k2_x / 2), t0 + (stepTime / 2));

        float k4_x = stepTime * func(x0 + k3_x, t0 + stepTime);

        float rk_x = x0 + (1 / 6 * k1_x) + (1 / 3 * k2_x) + (1 / 3 * k3_x) + (1 / 6 * k4_x);

        return rk_x;

        /* runge-kutta form pdf page 7
         *

            h =  time step

            k1 = h * f(x0, t0)

            k2 = h * f(x0 + k1/2 , t0 + h/2)

            k3 = h * f(x0 + k2/2 , t0 + h/2)

            k4 = h * f(x0 + k3 , t0 + h)

            x0(t0+h) = x0 + (1/6 * k1) + (1/3 * k2) +(1/3 * k3) +(1/6 * k4)

The simplest numerical method is called Euler’s method. Let our initial value for x be denoted by
x0 = x(t0) and our estimate of x at a later time t0 + h by x(t0 + h) where h is a stepsize parameter.
Euler’s method simply computes x.t0 C h/ by taking a step in the derivative direction,
x(t0 + h) = x0 + h x'(t0):

x' = f(x,t)

x'' = f/m

v' = f/m

x' = v

which for me means

        */
    }

    //--------------------- runge kutta stuff below ----------------------

    /*
     *
     * matlab ode4 to convert to c#
     *
     *  phase_space_states = ode4 (...
                @compute_state_derivative, ...
                [time_start, time_end], ...
                phase_space_state, ...
                Particle_System);
     *
     *
            function Y = ode4(odefun,tspan,y0,varargin)
                %ODE4  Solve differential equations with a non-adaptive method of order 4.
                %   Y = ODE4(ODEFUN,TSPAN,Y0) with TSPAN = [T1, T2, T3, ... TN] integrates
                %   the system of differential equations y' = f(t,y) by stepping from T0 to
                %   T1 to TN. Function ODEFUN(T,Y) must return f(t,y) in a column vector.
                %   The vector Y0 is the initial conditions at T0. Each row in the solution
                %   array Y corresponds to a time specified in TSPAN.
                %
                %   Y = ODE4(ODEFUN,TSPAN,Y0,P1,P2...) passes the additional parameters
                %   P1,P2... to the derivative function as ODEFUN(T,Y,P1,P2...).
                %
                %   This is a non-adaptive solver. The step sequence is determined by TSPAN
                %   but the derivative function ODEFUN is evaluated multiple times per step.
                %   The solver implements the classical Runge-Kutta method of order 4.
                %
                %   Example
                %         tspan = 0:0.1:20;
                %         y = ode4(@vdp1,tspan,[2 0]);
                %         plot(tspan,y(:,1));
                %     solves the system y' = vdp1(t,y) with a constant step size of 0.1,
                %     and plots the first component of the solution.
                %

                if ~isnumeric(tspan)
                    error('TSPAN should be a vector of integration steps.');
                end

                if ~isnumeric(y0)
                    error('Y0 should be a vector of initial conditions.');
                end

                h = diff(tspan);
                if any(sign(h(1))*h <= 0)
                    error('Entries of TSPAN are not in order.')
                end

                try
                    f0 = feval(odefun,tspan(1),y0,varargin{:});
                catch
                    msg = ['Unable to evaluate the ODEFUN at t0,y0. ',lasterr];
                    error(msg);
                end

                y0 = y0(:);   % Make a column vector.
                if ~isequal(size(y0),size(f0))
                    error('Inconsistent sizes of Y0 and f(t0,y0).');
                end

                neq = length(y0);
                N = length(tspan);
                Y = zeros(neq,N);
                F = zeros(neq,4);

                Y(:,1) = y0;
                for i = 2:N
                    ti = tspan(i-1);
                    hi = h(i-1);
                    yi = Y(:,i-1);
                    F(:,1) = feval(odefun,ti,yi,varargin{:});				                    F(:,1) = feval(devState,ti,phaseSpace,particleSystem); // what it's equl to in my code
                    F(:,2) = feval(odefun,ti+0.5*hi,yi+0.5*hi*F(:,1),varargin{:});				F(:,2) = feval(devState,ti+0.5*hi,phaseSpace+0.5*hi*F(:,1),particleSystem{:});
                    F(:,3) = feval(odefun,ti+0.5*hi,yi+0.5*hi*F(:,2),varargin{:});				F(:,3) = feval(devState,ti+0.5*hi,phaseSpace+0.5*hi*F(:,2),particleSystem{:});
                    F(:,4) = feval(odefun,tspan(i),yi+hi*F(:,3),varargin{:});					F(:,4) = feval(devState,tspan(i),yi+hi*F(:,3),particleSystem{:});
                    Y(:,i) = yi + (hi/6)*(F(:,1) + 2*F(:,2) + 2*F(:,3) + F(:,4));				 Y(:,i) = yi + (hi/6)*(F(:,1) + 2*F(:,2) + 2*F(:,3) + F(:,4));
                end
                Y = Y.';

            end

        end

------ from the pdf -------------

After a successful integration step, ode4 returns a state matrix, each row representing
the state vector at one of the specified points of time. Since we already know the state
vector at time_start (corresponding to the first row), we are only interested in the
second row, featuring the state vector at the end of the time step
phase_space_state = phase_space_states (2 ,:);   // does thise sounds like they only want the midway funtion?

--------------------------- an other runge kutta made for c#

http://csharpcomputing.com/tutorials/Lesson16.htm

Finally, let me show a simple code for solving first order ordinary differential equations.
The code uses a Runge-Kutta method. The simplest method to solve ODE is to do a Taylor expansion, which is called Euler's method.
Euler's method approximates the solution with the series of consecutive secants.
The error in Euler's method is O(h) on every step of size h.
The Runge-Kutta method has an error O(h^4)

using System;
//fourth order Runge Kutte method for y'=f(t,y);
//solve first order ode in the interval (a,b) with a given initial condition at x=a and fixed step h.
class Runge{
    public delegate double Function(double t,double y); //declare a delegate that takes a double and returns
//double
    public static void runge(double a, double b,double value, double step, Function f)
    {
          double t,w,k1,k2,k3,k4;
        t=a;
        w=value;

        for(int i=0;i<(b-a)/step;i++)
        {
            k1=step*f(t,w);
            k2=step*f(t+step/2,w+k1/2);
            k3=step*f(t+step/2,w+k2/2);
            k4=step*f(t+step,w+k3);
            w=w+(k1+2*k2+2*k3+k4)/6;
            t=a+i*step;
            Console.WriteLine("{0} {1} ",t,w);
       }
    }
}
class Test
{
    public static double f1(double t, double y)
    {
    return -y+t+1;
    }
    public static void Main()
    {
    Runge.runge(0,1,1,.1f,new Runge.Function(Test.f1));
    }
}
 Runge-Kutta methods with a variable step size are often used in practice since they converge faster than fixed size methods.

---------------------- yet another ----------------------------------

http://letsthinkabout.us/post/runge-kutta-in-c

public static class RungeKutta
{
    public delegate double SmallRkDelegate(double x, double y);

    static double sixth = 1.0 / 6.0;

    public static double rk4(double x, double y, double dx, SmallRkDelegate f)
    {
        double halfdx = 0.5 * dx;

        double k1 = dx * f(x, y);
        double k2 = dx * f(x + halfdx, y + k1 * halfdx);
        double k3 = dx * f(x + halfdx, y + k2 * halfdx);
        double k4 = dx * f(x + dx, y + k3 * dx);

        return (y + sixth * (k1 + 2 * k2 + 2 * k3 + k4));
    }
}

*/

    //end of class
}