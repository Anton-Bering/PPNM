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
