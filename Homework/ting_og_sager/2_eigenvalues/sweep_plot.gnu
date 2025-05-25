set terminal pngcairo size 900,600 enhanced font 'Arial,10'
set output 'sweeps.png'
set title 'Number of Jacobi Sweeps vs Matrix Size N'
set xlabel 'Matrix size N'
set ylabel 'Number of sweeps'
set grid
set key top left
plot 'sweeps.data' using 1:2 with linespoints title 'Jacobi sweeps'
