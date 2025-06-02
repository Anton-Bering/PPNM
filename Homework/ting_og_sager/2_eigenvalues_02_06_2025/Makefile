all: main.exe run plot plot_eigenfunctions

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

clean:
	rm -f main.exe Out.txt varying_*.txt varying_*.svg numerically_eigenfunctions_n*.txt analytic_eigenfunctions_n*.txt eigenfunctions.svg

# === Optimize Makefile such that it can run convergence calculations in parallel (TASK C) ===
benchmark.exe: Benchmark.cs Jacobi.cs Matrix.cs Vector.cs
	mcs -optimize -target:exe -out:benchmark.exe Benchmark.cs Jacobi.cs Matrix.cs Vector.cs

parallel_dr:
	@echo "dr   E0" > timing_dr.txt
	@for i in $$(seq 1 100); do \
		dr=$$(awk -v x=$$i 'BEGIN { printf "%.4f", x * 0.01 }'); \
		( mono benchmark.exe -dr $$dr -rmax 10 >> timing_dr.txt ) & \
	done; wait

parallel_rmax:
	@echo "rmax   E0" > timing_rmax.txt
	@for rmax in 5.0 6.0 7.0 8.0 9.0 10.0 11.0 12.0 13.0 14.0; do \
		( time -f "$$rmax %e" -ao timing_times_rmax.txt mono benchmark.exe -dr 0.3 -rmax $$rmax -varyrmax >> timing_rmax.txt ) & \
	done; wait

parallel_all:
	$(MAKE) parallel_dr &
	$(MAKE) parallel_rmax &
	wait
	@echo "All parallel runs complete (dr + rmax ran in parallel)."



# Til sammeligning:
sequential:
	@echo "dr   E0" > timing_seq.txt
	@for dr in 0.02 0.04 0.06 0.08 0.1 0.12 0.14 0.16 0.18 0.2; do \
		time mono benchmark.exe -dr $$dr -rmax 10 >> timing_seq.txt; \
	done

# TASK C.2:
NumberOfOperations.exe: NumberOfOperations.cs Jacobi.cs Matrix.cs Vector.cs
	mcs -optimize -target:exe -out:NumberOfOperations.exe NumberOfOperations.cs Jacobi.cs Matrix.cs Vector.cs

number_of_operations: NumberOfOperations.exe
	@echo "n   seconds" > number_of_operations.txt
	@for n in 30 40 50 60 70 80 90 100 110 120; do \
		( mono NumberOfOperations.exe $$n >> number_of_operations.txt ) & \
	done; wait
	@echo "NumberOfOperations test complete. Results in number_of_operations.txt"


plot_number_of_operations: number_of_operations.svg

number_of_operations.svg: number_of_operations.txt
	gnuplot plot_number_of_operations.gnuplot
