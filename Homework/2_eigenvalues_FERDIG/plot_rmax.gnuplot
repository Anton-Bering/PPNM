set terminal svg size 800,600 enhanced font 'Arial,12' background rgb "white"
set output 'varying_rmax.svg'

set title "Convergence of Ground State Energy with Varying r_{max}"
set xlabel "r_{max}"
set ylabel "ε₀"
set grid
set datafile separator ","
set style data linespoints

plot "varying_rmax.txt" using 1:2 title "E₀ vs r_{max}" lc rgb "dark-red" pt 7 ps 1.5
