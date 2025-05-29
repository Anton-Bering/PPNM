set terminal pngcairo size 800,600
set datafile separator "\t"

# --- cos(x) plot ---
set output 'cos.svg'
set title "Linear Spline Interpolant of cos(x) and Interpolant's Anti-derivative"
set xlabel "x"
set ylabel "y"
plot 'cos.txt' using 1:2 with lines title 'Linear Spline', \
     '' using 1:3 with lines title 'Integral', \
     cos(x) with lines title 'cos(x)'

# --- sin(x) plot ---
set output 'sin.svg'
set title "Quadratic Spline Interpolant of sin(x) and Interpolant's Anti-derivative"
set xlabel "x"
set ylabel "y"
plot 'sin.txt' using 1:2 with lines title 'Quadratic Spline', \
     '' using 1:3 with lines title 'Integral', \
     '' using 1:4 with lines title 'Derivative', \
     sin(x) with lines title 'sin(x)'

# --- sqrt(x) plot ---
set output 'sqrt.svg'
set title "Cubic Spline Interpolation of sqrt(x)"
set xlabel "x"
set ylabel "y"
plot sqrt(x) with lines title 'sqrt(x)', \
     'sqrt.txt' using 1:2 with lines title 'Cubic Spline', \
     '' using 1:4 with lines title 'Derivative', \
     '' using 1:3 with lines title 'Integral'

# --- sqrt comparison plot ---
set output 'sqrt_comparing.svg'
set title "Comparison of Cubic Spline for sqrt(x)"
set xlabel "x"
set ylabel "y"
plot 'sqrt.txt' using 1:2 with lines title 'My Cubic Spline', \
     'sqrt_data_points_to_Gnuplot.txt' using 1:2 with linespoints smooth csplines title 'Gnuplot Cubic Spline'

# --- b_i comparison ---
set terminal svg size 800,600 enhanced background rgb 'white'
set output 'bi_comparison.svg'
set title "Comparison of b_i values"
set xlabel "i"
set ylabel "b_i"
plot 'Manually_calculate_bi_and_ci.txt' using 1:2 with linespoints title 'Manual b_i', \
     'Computed_bi_and_ci.txt' using 1:2 with linespoints title 'Program b_i'

# --- c_i comparison ---
set output 'ci_comparison.svg'
set title "Comparison of c_i values"
set xlabel "i"
set ylabel "c_i"
plot 'Manually_calculate_bi_and_ci.txt' using 1:3 with linespoints title 'Manual c_i', \
     'Computed_bi_and_ci.txt' using 1:3 with linespoints title 'Program c_i'
