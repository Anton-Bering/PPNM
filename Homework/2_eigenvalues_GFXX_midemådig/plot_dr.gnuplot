set terminal pngcairo size 800,600 enhanced font 'Arial,12'
set output 'varying_dr.png'

set title "Convergence of Ground State Energy with Varying Δr"
set xlabel "Δr"
set ylabel "ε₀"
set grid
set datafile separator ","
set style data linespoints

plot "varying_dr.txt" using 1:2 title "E₀ vs Δr" lc rgb "blue" pt 7 ps 1.5
