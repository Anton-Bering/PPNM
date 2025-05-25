set terminal pngcairo enhanced font "Arial,12"
set output 'activity_errorbars.png'
set title "Activity vs time"
set xlabel "Time (days)"
set ylabel "Actyvity"
set grid
set yrange [0:150]

# Plot data med fejlbjælker og fitkurve
plot "data.txt" using 1:2:3 with errorbars title "Data" lc rgb "blue" pt 7 ps 1.5, \
     "fit.txt" using 1:2 with lines title "Bedste fit: y(t) = a e^{-λt}" lc rgb "red" lw 2
