# Homework "ODE"

## Objective

Implement an embedded Runge-Kutta stepper with error estimate, and an adaptive-step-size driver for solving Ordinary Differential Equation Initial Value Problems (ODE IVP).

## Tasks

### A. (6 points) Linear spline (linear interpolation)

1. Implement an embedded Runge-Kutta stepper `rkstepXY` (where XY describes the order of the imbedded method used, for example XY=12—"one-two"—for the midpoint-Euler method) of your choice, which advances the solution to the equation: 
dy/dx = f(x, y)
(where y and f are vectors) by a given step h, and estimates the error.

2. Implement an adaptive-step-size driver routine wchich advances the solution from initial-point a to final-point b keeping the specified relative, eps, and absolute, acc, precision. You driver should record the solution in two generic lists, `genlist<double> x` and `genlist<vector> y` and then return the two lists.

3. Debug your routines by solving some interesting systems of ordinary differential equations.

*As suggested in the assignment description, I do this for: u''=-u*

4. Reproduce (with your routines) the example from the [scipy.integrate.odeint manual](https://docs.scipy.org/doc/scipy/reference/generated/scipy.integrate.odeint.html) (oscillator with friction) and/or the example from the [scipy.integrate.solve_ivp manual](https://docs.scipy.org/doc/scipy/reference/generated/scipy.integrate.solve_ivp.html) (Lotka-Volterra system).

*I do both*

### B. (3 points) Relativistic precession of planetary orbit

Consider the equation (in certain units) of equatorial motion of a planet around a star in General Relativity,  
u''(φ) + u(φ) = 1 + εu(φ)^2  

where where u(φ) ≡ 1/r(φ),  
r is the (reduced-circumference) radial coordinate,  
φ is the azimuthal angle,  
ε is the relativistic correction (on the order of the star's Schwarzschild radius divided by the radius of the planet's orbit),  
and primes denote the derivative with respect to φ.  

1. Integrate (for several rotations) this equation with ε=0 and initial conditions u(0)=1, u'(0)=0 : this should give a Newtonian circular motion.  

2. Integrate (for several rotations) this equation with ε=0 and initial conditions u(0)=1, u'(0)≈-0.5 : this should give a Newtonian elliptical motion. Hint: u'(0) shouldn't bee too large or you will lose your planet.  

3. Integrate (for several rotations) this equation with ε≈0.01 and initial conditions u(0)=1, u'(0)≈-0.5 : this should illustrate the relativistic precession of a planetary orbit.  

### C. (1 points) Test of the order of the method; Alternative interface; Newtonian gravitational three-body problem

1. Implement an imbedded stepper of order 23.  
   Test whether it intergates the equation y''=2x exactly: solve this equation numerically, and check that the driver keeps doubling the step-size still producing the exact result.

2. Implement a quadratic spline interpolation routine that interpolates tables, {xᵢ,yᵢ}, of vector-valued functions y=f(x).

3. Change the interface of your driver such that it returns not the calculated table but the quadratic spline of the table.

4. Using your numerical ODE integrator reproduce a solution to the three-body problem. 
