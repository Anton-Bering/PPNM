# --- Plot: Unit Circle Error ---
set terminal svg size 900,600 enhanced font 'Arial,16' background rgb "white"
set output 'Estimate_the_area_of_a_unit_circle_error.svg'

set title 'Error from calculating the area of a unit circle via the MC routine'
set xlabel 'N [Number of samples]'
set ylabel 'Error'
set grid
set key left top

plot \
    'Estimate_the_area_of_a_unit_circle.txt' using 1:2 with linespoints title 'Estimated error', \
    '' using 1:3 with linespoints title 'Actual error', \
    '' using 1:(1/sqrt($1)) with lines title '1/√N'

# --- Plot: Gaussian Bell Error ---
set output 'Estimate_GaussianBell2D_error.svg'
set title 'Monte Carlo Error Scaling (Gaussian Bell Integral)'

plot \
    'Estimate_GaussianBell2D.txt' using 1:2 with linespoints title 'Estimated error', \
    '' using 1:3 with linespoints title 'Actual error', \
    '' using 1:(1/sqrt($1)) with lines title '1/sqrt(N)'


# --- Plot: Less Singular Integral ---
set terminal svg size 800,600 enhanced font 'Verdana,10'
set output 'Estimate_LessSungularIntegral_error.svg'
set title 'Monte Carlo Error Scaling (Less Singular 3D Integral)'
set xlabel 'N (Number of samples)'
set ylabel 'Error'
set logscale x
set logscale y
set grid
set key left top

plot \
    'Estimate_LessSungularIntegral.txt' using 1:2 with linespoints title 'Estimated error', \
    '' using 1:3 with linespoints title 'Actual error', \
    '' using 1:(1/sqrt($1)) with lines title '1/sqrt(N)'

# --- Plot: QMC vs MC error ---
set output 'Estimate_QMC_vs_MC_error.svg'
set title 'Error Scaling: Pseudo-Random vs Quasi-Random Monte Carlo'

plot \
  'Estimate_QMC_vs_MC.txt' using 1:2 with linespoints title 'MC est. error', \
  '' using 1:3 with linespoints title 'MC actual error', \
  '' using 1:4 with linespoints title 'QMC est. error', \
  '' using 1:5 with linespoints title 'QMC actual error', \
  '' using 1:(1/sqrt($1)) with lines title '1/sqrt(N)'

# --- Plot: Stratified Sampling vs Plain MC ---
set output 'StratifiedSamplingError.svg'
set title 'Actual Error: Plain MC vs Stratified Sampling'
set xlabel 'N (Number of samples)'
set ylabel 'Actual Error'
set logscale x
set logscale y
set grid
set key left top

plot \
  'StratifiedSamplingErrors.txt' using 1:2 with linespoints title 'Plain MC', \
  '' using 1:3 with linespoints title 'Stratified MC', \
  '' using 1:(1/sqrt($1)) with lines title '1/sqrt(N)'
