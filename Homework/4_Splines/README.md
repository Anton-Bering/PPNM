# Homework "Splines"

## Objective

Implement functions for spline interpolation of tabulated data points {xᵢ, yᵢ}ᵢ₌₁…ₙ.

## Tasks

### A. (6 points) Linear spline (linear interpolation)

1. Implement a function that makes linear spline interpolation from a table {x[i], y[i]} at a given point z.

2. Implement a function that calculates the integral of the linear spline from the point x[0] to the given point z.

3. Make some indicative plots to prove that your linear spline and your integrator work as intended.

*For this, I use the example from the assignment description, i.e.:*  
*take the table*  
*{xᵢ = 0, 1, ..., 9 ; yᵢ = cos(xᵢ)},*  
*and plot its linear interpolant together with the interpolant's anti-derivative.*

*Furthermore, I do the same for the table {xᵢ = 0, 1, ..., 9 ; yᵢ = xᵢ²}.*

### B. (3 points) Quadratic spline

1. Implement quadratic spline with derivative and definite integral (anti-derivative).

2. Make some indicative plots to prove that your linear spline and your integrator work as intended.

*For this, I use the example from the assignment description, i.e.:*  
*{xᵢ=0,1,…,9; yᵢ=sin(xᵢ)}.*

*Furthermore, I do the same for: {xᵢ=0,1,...,9; yᵢ=ln(xᵢ+1)}*

---
### C. (1 points) Cubic spline

1. Implement the cubic spline with derivative and definite integral (anti-derivative).

2. Check that the built-in cubic splines in pyxplot/gnuplot (or the `spline` utility from plotutils) produces a similar cubic spline to your implementation
