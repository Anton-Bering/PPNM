# Makefile for Monte Carlo Integration solution

# Executable and source files
TARGET = MonteCarloIntegration.exe
CSFILES = Main.cs PlainMC.cs QuasiRandomMC.cs StratifiedMC.cs \
          UnitCircleCalculator.cs GaussianBellCalculator.cs SpecialIntegralCalculator.cs \
          QuasiVsPseudoComparison.cs StratifiedSamplingComparison.cs LessSungularIntegralCalculator.cs

# Output and plotting
OUTTXT = Out.txt
PLOTSCRIPT = plot.gnu

# List of SVG output plots
SVG_OUTPUTS = \
    Estimate_the_area_of_a_unit_circle_error.svg \
    Estimate_GaussianBell2D_error.svg \
    Estimate_LessSungularIntegral_error.svg \
    Estimate_QMC_vs_MC_error.svg \
    StratifiedSamplingError.svg

# Default target
all: $(OUTTXT) $(SVG_OUTPUTS)

# Compile C# source files
$(TARGET): $(CSFILES)
	mcs -optimize+ -out:$(TARGET) $(CSFILES)

# Run program and write Out.txt
$(OUTTXT): $(TARGET)
	mono $(TARGET) > $(OUTTXT)

# Generate all SVG plots using Gnuplot
$(SVG_OUTPUTS): $(OUTTXT) $(PLOTSCRIPT)
	gnuplot $(PLOTSCRIPT)

# Clean everything except source code
clean:
	rm -f $(TARGET) *.txt *.svg *.exe
