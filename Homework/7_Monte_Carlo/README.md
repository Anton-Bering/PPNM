# Homework "Monte Carlo integration"

## Tasks

### A. Plain Monte Carlo integration

1. Implement plain Monte Carlo multi-dimensional integration

2. Calculate some interesting two-dimensional integrals with your Monte-Carlo routine.

Plot the estimated error and the actual error as functions of the number of sampling points. 

Check whether the actual error scales as `1/√N`.

*As suggested in the assignment description, I calculate the area of a unit circle*
*Furthermore, I also calculate the Gaussian Bell Integral*

3. Try calculate: ∫₀^π dx / π ∫₀^π dy / π ∫₀^π dz / π [1 - cos(x) cos(y) cos(z)]⁻¹
= Γ(1/4)⁴ / (4π³) ≈ 1.39320392968567859184246268255

`∫₀^π dx / π ∫₀^π dy / π ∫₀^π dz / π [1 - cos(x) cos(y) cos(z)]⁻¹
= Γ(1/4)⁴ / (4π³) ≈ 1.39320392968567859184246268255`

(This is a difficult singular integral, do not use it for your tests and error estimates, use something less sungular.)

### B. Quasi-random sequences

1. Implement a multidimensional Monte-Carlo integrator that uses low-discrepancy (quasi-random) sequences. 

The error could be estimated by using two different sequences. 

Compare the scaling of the error with your pseudo-random Monte-Carlo integrator.
  
### C. Stratified sampling

1. Implement recursive stratified sampling, the variant where the subroutine receives the number of points N as an argument.


`...kode..ish...`
[TEXT](LINK)

xᵢ



*Furthermore, I also do it for: ...*  
*and for: ...*  

*I do both*do this for