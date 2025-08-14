# Homework "Minimization"

## Tasks

### A. Newton's method with numerical gradient, numerical Hessian matrix and back-tracking linesearch

1. Implement the Newton minimization method with numerical gradient, numerical Hessian matrix, and back-tracking linesearch.

2. Find a minimum of the Rosenbrock's valley function, `f(x,y) = (1 - x)^2 + 100 (y - x^2)^2`.

*The minimum is: f(x,y)=(1,1)*   

3. Find a minimum of the Himmelblau's function, `f(x,y) = (x^2 + y - 11)^2 + (x + y^2 - 7)^2`.

*The minimum is:*  

*f(x,y)=(3, 2)*  
*f(x,y)=(−2.805118, 3.131313)*  
*f(x,y)=(−3.779310, −3.283186)*  
*f(x,y)=(3.584428, −1.848127)*  

4. Record the number of steps it takes for the algorithm to reach the minimum.

### B. Higgs boson discovery

1. Fit the Breit-Wigner function: `F(E|m,Γ,A) = A /[(E-m)² + Γ² /4]`  
   (where A is the scale-factor, m is the mass and Γ is the widths of the resonance)  
   to the data (2012 CERN) and determine the mass and the experimetnal width of the Higgs boson.

2. Plot your fit together with the experimental data.

### C. Central instead of forward finite difference approximation for the derivatives

1. Implement the central finite difference approximations for the derivatives, compare the resulting algorithm with the one in Part~A and find out which is bettter.  

You must take advantage of the fact that the estimates of the first and second derivatives share the function evaluations.
