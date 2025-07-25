# Homework "Root Finding"

## Tasks

### A. Newton's method with numerical Jacobian and back-tracking line-search

1. Implement the Newton's method with simple backtracking line-search algorithm where the derivatives in the Jacobian matrix are calculated numerically using finite differences (as described in the Book).  

   Something like this:

```csharp
static vector newton(
    Func<vector, vector> f   /* the function to find the root of */
    ,vector start             /* the start point */
    ,double acc = 1e-2        /* accuracy goal: on exit ‖f(x)‖ should be < acc */
    ,vector δx = null         /* optional δx-vector for calculation of Jacobian */
){
...
return x;
}
```
   The routine returns a vector x that approximates the root of the equation f(x)=0 such that ‖f(x)‖<acc.  

   The vector δx to be used in the finite-difference numerical evaluation of the Jacobian depends on the problem at hand  
   and should be supplied by the user. If the user does not supply it, the routine can choose it as  
   `δxᵢ = |xᵢ|*2^{-26}`  
   or as (might work better sometimes)  
   `δxᵢ = Max(|xᵢ|,1)*2^{-26}`.

   The Jacobian can be estimated numerically like this:

```csharp
public static matrix jacobian(
    Func<vector, vector> f
    ,vector x
    ,vector fx = null
    ,vector dx = null
){
    ...
    return J;
}
```

   One should stop one's iterations if either the condition ‖f(x)‖<acc is satisfied  
   or if the step-size becomes smaller than the size of the δx parameter in your numerical gradient calculation.

2. You should use your own routines for solving linear systems.

3. Debug your root-finding routine using some simple one- and two-dimensional equations.

4. Find the extremum(s) of the [Rosenbrock's valley function](https://en.wikipedia.org/wiki/Rosenbrock_function),  
   `f(x,y) = (1-x)^2 + 100(y-x^2)^2`,  
    by searching for the roots of its gradient (you should calculate the latter analytically).

5. Find the minimum(s) of the [Himmelblau's function](https://en.wikipedia.org/wiki/Himmelblau%27s_function),  
   `f(x,y) = (x^2 + y-11)^2 + (x + y^2 - 7)^2`,  
    by searching for the roots of its (analytic) gradient.

### B. Bound states of hydrogen atom with [shooting method for boundary value problems](https://en.wikipedia.org/wiki/Shooting_method)

1. Find the lowest root, E₀, of the equation M(E)=0 for rₘₐₓ=8.  

   Plot the resulting wave function and compare with the exact result  
   (which is E₀=-1/2, f₀(r)=r·e⁻ʳ — check this by inserting E₀ and f₀(r) into the Schrödinger equation).


2. Investigate the convergence of your solution towards the exact result with respect to the rₘₐₓ and rₘᵢₙ parameters (separately)  
   as well as with respect to the parameters `acc` and `eps` of your ODE integrator.

### C. Quadratic interpolation line-search

1. (In the naïve implementation in part A we allocate — at each step — a new matrix to keep the Jacobian.)  
   Optimize the implementation by only allocating one matrix in the beginning and then updating it at each step.

2. Implement the quadratic interpolation line-search (as described in the book).
