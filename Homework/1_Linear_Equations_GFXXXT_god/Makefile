# Standard-mål: kompiler, mål tid, lav plot og generer Out.txt
all: main.exe times plot
	@mono main.exe > Out.txt
	@echo "✅ Out.txt, times.txt og times_plot.png er nu genereret."

# Kompilér C#-kode
main.exe: Main.cs Matrix.cs Vector.cs QR.cs
	mcs -out:main.exe Main.cs Matrix.cs Vector.cs QR.cs

# Del C: Mål tid for matrixstørrelser fra 200 til 800
times: main.exe
	@echo "Generating times.txt ..."
	@echo "# N    Time (seconds)" > times.txt
	@for N in $(shell seq 200 100 800); do \
		echo -n "$$N    " >> times.txt; \
		/usr/bin/time -f "%e" mono main.exe -size:$$N 1>/dev/null 2>> times.txt; \
	done
	@echo "Done."

# Lav et plot af times.txt og fit med f(x) = a*x**3
plot: times.txt plot.gpi
	@gnuplot plot.gpi
	@echo "📈 Plot gemt som times_plot.png"

# Åbn billedet i billedfremviser
open: times_plot.png
	xdg-open times_plot.png

# Ryd op
clean:
	rm -f main.exe Out.txt times.txt times_plot.png
