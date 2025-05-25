# Makefile for Homework 1 – Linear Equations
# ===========================================
# Usage:
#   make               → build the program (main.exe)
#   make run           → run tests (QR, solve, inverse, determinant)
#   make out.txt       → save output to Out.txt
#   make out.times.data    → generate timing data for QR decomposition (part C)
#   make plot          → generate timing.pdf from out.times.data using gnuplot
#   make open          → open timing.pdf (requires xdg-open)
#   make clean         → remove generated files

CSFILES = main.cs matrix.cs vector.cs QR.cs Extensions.cs
TARGET  = main.exe
CSC     = mcs

$(TARGET): $(CSFILES)
	$(CSC) -target:exe -out:$(TARGET) $(CSFILES)

run: $(TARGET)
	mono $(TARGET)

out.txt: $(TARGET)
	mono $(TARGET) > Out.txt

out.times.data: $(TARGET)
	@ > $@  # truncate the file
	@ for N in $$(seq 100 20 200); do \
		/usr/bin/time -f "$$N %e" --output $@ --append \
		mono $(TARGET) -size:$$N 1> /dev/null 2> /dev/null ; \
	done
	@ echo "Timing data written to $@"

plot: out.times.data
	gnuplot plot.gnu
	@echo "Generated timing.pdf"

open: timing.pdf
	xdg-open timing.pdf

clean:
	rm -f $(TARGET) Out.txt out.times.data timing.pdf fit.log
