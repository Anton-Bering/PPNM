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

# === PLOT OF WAVEFUNCTIONS ===
plot_radials: radial_wavefunctions.png

radial_wavefunctions.png: radial_n1.txt radial_n2.txt radial_n3.txt
	gnuplot plot_radials.gnuplot

clean:
	rm -f main.exe Out.txt \
	      varying_*.txt varying_*.png \
	      radial_n*.txt radial_wavefunctions.png
