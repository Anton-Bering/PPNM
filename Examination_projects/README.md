# [Quasi-Newton mininization with Broyden's update of the Hessian matrix](https://fedorov.sdfeu.org/prog/projex/minimum-broyden.htm)

Here is my folder with my solution for the exam project the course [Practical Programming and Numerical Methods (forår 2025)](https://fedorov.sdfeu.org/prog/).

## The task for the eksamen is the fowlering:

1. Implement a function with the signature
'vector broyden(Func<vector,double> f, vector x, double acc)'
that takes the function 'f' and runs the quasi-Newton algorithm from the starting point 'x', using the Broyden's update of the Hessian matrix until the accuracty goal 'acc' is reached ([see the book for the details](https://fedorov.sdfeu.org/prog/book/book.pdf)).

2. Test your implementation on some functions with known minima.

3. Apply your implementation to a more complicated problem

## Below is a summary of my solution:

1. The implementation, of the function that runs the quasi-Newton algorithm using the Broyden's update of the Hessian matrix, is made in the file QuasiNewton.cs.

2. to test my implementation on some functions with known minima, I juses the the following test functions:

The quadratic function: f(x,y) = x^2 + y^2*
with known minimum at (0,0)

The [Rosenbrock's valley function](https://en.wikipedia.org/wiki/Rosenbrock_function): f(x,y) = (1-x)^2+100(y-x^2)^2*
with known minimum at (1,1)

The [Himmelblau's function](https://en.wikipedia.org/wiki/Himmelblau%27s_function): f(x,y) = (x^2+y-11)^2+(x+y^2-7)^2*
with known minimum at (3,2), (−2.805, 3.131), (−3.779, −3.283), and (3.584, −1.848)

All of this functions is defint in the fiel Problems.cs, and the faunde minima is in agremete witch the expatiationes (borthe withe the (normal) Broyden's update and the the symmetrized Broyden's update).

3. del 1: To apply my implementation to a more complicated problem, I first make it a little more complicated, by using the quadratic function and Rosenbrock's valley function once again, but in higher dimensions (8D).*

The faunde minima is in agremete witch the expatiatione (borthe withe the (normal) Broyden's update and the the symmetrized Broyden's update)

3. del 2: For a more complex problem, I followed a procedure similar to that in homework problem 9 to find the mass of the Higgs boson. But unlike in Homework 9, where Newton’s method with numerical Hessian was used, here I apply my Quasi-Newton implementation with Broyden’s update (and the symmetrized update). 

The faunde mimumia (for borthe the (normal) Broyden's update and the symmetrized Broyden's update) at [125.97 2.086 9.88] koraspont to the Higgs mass, and the widths of the resonance, and the scale-factor, respekley.  

The generalte (juseing the symmetrized Broyden's update) file Higgs_fit.svg , showes a plot of the signal [certain units] as a function of Enerdy [GeV], witch the expermintel data (CERN 2012) along witch the fint. The pekke in the fit (at aunde 125.3 GeV) coraspont to the Higgs mass (in unites of GeV/c²). 

3. del 3: For another, more complex problem, I followed a procedure similar to that in homework problem 10, where an ANN was trained to approximate the function g(x) = cos(5x−1)·exp(−x²). But, unlike in homework 10, where the training relied on gradient-based methods, here I once again use my Quasi-Newton implementation.


The genrate plot ANN_fit.svg showes a plot of the g(x) as a function of x, form the ANN aprogimation of the function along witch the analytisk seluation, as sigen from the plote the ANN aprigesime alirens witne the analysisty selutiosion 

The functions for all of this thre more complekate problmes is defint in the firel problems.cs 

4. The implement of the the symmetrized Broyden's update i made alonget withe the (normel) Broyden's update i the file QuasiNewton.cs.
For all the 2D test function (Quadratic, Rosenbrock's valley, and Himmelblau's), and the higher demetiale (8D) functions (Quadratic, and Rosenbrock's valley) 
