set terminal pdf
set output 'timing.pdf'

set title 'QR Decomposition Runtime vs Matrix Size'
set xlabel 'Matrix size N'
set ylabel 'Time [s]'

set key top left
set grid
set style line 1 lt 1 lw 2 pt 7 ps 1.5 lc rgb 'blue'

# Fit N^3 curve to the data
f(x) = a * x**3
fit f(x) 'timing.txt' using 1:2 via a

plot \
    'timing.txt' using 1:2 with linespoints linestyle 1 title 'Measured QR time', \
    f(x) with lines lt 0 lc rgb 'gray' title sprintf("Fit: %.2e * N^3", a)
