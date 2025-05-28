# Standard-mål: kompiler, mål tid, lav plot og generer Out.txt
all: main.exe timing plot
	@mono main.exe > Out.txt
	@echo "✅ Out.txt, timing.txt og times_plot.png er nu genereret."

# Kompilér C#-kode
main.exe: Main.cs Matrix.cs Vector.cs QR.cs
	mcs -out:main.exe Main.cs Matrix.cs Vector.cs QR.cs

# Del C: Hvor lang tid det tager at QR-faktorisere en NxN matrix
timing: main.exe
	@echo "Generating timing.txt ..."
	@echo "# N    Time (seconds)" > timing.txt
	@for N in $(shell seq 100 20 500); do \
		mono main.exe -size:$$N >> timing.txt; \
	done
	@echo "Done."

# Lav et plot af timing.txt og fit med f(x) = a*x**3
plot: timing.txt plot.gpi
	@gnuplot plot.gpi
	@echo "📈 Plot gemt som times_plot.png"

# Åbn billedet i billedfremviser
open: times_plot.png
	xdg-open times_plot.png

# Ryd op
clean:
	rm -f main.exe Out.txt timing.txt times_plot.png fit.log
