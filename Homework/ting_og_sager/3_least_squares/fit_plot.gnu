set terminal pngcairo size 900,600 enhanced font 'Arial,10'
set output 'fit_plot.png'
set title 'Least-Squares Fit: Exponential Decay'
set xlabel 'Time t [days]'
set ylabel 'Activity y(t)'
set grid
set key top right

set style line 1 lc rgb 'black' pt 7 ps 1.2
set style line 2 lt 1 lc rgb 'blue' lw 2
set style line 3 lt 2 lc rgb 'red' lw 1.5
set style line 4 lt 2 lc rgb 'green' lw 1.5

plot \
  'data.txt' using 1:2:3 with yerrorbars ls 1 title 'Data ±dy', \
  'fit.txt' using 1:2 with lines ls 2 title 'Best Fit', \
  'fit_upper.txt' using 1:2 with lines ls 3 title 'Fit (c+δc)', \
  'fit_lower.txt' using 1:2 with lines ls 4 title 'Fit (c−δc)'
