set terminal svg size 800,600 enhanced font 'Arial,12' background rgb "white"
set output 'number_of_operations.svg'

set title "Jacobi Diagonalization Time vs Matrix Size"
set xlabel "Matrix size n"
set ylabel "Time (seconds)"
set grid
set key top left
set style data linespoints
set datafile separator whitespace

# Fit a cubic function
f(n) = a * n**3
fit f(x) 'number_of_operations.txt' using 1:2 via a

# Add a label with fitted a
set label sprintf("f(n) = %.2e * n^3", a) at graph 0.05, 0.9 font ",12" tc rgb "black"

plot \
  'number_of_operations.txt' using 1:2 title 'Measured time' lc rgb 'blue', \
  f(x) title 'O(n^3) fit' lc rgb 'red' lt 2
