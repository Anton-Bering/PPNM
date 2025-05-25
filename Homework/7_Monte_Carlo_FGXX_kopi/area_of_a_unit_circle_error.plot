set terminal pngcairo size 800,600
set output 'Estimate_the_area_of_a_unit_circle_error.png'

set title 'Monte Carlo Error Scaling (Unit Circle Area)'
set xlabel 'N (Number of samples)'
set ylabel 'Error'
set logscale x
set logscale y
set grid
set key left top

plot \
    'Estimate_the_area_of_a_unit_circle.txt' using 1:2 with linespoints title 'Estimated error', \
    '' using 1:3 with linespoints title 'Actual error', \
    '' using 1:(1/sqrt($1)) with lines title '1/sqrt(N)'
