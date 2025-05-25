set terminal pngcairo size 1000,900 enhanced font 'Arial,12'
set output 'plot_B.png'

set multiplot layout 3,1 title "Function and Derivatives with Error (F(x) - g(x))"

# --- Plot 1: g(x) vs F(x) + fejl ---
set title "Function: g(x) and F(x)"
set xlabel "x"
set ylabel "value"
set y2label "error"
set y2tics
set tics nomirror
plot \
  'derivatives_data.txt' using 1:2 with lines dt 2 lw 2 lc rgb "purple" title "g(x)", \
  'derivatives_data.txt' using 1:3 with lines dt 3 lw 2 lc rgb "blue" title "F(x)", \
  'derivatives_data.txt' using 1:($3-$2) axes x1y2 with lines lw 1 dt 2 lc rgb "red" title "Error F(x) - g(x)"

# --- Plot 2: g'(x) vs F'(x) + fejl ---
set title "First Derivative: g'(x) and F'(x)"
plot \
  'derivatives_data.txt' using 1:4 with lines dt 2 lw 2 lc rgb "green" title "g'(x)", \
  'derivatives_data.txt' using 1:5 with lines dt 3 lw 2 lc rgb "cyan" title "F'(x)", \
  'derivatives_data.txt' using 1:($5-$4) axes x1y2 with lines lw 1 dt 2 lc rgb "dark-red" title "Error F' - g'"

# --- Plot 3: g''(x) vs F''(x) + fejl ---
set title "Second Derivative: g''(x) and F''(x)"
plot \
  'derivatives_data.txt' using 1:6 with lines dt 2 lw 2 lc rgb "orange" title "g''(x)", \
  'derivatives_data.txt' using 1:7 with lines dt 3 lw 2 lc rgb "brown" title "F''(x)", \
  'derivatives_data.txt' using 1:($7-$6) axes x1y2 with lines lw 1 dt 2 lc rgb "dark-blue" title "Error F'' - g''"

unset multiplot
