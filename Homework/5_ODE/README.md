# Homework "ODE"

## Objective

Implement an embedded Runge-Kutta stepper with error estimate, and an adaptive-step-size driver for solving Ordinary Differential Equation Initial Value Problems (ODE IVP).

## Tasks

### A. (6 points) Linear spline (linear interpolation)

1. Implement an embedded Runge-Kutta stepper `rkstepXY` of your choice, which advances the solution to the equation: 
dy/dx = f(x, y)
(where y and f are vectors) by a given step h, and estimates the error.

2. Implement an adaptive-step-size driver routine wchich advances the solution from initial-point a to final-point b keeping the specified relative, eps, and absolute, acc, precision. You driver should record the solution in two generic lists, `genlist<double> x` and `genlist<vector> y` and then return the two lists.

3. Debug your routines by solving some interesting systems of ordinary differential equations.

4. Reproduce the example from the [scipy.integrate.odeint manual](https://docs.scipy.org/doc/scipy/reference/generated/scipy.integrate.odeint.html) (oscillator with friction) and/or the example from the [scipy.integrate.solve_ivp manual](https://docs.scipy.org/doc/scipy/reference/generated/scipy.integrate.solve_ivp.html) (Lotka-Volterra system).


### B. (3 points) Relativistic precession of planetary orbit

### C. (1 points) Test of the order of the method; Alternative interface; Newtonian gravitational three-body problem

1. Implement the cubic spline with derivative and definite integral (anti-derivative).

2. Check that the built-in cubic splines in pyxplot/gnuplot (or the `spline` utility from plotutils) produces a similar cubic spline to your implementation
