set terminal svg size 1000,700 enhanced font 'Arial,12' background rgb "white"
set output 'eigenfunctions.svg'

set title "Hydrogen Atom s-wave Functions (Numerical vs Analytical)"
set xlabel "r [Bohr radii]"
set ylabel "f(r)"
set grid
set key outside
set style data lines
set datafile separator whitespace

plot \
  "numerically_eigenfunctions_n1.txt"    using 1:2 title "Numerical n=1"    lc rgb "blue", \
  "analytic_eigenfunctions_n1.txt"  using 1:2 title "Analytical n=1"   lc rgb "blue" dt 2, \
  "numerically_eigenfunctions_n2.txt"    using 1:2 title "Numerical n=2"    lc rgb "red", \
  "analytic_eigenfunctions_n2.txt"  using 1:2 title "Analytical n=2"   lc rgb "red" dt 2, \
  "numerically_eigenfunctions_n3.txt"    using 1:2 title "Numerical n=3"    lc rgb "green", \
  "analytic_eigenfunctions_n3.txt"  using 1:2 title "Analytical n=3"   lc rgb "green" dt 2
