# Quasi-Newton mininization with Broyden's update of the Hessian matrix

This folder contains my solution to the examination project.

The examination task can be found here:  
[https://fedorov.sdfeu.org/prog/projex/minimum-broyden.htm](https://fedorov.sdfeu.org/prog/projex/minimum-broyden.htm)

The examination task is the following:

## Part 1:
Implement a function with the signature
`vector broyden(Func<vector,double> f, vector x, double acc)`
that takes the function f and runs the quasi-Newton algorithm from the starting point x, using the Broyden's update of the Hessian matrix until the accuracty goal acc is reached.

## Part 2:
Test your implementation on some functions with known minima.

## Part 3:
Apply your implementation to a more complicated problem

## Part 4:
Implement also the symmetrized Broyden's update and check whether it is any better.
