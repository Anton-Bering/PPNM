# Makefile for Monte Carlo Integration solution

TARGET = MonteCarloIntegration.exe
CSFILES = Main.cs PlainMC.cs QuasiRandomMC.cs StratifiedMC.cs UnitCircleCalculator.cs GaussianBellCalculator.cs SpecialIntegralCalculator.cs QuasiVsPseudoComparison.cs StratifiedSamplingComparison.cs
OUTTXT = Out.txt
DATATXT = area_of_a_unit_circle.txt
PLOTCMD = area_of_a_unit_circle_error.plot
PLOTIMG = area_of_a_unit_circle_error.png

# Default target: build everything and plot
all: $(OUTTXT) $(PLOTIMG)

# Compile C# code
$(TARGET): $(CSFILES)
	mcs -optimize+ -out:$(TARGET) $(CSFILES)

# Run program and capture output
$(OUTTXT): $(TARGET)
	mono $(TARGET) > $(OUTTXT)

# Generate plot using gnuplot
$(PLOTIMG): $(DATATXT) $(PLOTCMD)
	gnuplot $(PLOTCMD)

# Clean up build artifacts
clean:
	rm -f $(TARGET) $(OUTTXT) $(DATATXT) $(PLOTIMG)
