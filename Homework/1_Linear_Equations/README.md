# Homework "Linear Equations"

## Objective

Implement functions to:  
    1. solve linear equations
    2. calculate matrix inverse 
    3. calculate matrix determinant. 
   
The method to use is the modified Gram-Schmidt orthogonalization.

## Tasks

### A. Solving linear equations using QR-decomposition by modified Gram-Schmidt orthogonalization

1. Implement a static (or, at your choice, non-static) class "QR" with functions "decomp", "solve", and "det" (as indicated above). In the non-static class "decomp" becomes a constructor and must be called "QR").  

*I implement a non-static class*

2. The function "decomp" (or the constructor QR) should perform stabilized Gram-Schmidt orthogonalization of an n×m (where n≥m) matrix A and calculate R.

*In my solution, the constructor QR performs this.*

3. The function/method "solve" should use the matrices Q and R from "decomp" and solve the equation QRx=b for the given right-hand-side "b".

4. The function/method "det" should return the determinant of matrix R which is equal to the determinant of the original matrix.  
   (Determinant of a triangular matrix is given by the product of its diagonal elements.)

5. Check that your **"decomp"** works as intended:
- Generate a random tall (n > m) matrix *A* (of a modest size);
- Factorize it into *QR*;
- Check that *R* is upper triangular;
- Check that *QᵀQ = 1*;
- Check that *QR = A*.

6. Check that your **"solve"** works as intended:
- Generate a random square matrix *A* (of a modest size);
- Generate a random vector *b* (of the same size);
- Factorize *A* into *QR*;
- Solve *QRx = b*;
- Check that *Ax = b*.

### B. Matrix inverse by Gram-Schmidt QR factorization

1. Add the function/method "inverse" to your "QR" class that, using the calculated Q and R, should calculate the inverse of the matrix A and returns it.

2. Check that your function works as intended:
- Generate a random square matrix *A* (of a modest size);
- Factorize *A* into *QR*;
- Calculate the inverse *B*;
- Check that *AB = I*, where *I* is the identity matrix.

### C. Operations count for QR-decomposition

1. Measure the time it takes to QR-factorize (with your implementation) a random NxN matrix as function of N. Convince yourself that it goes like O(N³): measure (using the POSIX `time` utility) the time it takes to QR-factorize an N×N matrix for several values of N, plot the time as function of N in gnuplot and fit with N³.
