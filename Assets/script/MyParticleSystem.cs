using System.Collections.Generic;
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

        var newState = computeStateDerivate(phaseSpaceState, deltaTime);

        this.setPhaseSpace(newState);

        this.systemTime = timeEnd;
    }

    private void updateAllForces()
    {
        clearSysForces();

        UpdateLines();
        updateExternalForce();
        updateSprings();
        updateAttractions();
        updateDrag();
        addGravity();
    }

    private void updateExternalForce ()
    {
        foreach (MyParticle particle in particles)
        {
            Vector3 ExternalForce = -particle.force;

            particle.AddForce(ExternalForce);
        }

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

                    var gourndlevel = 0;

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

            phaseSpace[i].dx = velocities[i].x;
            phaseSpace[i].dy = velocities[i].y;
            phaseSpace[i].dz = velocities[i].z;
        }

        return phaseSpace;
    }

    private List<PhaseSpace> computeStateDerivate(List<PhaseSpace> phaseSpaceStates, float deltaTime) //not sure time needed // herein lays the problem
    {
        List<PhaseSpace> stateDerivate = null;

        //if (this.currentPhaseSpace != null)
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