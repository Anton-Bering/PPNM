# [Quasi-Newton Minimization with Broyden's Update of the Hessian Matrix](https://fedorov.sdfeu.org/prog/projex/minimum-broyden.htm)

This repository contains my solution for the exam project in the course [Practical Programming and Numerical Methods (Spring 2025)](https://fedorov.sdfeu.org/prog/).

## The task for the exam is the following:

1.  Implement a function with the signature
    `vector broyden(Func<vector,double> f, vector x, double acc)`
    that takes the function `f` and runs the quasi-Newton algorithm from the starting point `x`, using the Broyden's update of the Hessian matrix until the accuracy goal `acc` is reached (see the [course book](https://fedorov.sdfeu.org/prog/book/book.pdf) for the details).

2.  Test your implementation on some functions with known minima.

3.  Apply your implementation to a more complicated problem.

## Below is a summary of my solution:

1.  The implementation of the function that runs the quasi-Newton algorithm using Broyden's update of the Hessian matrix is made in the file `QuasiNewton.cs`.

2.  To test my implementation on some functions with known minima, I used the following test functions:

    *   The quadratic function: `f(x,y) = x² + y²`
        with a known minimum at (0,0).

    *   The [Rosenbrock's valley function](https://en.wikipedia.org/wiki/Rosenbrock_function): `f(x,y) = (1-x)² + 100(y-x²)²`
        with a known minimum at (1,1).

    *   The [Himmelblau's function](https://en.wikipedia.org/wiki/Himmelblau%27s_function): `f(x,y) = (x² + y - 11)² + (x + y² - 7)²`
        with known minima at (3,2), (−2.805, 3.131), (−3.779, −3.283), and (3.584, −1.848).

    All of these functions are defined in the file `Problems.cs`, and the found minima are in agreement with the expectations (for both the standard Broyden's update and the symmetrized Broyden's update).

3.  **Part 1:** To apply my implementation to a more complicated problem, I first made it a little more complicated by using the quadratic function and Rosenbrock's valley function once again, but in higher dimensions (8D).

    The found minima are in agreement with the expectations (for both the standard Broyden's update and the symmetrized Broyden's update).

    **Part 2:** For a more complex problem, I followed a procedure similar to that in homework problem 9 to find the mass of the Higgs boson. But unlike in Homework 9, where Newton’s method with numerical Hessian was used, here I apply my Quasi-Newton implementation with Broyden’s update (and the symmetrized update).

    The found minima (for both the standard Broyden's update and the symmetrized Broyden's update) at approximately `[125.97, 2.086, 9.88]` correspond to the Higgs mass, the width of the resonance, and the scale-factor, respectively.

    The generated file `Higgs_fit.svg` (using the symmetrized Broyden's update) shows a plot of the signal [certain units] as a function of Energy [GeV], with the experimental data (CERN 2012) along with the fit. The peak in the fit at approximately 126.0 GeV corresponds to the Higgs mass (in units of GeV/c²).

    **Part 3:** For another, more complex problem, I followed a procedure similar to that in homework problem 10, where an ANN was trained to approximate the function `g(x) = cos(5x−1)·exp(−x²)`. But, unlike in homework 10, where the training relied on gradient-based methods, here I once again use my Quasi-Newton implementation.

    The generated plot `ANN_fit.svg` shows a plot of g(x) as a function of x, for the ANN approximation of the function along with the analytical solution. As seen from the plot, the ANN approximation aligns well with the analytical solution.

    The functions for all three of these more complicated problems are defined in the file `problems.cs`.

4.  The implementation of the symmetrized Broyden's update is made alongside the standard Broyden's update in the file `QuasiNewton.cs`.