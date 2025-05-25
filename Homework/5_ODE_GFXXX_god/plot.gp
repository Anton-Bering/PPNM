set terminal png size 800,600
set output 'three_body_problem_plot.png'
set title 'Three-body figure-8 orbit'
set xlabel 'x'
set ylabel 'y'
set grid
plot \
'three_body_problem.txt' using 2:3 with lines title 'Body 1', \
'' using 4:5 with lines title 'Body 2', \
'' using 6:7 with lines title 'Body 3'
