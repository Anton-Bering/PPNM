# Homework "Ordinary least-squares fit"

## Objective

Fit a linear combination `F_c(x)` of given functions `f_k(x)`, to a data set.

## Tasks

### A. Ordinary least-squares fit by QR-decomposition

1. Make sure that your QR-decomposition routines work for tall matrices.

*Here I use my QR decomposition routines (`QR.cs`) from the first homework assignment (Linear Equations).*

2. Implement a routine that makes a least-squares fit — using your QR-decomposition routines — of a given data-set, with a linear combination of given functions.

The routine must calculate and return the vector of the best fit coefficients.

3. Fit the ThX data (from Rutherford and Soddy) with exponential function `y(t)=a exp^{-λt}` in the usual logarithmic way `ln(y)=ln(a)-λt`.

The uncertainty of the logarithm should be taken as `δln(y)=δy/y` (prove it), wher `δy` is the uncertainty of the measurement.

*...*

4. Plot the experimental data (with error-bars) and your best fit.

From your fit find out the half-life time, `T_{1/2} = ln(2)/λ`, of ThX.

Compare your result for ThX (224-Ra) with the modern value.

### B. Uncertainties of the fitting coefficients

1. Modify you least-squares fitting function such that it also calculates the covariance matrix and the uncertainties of the fitting coefficients.

2. Estimate the uncertainty of the half-life value for ThX from the given data.

Does it agree with the modern value within the estimated uncertainty?
  
### C. Evaluation of the quality of the uncertainties on the fit coefficients

1. Plot your best fit, together with the fits where you change the fit coefficients by the estimated uncertainties δc in different combinations
