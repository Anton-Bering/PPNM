all: main.exe run plot plot_eigenfunctions number_of_operations plot_number_of_operations

main.exe: Main.cs Jacobi.cs Matrix.cs Vector.cs
	mcs -optimize -target:exe -out:main.exe Main.cs Jacobi.cs Matrix.cs Vector.cs

run: main.exe
	mono main.exe

# === PLOTS FOR CONVERGENCE ===
plot: varying_dr.svg varying_rmax.svg

varying_dr.svg: varying_dr.txt
	gnuplot plot_dr.gnuplot

varying_rmax.svg: varying_rmax.txt
	gnuplot plot_rmax.gnuplot

# === PLOT OF NUMERICAL vs ANALYTICAL WAVEFUNCTIONS ===
plot_eigenfunctions: eigenfunctions.svg

eigenfunctions.svg: analytic_eigenfunctions_n1.txt analytic_eigenfunctions_n2.txt analytic_eigenfunctions_n3.txt numerically_eigenfunctions_n1.txt numerically_eigenfunctions_n2.txt numerically_eigenfunctions_n3.txt
	gnuplot plot_eigenfunctions.gnuplot

# === Ensure output files exist before plotting ===
analytic_eigenfunctions_n1.txt analytic_eigenfunctions_n2.txt analytic_eigenfunctions_n3.txt: main.exe
	mono main.exe

numerically_eigenfunctions_n1.txt numerically_eigenfunctions_n2.txt numerically_eigenfunctions_n3.txt: main.exe
	mono main.exe

# === TASK C.2: Check O(n^3) scaling ===
NumberOfOperations.exe: NumberOfOperations.cs Jacobi.cs Matrix.cs Vector.cs
	mcs -optimize -target:exe -out:NumberOfOperations.exe NumberOfOperations.cs Jacobi.cs Matrix.cs Vector.cs

number_of_operations: NumberOfOperations.exe
	@echo "n   seconds" > number_of_operations.txt
	@/usr/bin/time -f "real: %e\nuser: %U\nsys: %S" -o number_of_operations_time.txt sh -c '\
		for n in 30 40 50 60 70 80 90 100 110 120; do \
			mono NumberOfOperations.exe $$n >> number_of_operations.txt; \
		done'
	@echo "NumberOfOperations test complete. Results in number_of_operations.txt"


plot_number_of_operations: number_of_operations.svg

number_of_operations.svg: number_of_operations.txt
	gnuplot plot_number_of_operations.gnuplot

# === CLEAN ===
clean:
	rm -f main.exe Out.txt varying_*.txt varying_*.svg numerically_eigenfunctions_n*.txt analytic_eigenfunctions_n*.txt eigenfunctions.svg number_of_operations.txt number_of_operations.svg NumberOfOperations.exe fit.log
