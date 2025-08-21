CS      = mcs
CSFLAGS = -optimize+ -warn:4
SRC     = Main.cs linalg.cs minimizer.cs functions.cs counter.cs
BIN     = Main.exe

OUTTXT  = Out.txt
FITTXT  = Higgs_fit.txt
PLOTFIG = Higgs_fit_and_experimental_data.svg

.PHONY: all clean run plot

all: $(OUTTXT) $(PLOTFIG)

$(BIN): $(SRC)
	$(CS) $(CSFLAGS) -out:$(BIN) $(SRC)

# Programmet skriver FITTXT selv; stdout gemmes i OUTTXT
$(OUTTXT) $(FITTXT): $(BIN) cross_section_experiment.txt
	@echo " --- Running program to produce $(OUTTXT) and $(FITTXT) ---"
	mono $(BIN) < cross_section_experiment.txt

$(PLOTFIG): plot.gpi $(FITTXT) cross_section_experiment.txt
	@echo " --- Drawing plot with gnuplot ---"
	gnuplot plot.gpi

run: $(OUTTXT)
	@echo " --- See $(OUTTXT) ---"

plot: $(PLOTFIG)

clean:
	rm -f $(BIN) $(OUTTXT) $(FITTXT) $(PLOTFIG) *.svg *.dll
