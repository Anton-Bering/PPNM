set terminal pngcairo size 800,400 enhanced font 'Arial,12'
set output 'plot_residual.png'

set title "Residual: F''(x) + F(x)"
set xlabel "x"
set ylabel "Residual"
set grid
plot 'residual_data.txt' using 1:2 with lines lw 2 lc rgb "red" title "Residual r(x) = F''(x) + F(x)"
