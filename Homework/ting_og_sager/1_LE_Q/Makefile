# --------------------------------------------
# File names and variables
# --------------------------------------------

# C# source files
CS_FILES = Main.cs Matrix.cs Vector.cs QR.cs

# Name of the compiled executable
TARGET = QRApp.exe

# Output file for program output
OUTPUT = Out.txt

# Benchmark data file (CSV format)
BENCHMARK = benchmark.csv

# Plot image file
PLOT = benchmark_plot.svg

# --------------------------------------------
# Default target: run the program and plot
# --------------------------------------------

all: run plot

# --------------------------------------------
# Compile C# code using Mono C# compiler (mcs)
# --------------------------------------------

$(TARGET): $(CS_FILES)
	mcs -out:$(TARGET) $(CS_FILES)

# --------------------------------------------
# Run the program using Mono and save output
# --------------------------------------------

$(OUTPUT): $(TARGET)
	mono $(TARGET) > $(OUTPUT)

# --------------------------------------------
# Run target = compile and then execute
# --------------------------------------------

run: $(OUTPUT)

# --------------------------------------------
# Generate a benchmark plot using gnuplot
# --------------------------------------------

plot: $(BENCHMARK)
	gnuplot -e "\
	set terminal svg size 800,600 background 'white'; \
	set output '$(PLOT)'; \
	set title 'QR Decomposition Benchmark'; \
	set xlabel 'Matrix Size'; \
	set ylabel 'Time (seconds)'; \
	set grid; \
	f(x) = a*x**3; \
	fit f(x) '$(BENCHMARK)' using 1:2 via a; \
	plot '$(BENCHMARK)' using 1:2 with linespoints title 'Execution Time', \
	f(x) with lines title sprintf('Fit: %.3e x^3', a); \
	"

# --------------------------------------------
# Clean target: remove all generated files
# --------------------------------------------

clean:
	rm -f *.exe *.dll $(OUTPUT) $(BENCHMARK) $(PLOT) fit.log

# --------------------------------------------
# Declare these targets as not real files
# --------------------------------------------

.PHONY: all run plot clean
