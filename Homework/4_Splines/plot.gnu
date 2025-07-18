set terminal svg size 800,600 enhanced font 'Arial,12' background rgb "white"
set datafile separator "\t"

# --- cos(x) plot ---
set output 'Out_Task_A/cos.svg'
set title "Linear Spline Interpolant of cos(x) and Interpolant's Anti-derivative"
set xlabel "x"
set ylabel "y"
plot 'Out_Task_A/cos.txt' using 1:2 with lines title 'Linear Spline', \
     '' using 1:3 with lines title 'Integral', \
     cos(x) with lines linetype 1 dashtype 2 title 'cos(x)'

# --- x^2 plot ---
set output 'Out_Task_A/quad.svg'
set title "Linear Spline Interpolant of x^2 and Interpolant's Anti-derivative"
set xlabel "x"
set ylabel "y"
plot 'Out_Task_A/quad.txt' using 1:2 with lines title 'Linear Spline', \
     '' using 1:3 with lines title 'Integral', \
     x**2 with lines linetype 1 dashtype 2 title 'x^2'

# --- sin(x) plot ---
set output 'Out_Task_B/sin.svg'
set title "Quadratic Spline Interpolant of sin(x) and Interpolant's Anti-derivative"
set xlabel "x"
set ylabel "y"
plot 'Out_Task_B/sin.txt' using 1:2 with lines title 'Quadratic Spline', \
     '' using 1:3 with lines title 'Integral', \
     '' using 1:4 with lines title 'Derivative', \
     sin(x) with lines linetype 1 dashtype 2 title 'sin(x)'

# --- ln(x+1) quadratic spline plot ---
set output 'Out_Task_B/log.svg'
set title "Quadratic Spline Interpolation and Integral of ln(x+1)"
set xlabel "x"
set ylabel "y"
plot 'Out_Task_B/log.txt' using 1:2 with lines title 'Quadratic Spline', \
     '' using 1:3 with lines title 'Integral', \
     '' using 1:4 with lines title 'Derivative', \
     log(x + 1) with lines linetype 1 dashtype 2 title 'log(x + 1)'


# --- sqrt(x) plot ---
set output 'Out_Task_C/sqrt.svg'
set title "Cubic Spline Interpolation of sqrt(x)"
set xlabel "x"
set ylabel "y"
plot sqrt(x) with lines title 'sqrt(x)', \
     'Out_Task_C/sqrt.txt' using 1:2 with lines title 'Cubic Spline', \
     '' using 1:4 with lines title 'Derivative', \
     '' using 1:3 with lines title 'Integral'

# --- sqrt comparison plot ---
set output 'sqrt_comparing.svg'
set title "Comparison of Cubic Spline for sqrt(x)"
set xlabel "x"
set ylabel "y"
plot 'Out_Task_C/sqrt.txt' using 1:2 with lines title 'My Cubic Spline', \
     'sqrt_data_points_to_Gnuplot.txt' using 1:2 with linespoints smooth csplines title 'Gnuplot Cubic Spline'

# --- b_i comparison for sin(x) ---
set terminal svg size 800,600 enhanced background rgb 'white'
set output 'bi_comparison_sin.svg'
set title "Comparison of b_i values"
set xlabel "i"
set ylabel "b_i"
plot 'Manually_calculate_bi_and_ci_sin.txt' using 1:2 with linespoints title 'Manual b_i', \
     'Computed_bi_and_ci_sin.txt' using 1:2 with linespoints title 'Program b_i'

# --- c_i comparison for sin(x) ---
set output 'ci_comparison_sin.svg'
set title "Comparison of c_i values"
set xlabel "i"
set ylabel "c_i"
plot 'Manually_calculate_bi_and_ci_sin.txt' using 1:3 with linespoints title 'Manual c_i', \
     'Computed_bi_and_ci_sin.txt' using 1:3 with linespoints title 'Program c_i'

# --- b_i comparison for log(x+1) ---
set terminal svg size 800,600 enhanced background rgb 'white'
set output 'bi_comparison_log.svg'
set title "Comparison of b_i values"
set xlabel "i"
set ylabel "b_i"
plot 'Manually_calculate_bi_and_ci_log.txt' using 1:2 with linespoints title 'Manual b_i', \
     'Computed_bi_and_ci_log.txt' using 1:2 with linespoints title 'Program b_i'

# --- c_i comparison for log(x+1) ---
set output 'ci_comparison_log.svg'
set title "Comparison of c_i values"
set xlabel "i"
set ylabel "c_i"
plot 'Manually_calculate_bi_and_ci_log.txt' using 1:3 with linespoints title 'Manual c_i', \
     'Computed_bi_and_ci_log.txt' using 1:3 with linespoints title 'Program c_i'
