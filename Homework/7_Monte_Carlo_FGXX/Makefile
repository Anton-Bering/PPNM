# Makefile for Monte Carlo Integration solution

TARGET = MonteCarloIntegration.exe

CSFILES = Main.cs PlainMC.cs QuasiRandomMC.cs StratifiedMC.cs

# Default target: build and run to produce Out.txt
all: Out.txt

$(TARGET): $(CSFILES)
	# Compile with mcs (Mono C# compiler) or csc if available
	mcs -optimize+ -out:$(TARGET) $(CSFILES)

Out.txt: $(TARGET)
	# Run the program and save output to Out.txt
	mono $(TARGET) > Out.txt

# Cleanup build artifacts
clean:
	rm -f $(TARGET) Out.txt
