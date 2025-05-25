set title "QR Factorization Timing"
set xlabel "Matrix Size N"
set ylabel "Time [ms]"
set grid
set key left top

f(x) = a * x**3
fit f(x) 'timing.txt' using 1:($2*1000) via a

set terminal pngcairo enhanced font "Arial,12"
set output 'plot.png'

plot 'timing.txt' using 1:($2*1000) with points title "Timing data", \
     f(x) with lines title sprintf("Fit: %.2e · N³", a)