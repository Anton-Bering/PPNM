# Gnuplot script for Homework Root_Finding

set terminal svg background "white" size 800,1000 font ",10"

# --- Plot 1: Bølgefunktion ---
set output "Hydrogen_wavefunction.svg"
set title "Hydrogen Atom Ground State Wavefunction"
set xlabel "Radius, r (Bohr radii)"
set ylabel "Wavefunction, f(r)"
set grid
f_exact(r) = r*exp(-r)
plot \
"Hydrogen_wavefunction.txt" with lines title "Numerical Solution", \
f_exact(x) with lines dashtype 2 title "Exact Solution f(r) = re^{-r}"

# --- Plot 2: Konvergensanalyse ---
set output "Hydrogen_convergence.svg"
set multiplot layout 2,2 title "Convergence of Ground State Energy E_0"

set xlabel "r_{max}"
set ylabel "E_0 - E_{exact}"
set grid
plot "Hydrogen_convergence_rmax.txt" using 1:($2+0.5) with linespoints title "vs r_{max}"

set xlabel "r_{min}"
set logscale x
plot "Hydrogen_convergence_rmin.txt" using 1:($2+0.5) with linespoints title "vs r_{min}"

unset logscale x
set xlabel "ODE accuracy, acc"
set logscale x
plot "Hydrogen_convergence_acc.txt" using 1:($2+0.5) with linespoints title "vs acc"

set xlabel "ODE accuracy, eps"
set logscale x
plot "Hydrogen_convergence_eps.txt" using 1:($2+0.5) with linespoints title "vs eps"

unset multiplot