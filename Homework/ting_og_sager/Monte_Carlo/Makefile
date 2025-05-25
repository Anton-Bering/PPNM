# Makefile for Monte Carlo Integration project (C#)
CSC = mcs
RUN = mono

TARGET = MonteCarloIntegration.exe
CSFILES = Vector.cs MonteCarlo.cs Program.cs

$(TARGET): $(CSFILES)
	$(CSC) -target:exe -out:$(TARGET) $(CSFILES)

Out.txt: $(TARGET)
	$(RUN) $(TARGET) > Out.txt

run: $(TARGET)
	$(RUN) $(TARGET)

clean:
	@rm -f $(TARGET) Out.txt
