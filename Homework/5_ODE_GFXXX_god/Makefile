CC     = mcs
TARGET = main.exe

all: $(TARGET)
	mono $(TARGET)

	# Plot for u'' = -u
	echo "set terminal png size 800,600" > plot.gp
	echo "set output 'sho_plot.png'" >> plot.gp
	echo "set title \"u'' = -u (Numerical vs Analytical)\"" >> plot.gp
	echo "set xlabel 'x'" >> plot.gp
	echo "set ylabel 'u(x)'" >> plot.gp
	echo "set grid" >> plot.gp
	echo "plot 'sho_data.txt' with lines title 'Numerical', sin(x) with lines dashtype 2 title 'Analytical'" >> plot.gp
	gnuplot plot.gp

	# Plot for dy/dx = y
	echo "set terminal png size 800,600" > plot.gp
	echo "set output 'exp_plot.png'" >> plot.gp
	echo "set title \"dy/dx = y (Numerical vs Analytical)\"" >> plot.gp
	echo "set xlabel 'x'" >> plot.gp
	echo "set ylabel 'y(x)'" >> plot.gp
	echo "set grid" >> plot.gp
	echo "plot 'exp_data.txt' with lines title 'Numerical', exp(x) with lines dashtype 2 title 'Analytical'" >> plot.gp
	gnuplot plot.gp

	# Plot for y'' = 6x
	echo "set terminal png size 800,600" > plot.gp
	echo "set output 'poly_plot.png'" >> plot.gp
	echo "set title \"y'' = 6x (Numerical vs Analytical)\"" >> plot.gp
	echo "set xlabel 'x'" >> plot.gp
	echo "set ylabel 'y(x)'" >> plot.gp
	echo "set grid" >> plot.gp
	echo "plot 'poly_data.txt' with lines title 'Numerical', x**3 with lines dashtype 2 title 'Analytical'" >> plot.gp
	gnuplot plot.gp

	# Plot for oscillator with friction
	echo "set terminal png size 800,600" > plot.gp
	echo "set output 'oscillator_with_friction_plot.png'" >> plot.gp
	echo "set title \"Damped harmonic oscillator\"" >> plot.gp
	echo "set xlabel 't'" >> plot.gp
	echo "set ylabel 'y(t)'" >> plot.gp
	echo "set grid" >> plot.gp
	echo "plot 'oscillator_with_friction.txt' using 1:2 with lines title 'y(t)'" >> plot.gp
	gnuplot plot.gp

	# Plot for Lotka-Volterra
	echo "set terminal png size 800,600" > plot.gp
	echo "set output 'lotka_volterra_plot.png'" >> plot.gp
	echo "set title \"Lotka-Volterra system\"" >> plot.gp
	echo "set xlabel 't'" >> plot.gp
	echo "set ylabel 'population'" >> plot.gp
	echo "set grid" >> plot.gp
	echo "plot 'lotka_volterra.txt' using 1:2 with lines title 'Prey', '' using 1:3 with lines title 'Predator'" >> plot.gp
	gnuplot plot.gp

	# Plot for planetary orbits
	# echo "set terminal png size 800,600" > plot.gp
	# echo "set output 'Planetary_orbits_plot.png'" >> plot.gp
	# echo "set title 'Planetary orbits: Newtonian and Relativistic'" >> plot.gp
	# echo "set xlabel 'x'" >> plot.gp
	# echo "set ylabel 'y'" >> plot.gp
	# echo "set grid" >> plot.gp
	# echo "plot \\" >> plot.gp
	# echo "'Newtonian_circular_motion.txt' using (1/\\\$2)*cos(\\\$1):(1/\\\$2)*sin(\\\$1) with lines title 'Circular', \\" >> plot.gp
	# echo "'Newtonian_elliptical_motion.txt' using (1/\\\$2)*cos(\\\$1):(1/\\\$2)*sin(\\\$1) with lines title 'Elliptical', \\" >> plot.gp
	# echo "'Relativistic_motion.txt' using (1/\\\$2)*cos(\\\$1):(1/\\\$2)*sin(\\\$1) with lines title 'Relativistic'" >> plot.gp
	# gnuplot plot.gp

	# Plot for three-body
	echo "set terminal png size 800,600" > plot.gp
	echo "set output 'three_body_problem_plot.png'" >> plot.gp
	echo "set title 'Three-body figure-8 orbit'" >> plot.gp
	echo "set xlabel 'x'" >> plot.gp
	echo "set ylabel 'y'" >> plot.gp
	echo "set grid" >> plot.gp
	echo "plot \\" >> plot.gp
	echo "'three_body_problem.txt' using 2:3 with lines title 'Body 1', \\" >> plot.gp
	echo "'' using 4:5 with lines title 'Body 2', \\" >> plot.gp
	echo "'' using 6:7 with lines title 'Body 3'" >> plot.gp
	gnuplot plot.gp


$(TARGET): *.cs
	$(CC) -target:exe -out:$(TARGET) *.cs

clean:
	rm -f $(TARGET) *.txt *.png plot.gp
