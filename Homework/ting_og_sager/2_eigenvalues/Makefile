CSFILES = $(wildcard *.cs)
TARGET  = main.exe

$(TARGET): $(CSFILES)
	@mcs -target:exe -out:$(TARGET) $(CSFILES)
	@echo "Compiled -> $(TARGET)"

.PHONY: run clean sweeps.data sweepplot

run: $(TARGET)
	@mono $(TARGET)

# Generate sweep data for different matrix sizes
sweeps.data: $(TARGET)
	@echo "# N sweeps" > sweeps.data
	@for N in 5 10 15 20 25 30 35 40 45 50; do \
		mono $(TARGET) -size:$$N >> sweeps.data ;\
	done
	@echo "Written to sweeps.data"

# Generate sweep plot using Gnuplot
sweepplot: sweeps.data
	@gnuplot sweep_plot.gnu
	@echo "Generated sweeps.png"

clean:
	@rm -f $(TARGET) sweeps.data sweeps.png
