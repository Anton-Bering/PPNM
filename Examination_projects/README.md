[Quasi-Newton mininization with Broyden's update of the Hessian matrix](https://fedorov.sdfeu.org/prog/projex/minimum-broyden.htm)

Here is my folder with my solution for the exam project the course [Practical Programming and Numerical Methods (forår 2025)](https://fedorov.sdfeu.org/prog/).

The task for the eksamen is the fowlering:

1. Implement a function with the signature
'vector broyden(Func<vector,double> f, vector x, double acc)'
that takes the function 'f' and runs the quasi-Newton algorithm from the starting point 'x', using the Broyden's update of the Hessian matrix until the accuracty goal 'acc' is reached ([see the book for the details](https://fedorov.sdfeu.org/prog/book/book.pdf)).

2. Test your implementation on some functions with known minima.

*For this I use the following test functions:*

*The quadratic function: f(x,y) = x^2 + y^2*
*Known minimum at (0,0)*

*The [Rosenbrock's valley function](https://en.wikipedia.org/wiki/Rosenbrock_function): f(x,y) = (1-x)^2+100(y-x^2)^2*
*Known minimum at (1,1)*

*The [Himmelblau's function](https://en.wikipedia.org/wiki/Himmelblau%27s_function): f(x,y) = (x^2+y-11)^2+(x+y^2-7)^2*
*Known minima at (3,2), (−2.805, 3.131), (−3.779, −3.283), and (3.584, −1.848)*

3. Apply your implementation to a more complicated problem
*At first, to make it a little more complicated, I used the quadratic function and Rosenbrock's valley function once again, but in higher dimensions (8D).*

*For a more complex problem, I followed a procedure similar to that in homework problem 9 to find the mass of the Higgs boson.*
*But unlike in Homework 9, where Newton’s method with numerical Hessian was used, here I apply my Quasi-Newton implementation with Broyden’s update (and the symmetrized update)*

*For another, more complex problem, I followed a procedure similar to that in homework problem 10, where an ANN was trained to approximate the function g(x) = cos(5x−1)·exp(−x²).*
*but, unlike in homework 10, where the training relied on gradient-based methods, here I once again use my Quasi-Newton implementation.*

4. Implement also the symmetrized Broyden's update and check whether it is any better.

*...*
