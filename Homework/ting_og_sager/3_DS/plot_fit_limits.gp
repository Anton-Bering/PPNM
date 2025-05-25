set terminal pngcairo enhanced font "Arial,12"
set output 'activity_fit_limits.png'
set title "Activity vs Time"
set xlabel "Time (days)"
set ylabel "Activity"
set grid
set yrange [0:150]

plot "fit.txt" using 1:2 with lines title "Bedste fit" lc rgb "red" lw 2, \
     "fit_limits.txt" using 1:2:3 with filledcurves title "Usikkerhed" fc rgb "orange" fs transparent solid 0.3
