set terminal pngcairo size 1000,700 enhanced font 'Arial,12'
set output 'radial_wavefunctions.png'

set title "Hydrogen Atom s-wave Functions (Numerical vs Analytical)"
set xlabel "r [Bohr radii]"
set ylabel "f(r)"
set grid
set key outside
set style data lines
set datafile separator whitespace

plot \
  "radial_n1.txt"    using 1:2 title "Numerical n=1"    lc rgb "blue", \
  "analytic_n1.txt"  using 1:2 title "Analytical n=1"   lc rgb "blue" dt 2, \
  "radial_n2.txt"    using 1:2 title "Numerical n=2"    lc rgb "red", \
  "analytic_n2.txt"  using 1:2 title "Analytical n=2"   lc rgb "red" dt 2, \
  "radial_n3.txt"    using 1:2 title "Numerical n=3"    lc rgb "green", \
  "analytic_n3.txt"  using 1:2 title "Analytical n=3"   lc rgb "green" dt 2
