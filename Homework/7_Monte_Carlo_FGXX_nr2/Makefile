# Makefile for Monte Carlo Integration solution

TARGET = MonteCarloIntegration.exe
CSFILES = Main.cs PlainMC.cs QuasiRandomMC.cs StratifiedMC.cs UnitCircleCalculator.cs GaussianBellCalculator.cs SpecialIntegralCalculator.cs QuasiVsPseudoComparison.cs StratifiedSamplingComparison.cs LessSungularIntegralCalculator.cs LessSungularIntegralCalculator.cs
OUTTXT = Out.txt
PLOTSCRIPT = plot.gnu
SVG_OUTPUTS = Estimate_the_area_of_a_unit_circle_error.svg Estimate_GaussianBell2D_error.svg

# Default target
all: $(OUTTXT) $(SVG_OUTPUTS)

# Compile C# code
$(TARGET): $(CSFILES)
	mcs -optimize+ -out:$(TARGET) $(CSFILES)

# Run program to generate output and data files
$(OUTTXT): $(TARGET)
	mono $(TARGET) > $(OUTTXT)

# Generate both SVG plots using plot.gnu
$(SVG_OUTPUTS): $(OUTTXT) $(PLOTSCRIPT)
	gnuplot $(PLOTSCRIPT)

# Clean up build artifacts
clean:
	rm -f $(TARGET) *.txt *.svg
