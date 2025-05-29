all: main.exe run plot plot_radials

main.exe: Main.cs Jacobi.cs Matrix.cs Vector.cs
	mcs -optimize -target:exe -out:main.exe Main.cs Jacobi.cs Matrix.cs Vector.cs

run: main.exe
	mono main.exe

# === PLOTS FOR CONVERGENCE ===
plot: varying_dr.png varying_rmax.png

varying_dr.png: varying_dr.txt
	gnuplot plot_dr.gnuplot

varying_rmax.png: varying_rmax.txt
	gnuplot plot_rmax.gnuplot

# === PLOT OF NUMERICAL vs ANALYTICAL WAVEFUNCTIONS ===
plot_radials: radial_wavefunctions.png

radial_wavefunctions.png: analytic_n1.txt analytic_n2.txt analytic_n3.txt radial_n1.txt radial_n2.txt radial_n3.txt
	gnuplot plot_radials.gnuplot

# === Ensure output files exist before plotting ===
analytic_n1.txt analytic_n2.txt analytic_n3.txt: main.exe
	mono main.exe

radial_n1.txt radial_n2.txt radial_n3.txt: main.exe
	mono main.exe

clean:
	rm -f main.exe Out.txt varying_*.txt varying_*.png radial_n*.txt analytic_n*.txt radial_wavefunctions.png

# === Optimize Makefile such that it can run convergence calculations in parallel (TASK C) ===
benchmark.exe: Benchmark.cs Jacobi.cs Matrix.cs Vector.cs
	mcs -optimize -target:exe -out:benchmark.exe Benchmark.cs Jacobi.cs Matrix.cs Vector.cs

parallel_dr:
	@echo "dr   E0" > timing_dr.txt
	@for dr in 0.02 0.04 0.06 0.08 0.1 0.12 0.14 0.16 0.18 0.2; do \
		( time -f "$$dr %e" -ao timing_times_dr.txt mono benchmark.exe -dr $$dr -rmax 10 >> timing_dr.txt ) & \
	done; wait

parallel_rmax:
	@echo "rmax   E0" > timing_rmax.txt
	@for rmax in 5.0 6.0 7.0 8.0 9.0 10.0 11.0 12.0 13.0 14.0; do \
		( time -f "$$rmax %e" -ao timing_times_rmax.txt mono benchmark.exe -dr 0.3 -rmax $$rmax >> timing_rmax.txt ) & \
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

