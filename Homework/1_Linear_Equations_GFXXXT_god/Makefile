# Standard-mål: kompiler, mål tid, lav plot og generer Out.txt
all: main.exe timing plot
	@mono main.exe > Out.txt
	@echo "✅ Out.txt, QR_factorize_time.txt og QR_factorize_time_plot.svg er nu genereret."

# Kompilér C#-kode
main.exe: Main.cs Matrix.cs Vector.cs QR.cs
	mcs -out:main.exe Main.cs Matrix.cs Vector.cs QR.cs

# Del C: Hvor lang tid det tager at QR-faktorisere en NxN matrix
timing: main.exe
	@echo "Generating QR_factorize_time.txt ..."
	@echo "# N    Time (seconds)" > QR_factorize_time.txt
	@for N in $(shell seq 100 20 500); do \
		mono main.exe -size:$$N >> QR_factorize_time.txt; \
	done
	@echo "Done."

# Lav et plot af QR_factorize_time.txt og fit med f(x) = a*x**3
plot: QR_factorize_time.txt QR_factorize_time_plot.gpi
	@gnuplot QR_factorize_time_plot.gpi
	@echo "📈 Plot gemt som QR_factorize_time_plot.svg"

# Åbn billedet i billedfremviser
open: QR_factorize_time_plot.svg
	xdg-open QR_factorize_time_plot.svg

# Ryd op
clean:
	rm -f main.exe Out.txt QR_factorize_time.txt QR_factorize_time_plot.svg QR_factorize_time_fit.log
