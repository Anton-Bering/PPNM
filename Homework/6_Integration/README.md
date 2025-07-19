# Homework "Recursive Adaptive Integration"

## Tasks

### A. Recursive open 4-point adaptive integrator

1. Implement a recursive open-quadrature adaptive integrator that estimates the integral of a given function `f(x)` on a given interval `[a,b]` with the required absolute, `acc`acc, or relative, `eps`, accuracy goals.

The integrator has to use a higher order quadrature to estimate the integral and an imbedded lower order quadrature to estimate the local error.

Reuse of points is of utmost importance for the effectiveness of the algorithm.

2. Test your implementation on some interesting integrals.

*As suggested in the assignment description, I do this for:*

∫₀¹ dx √(x) = 2/3 ,
∫₀¹ dx 1/√(x) = 2 , 
∫₀¹ dx √(1 - x²) = π/2, 
∫₀¹ dx ln(x)/√(x) = -4

Check that your integrator returns results within the given accuracy goals.

3. Using your integrator implement the error function via its integral representation,
erf(z) = ...

make a plot and compare with the tabulated values.

4. 

According to a chatbot
...

Now, calculate erf(1) with your routine using eps=0 and decreasing acc=0.1, 0.01, 0.001, …  (or something like this). Plot (in log-log scale) the (absolute value of the) difference between your result and the exact result as function of acc.

### B. Variable transformation quadratures
  
### C. Error estimate
