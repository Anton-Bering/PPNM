set terminal pngcairo size 800,600 enhanced font 'Arial,12'
set output 'radial_wavefunctions.png'

set title "Hydrogen Atom s-wave Functions (Numerical)"
set xlabel "r [Bohr radii]"
set ylabel "f(r)"
set grid
set style data lines
set key outside

plot \
  "radial_n1.txt" using 1:2 title "n=1" lc rgb "blue", \
  "radial_n2.txt" using 1:2 title "n=2" lc rgb "red", \
  "radial_n3.txt" using 1:2 title "n=3" lc rgb "green"
