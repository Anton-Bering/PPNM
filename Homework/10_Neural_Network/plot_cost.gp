set terminal pngcairo size 800,400 enhanced font 'Arial,12'
set output 'plot_cost_log.png'

set title "Training Cost over Iterations (Log Scale)"
set xlabel "Iteration"
set ylabel "Cost (log scale)"
set grid
set logscale y

plot 'cost_data.txt' using 1:2 with lines lw 2 lc rgb "blue" title "Cost (C)"
