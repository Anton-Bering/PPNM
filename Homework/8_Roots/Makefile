## --------- Makefile: Homework Root_Finding ---------------------------

CS      := mcs
CFLAGS  := -target:exe -langversion:latest
EXE     := main.exe

RUN     := mono $(EXE)

GNUPLOT := plot.gnu
OUT     := Out.txt

# Output mapper
DIR_A   := Out_Task_A
DIR_B   := Out_Task_B
DIR_C   := Out_Task_C

# Datafiler for Opgave A
DATA_A	:= Rosenbrock_minimum.txt \
		   Himmelblau_minimum.txt

# Datafiler for Opgave B
DATA_B	:= Hydrogen_energy.txt \
		   Hydrogen_wavefunction.txt \
		   Hydrogen_convergence_rmax.txt \
		   Hydrogen_convergence_rmin.txt \
		   Hydrogen_convergence_acc.txt \
		   Hydrogen_convergence_eps.txt

# Datafiler for Opgave C
DATA_C  := Himmelblau_minimum_optimized.txt

# Samlet liste over datafiler
DATA  := $(DATA_A) $(DATA_B) $(DATA_C)

# Plots for Opgave B
PLOTS_B := Hydrogen_wavefunction.svg \
		   Hydrogen_convergence.svg

# Samlet liste over plots
PLOTS  := $(PLOTS_B)

# Kildekofefiler
SRC     := Main.cs \
		   Vector.cs \
		   Matrix.cs \
		   QrDecomposition.cs \
		   RootFinder.cs \
		   OdeSolver.cs \
		   Functions.cs

CSFILES := $(SRC)

# Filer der skal flyttes til mapper
MOVE_A := $(DATA_A)
MOVE_B := $(DATA_B) $(PLOTS_B)
MOVE_C := $(DATA_C)

.PHONY: all clean package

all: package

# Kompilering
$(EXE): $(CSFILES)
	@echo ">> Compiling C# source files..."
	$(CS) $(CFLAGS) -out:$@ $^

# Kørsel og datagenerering
$(OUT) $(DATA): $(EXE)
	@echo ">> Running executable to generate data..."
	$(RUN) > $(OUT)

# Plotting
$(PLOTS): $(DATA) $(GNUPLOT)
	@echo ">> Generating plots with gnuplot..."
	@gnuplot $(GNUPLOT)

# Oprettelse af mapper
$(DIR_A) $(DIR_B) $(DIR_C):
	@mkdir -p $@

# Paketering: Flyt filer til deres respektive mapper
package: $(PLOTS) | $(DIR_A) $(DIR_B) $(DIR_C)
	@echo ">> Packaging output files..."
	@mv $(MOVE_A) $(DIR_A)/ 2>/dev/null || true
	@mv $(MOVE_B) $(DIR_B)/ 2>/dev/null || true
	@mv $(MOVE_C) $(DIR_C)/ 2>/dev/null || true
	@echo ">> Done. Output is in directories $(DIR_A), $(DIR_B), $(DIR_C)."

# Oprydning
RM ?= rm
clean:
	@echo ">> Cleaning up generated files..."
	-$(RM) -f $(EXE) $(OUT) $(DATA) $(PLOTS)
	-$(RM) -rf $(DIR_A) $(DIR_B) $(DIR_C)
	@echo ">> Cleanup complete."