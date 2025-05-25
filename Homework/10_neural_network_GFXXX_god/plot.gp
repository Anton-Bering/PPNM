set terminal pngcairo size 900,600 enhanced font 'Arial,12'
set output 'plot.png'

set title "Function Approximation and Error"
set xlabel "x"
set ylabel "y"
set y2label "Error: F(x) - g(x)"
set grid

set y2tics
set tics nomirror

set key outside top center horizontal box

plot \
  'plot_data.txt' using 1:3 with lines lw 2 lc rgb "blue" title "F(x) (NN approximation)", \
  'plot_data.txt' using 1:2 with lines dt 2 lw 2 lc rgb "purple" title "g(x) = cos(5x - 1) * exp(-x^2)", \
  'plot_data.txt' using 1:($3 - $2) axes x1y2 with lines lw 1 dt 2 lc rgb "red" title "Error (F(x) - g(x))"
