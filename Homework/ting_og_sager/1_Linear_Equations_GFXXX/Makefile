# Standard-mål: kompiler, mål tid, lav plot og generer Out.txt
all: main.exe timing plot
	@mono main.exe > Out.txt
	@echo "✅ Out.txt, timing.txt og timing_plot.png er nu genereret."

# Kompilér C#-kode
main.exe: Main.cs Matrix.cs Vector.cs QR.cs
	mcs -out:main.exe Main.cs Matrix.cs Vector.cs QR.cs

# Del C: Mål tid for matrixstørrelser fra 200 til 800
timing: main.exe
	@echo "Generating timing.txt ..."
	@echo "# N    Time (seconds)" > timing.txt
	@for N in $(shell seq 200 100 800); do \
		echo -n "$$N    " >> timing.txt; \
		/usr/bin/time -f "%e" mono main.exe -size:$$N 1>/dev/null 2>> timing.txt; \
	done
	@echo "Done."

# Lav et plot af timing.txt og fit med f(x) = a*x**3
plot: timing.txt plot.gpi
	@gnuplot plot.gpi
	@echo "📈 Plot gemt som timing_plot.png"

# Åbn billedet i billedfremviser
open: timing_plot.png
	xdg-open timing_plot.png

# Ryd op
clean:
	rm -f main.exe Out.txt timing.txt timing_plot.png
