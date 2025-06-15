set terminal pngcairo size 1000,600 enhanced font 'Arial,12'
set output 'plot_C.png'

set multiplot layout 2,1 title "Solution to Differential Equation: F(x) vs sin(x)"

# --- Plot 1: F(x) vs sin(x) + fejl ---
set title "Function: F(x) and sin(x)"
set xlabel "x"
set ylabel "value"
set y2label "error"
set y2tics
set tics nomirror
plot \
  'solution_data.txt' using 1:2 with lines lw 2 dt 2 lc rgb "purple" title "sin(x)", \
  'solution_data.txt' using 1:3 with lines lw 2 dt 3 lc rgb "blue" title "F(x)", \
  'solution_data.txt' using 1:($3 - $2) axes x1y2 with lines lw 1 dt 2 lc rgb "red" title "Error F(x) - sin(x)"

# --- Plot 2: F'(x) vs cos(x) + fejl ---
set title "First Derivative: F'(x) and cos(x)"
plot \
  'solution_data.txt' using 1:4 with lines lw 2 dt 2 lc rgb "green" title "cos(x)", \
  'solution_data.txt' using 1:5 with lines lw 2 dt 3 lc rgb "cyan" title "F'(x)", \
  'solution_data.txt' using 1:($5 - $4) axes x1y2 with lines lw 1 dt 2 lc rgb "dark-red" title "Error F' - cos(x)"

unset multiplot
