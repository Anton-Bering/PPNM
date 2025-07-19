# Homework "Recursive Adaptive Integration"

## Tasks

### A. Recursive open 4-point adaptive integrator

1. Implement a recursive open-quadrature adaptive integrator that estimates the integral of a given function `f(x)` on a given interval `[a,b]` with the required absolute, `acc`, or relative, `eps`, accuracy goals.

   The integrator has to use a higher order quadrature to estimate the integral and an imbedded lower order quadrature to estimate the local error.

   Reuse of points is of utmost importance for the effectiveness of the algorithm.

2. Test your implementation on some interesting integrals.  
   Check that your integrator returns results within the given accuracy goals.

*As suggested in the assignment description, I do this for:*

∫₀¹ dx √(x) = 2/3 ,  
∫₀¹ dx 1/√(x) = 2 ,   
∫₀¹ dx √(1 - x²) = π/2,   
∫₀¹ dx ln(x)/√(x) = -4  

3. Using your integrator implement the error function via its integral representation,  
   `erf(z) = (see the assignment description for the full explanation)`  
   make a plot and compare with the tabulated values.

4. Now, calculate `erf(1)` with your routine using `eps=0` and decreasing `acc=0.1, 0.01, 0.001, …`  (or something like this).  
   Plot (in log-log scale) the (absolute value of the) difference between your result and the exact result as function of `acc`.  

   (According to a chatbot: `erf(1) = 0.84270079294971486934`)


### B. Variable transformation quadratures

1.  Inplement an (open quandrature) adaptive integrator with the Clenshaw–Curtis variable transformation.

2. Calculate some integrals with integrable divergencies at the end-points of the intervals;   
   record the number of integrand evaluations;  
   compare with your ordinary integrator without variable transformation.  

*As suggested in the assignment description, I do this for:*  

∫₀¹ dx 1/√(x) = 2  
∫₀¹ dx ln(x)/√(x) = -4  

3. Compare the number of integrand evaluations with the `python/numpy`'s integration routines.

4. Generalize your integrator to accept infinite limits. 

5. Test your implementation on some (converging) infitine limit integrals and note the number of integrand evaluations.

6. Compare with the `python/numpy` integration routines.

### C. Error estimate

1. Make your integrator estimate and return the integration error.

2. Investigate the quality of this error estimate by calculating some difficult intergrals and comparing the estimated error with the actual error.
