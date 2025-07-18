# Homework "Splines"

## Objective

Implement functions for spline interpolation of tabulated data points {xᵢ, yᵢ}ᵢ₌₁…ₙ.

## Tasks

### A. (6 points) Linear spline (linear interpolation)

1. Implement a function that makes linear spline interpolation from a table {x[i], y[i]} at a given point z

2. Implement a function that calculates the integral of the linear spline from the point x[0] to the given point z.

3. Make some indicative plots to prove that your linear spline and your integrator work as intended.

### B. (3 points) Quadratic spline

1. Implement quadratic spline with derivative and definite integral (anti-derivative).

2. Make some indicative plots to prove that your linear spline and your integrator work as intended.

---
### C. (1 points) Cubic spline

1. Implement the cubic spline with derivative and definite integral (anti-derivative).

2. Check that the built-in cubic splines in pyxplot/gnuplot (or the `spline` utility from plotutils) produces a similar cubic spline to your implementation
