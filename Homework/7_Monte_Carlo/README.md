# Homework "Monte Carlo integration"

## Tasks

### A. Plain Monte Carlo integration

1. Implement plain Monte Carlo multi-dimensional integration.

2. Calculate some interesting two-dimensional integrals with your Monte-Carlo routine.

   Plot the estimated error and the actual error as functions of the number of sampling points. 

   Check whether the actual error scales as: `1/√N`.

*As suggested in the assignment description, I calculate the area of a unit circle*  
*Furthermore, I also calculate the Gaussian Bell Integral*

3. Try calculate:  

   ∫₀ᵖⁱ dx/π ∫₀ᵖⁱ dy/π ∫₀ᵖⁱ dz/π [1 - cos(x) cos(y) cos(z)]⁻¹ = Γ(1/4)⁴ / (4π³) ≈ 1.39320392968567859184246268255  

   This is a difficult singular integral, do not use it for your tests and error estimates, use something less sungular.

*For my tests and error estimates, I choose to use the (less sungular) 3D integral:*  
∫₀ᵖⁱ dx ∫₀ᵖⁱ dy ∫₀ᵖⁱ dz cos(x) cos(y) cos(z) /π³ = 8/π³ ≈ 0.2580122754655959134753764215085

### B. Quasi-random sequences

1. Implement a multidimensional Monte-Carlo integrator that uses low-discrepancy (quasi-random) sequences. 

   The error could be estimated by using two different sequences. 
   Compare the scaling of the error with your pseudo-random Monte-Carlo integrator.
  
### C. Stratified sampling

1. Implement recursive stratified sampling, the variant where the subroutine receives the number of points N as an argument.
