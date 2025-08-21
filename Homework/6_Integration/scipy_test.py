# scipy_test.py
import numpy as np
from scipy.integrate import quad

# Tæl antal kald
calls_f1 = 0
def f1(x):
    global calls_f1
    calls_f1 += 1
    return 1 / np.sqrt(x)

calls_f2 = 0
def f2(x):
    global calls_f2
    calls_f2 += 1
    return np.log(x) / np.sqrt(x)

# Beregn integralerne
res1, err1 = quad(f1, 0, 1)
res2, err2 = quad(f2, 0, 1)

print("--- Scipy/Numpy Resultater ---")
print(f"∫₀¹ 1/√(x) dx")
print(f"Resultat: {res1}, Kald: {calls_f1}")
print(f"\n∫₀¹ ln(x)/√(x) dx")
print(f"Resultat: {res2}, Kald: {calls_f2}")
