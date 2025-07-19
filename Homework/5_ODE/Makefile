## ------------ ODE homework (C# + gnuplot) ----------------------------

# 1) Compiler
CS      := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)
CFLAGS  := -target:exe -langversion:latest
EXE     := main.exe

# 2) Source files
SRC     := Main.cs OdeSolver.cs RKStepper.cs ODEModels.cs
GENERAL := ../../General/general_Vector_and_Matrix.cs \
           ../../General/HW_points.cs
CSFILES := $(SRC) $(GENERAL)

# 3) Data- and plot-files (adjust if you change names in Main.cs / plot.gpi)
DATA_TXT := sho_data.txt exp_data.txt poly_data.txt \
            lotka_volterra.txt oscillator_with_friction.txt \
            Newtonian_circular_motion.txt Newtonian_elliptical_motion.txt \
            Relativistic_motion.txt three_body_problem.txt
PLOTS    := sho_plot.svg exp_plot.svg poly_plot.svg \
            lotka_volterra_plot.svg oscillator_with_friction_plot.svg \
            Planetary_orbits_plot.svg three_body_problem_plot.svg
GNUPLOT  := plot.gpi

# 4) Output folders and what to move where
OUTA := Out_Task_A
OUTB := Out_Task_B
OUTC := Out_Task_C

MOVE_A := sho_data.txt sho_plot.svg \
          exp_data.txt exp_plot.svg \
          poly_data.txt poly_plot.svg \
          oscillator_with_friction.txt oscillator_with_friction_plot.svg \
          lotka_volterra.txt lotka_volterra_plot.svg

MOVE_B := Newtonian_circular_motion.txt \
          Newtonian_elliptical_motion.txt \
          Relativistic_motion.txt \
          Planetary_orbits_plot.svg

MOVE_C := three_body_problem.txt three_body_problem_plot.svg

.PHONY: all data plots package run clean

# ---------------------------------------------------------------------
# Default target: build, generate data, plot, and organise everything
# ---------------------------------------------------------------------
all: $(EXE) data plots package

# ---------------------------------------------------------------------
# Build executable
# ---------------------------------------------------------------------
$(EXE): $(CSFILES)
	$(CS) $(CFLAGS) -out:$@ $(CSFILES)

# ---------------------------------------------------------------------
# Generate data files
# ---------------------------------------------------------------------
data: $(DATA_TXT)

$(DATA_TXT): $(EXE)
	./$(EXE)

# ---------------------------------------------------------------------
# Plot rules
# ---------------------------------------------------------------------
define call_gnuplot
	@gnuplot -e "DATA='$(firstword $^)' ; OUT='$@'" $(GNUPLOT)
endef

sho_plot.svg: sho_data.txt $(GNUPLOT) ; $(call call_gnuplot)
exp_plot.svg: exp_data.txt $(GNUPLOT) ; $(call call_gnuplot)
poly_plot.svg: poly_data.txt $(GNUPLOT) ; $(call call_gnuplot)

lotka_volterra_plot.svg: lotka_volterra.txt $(GNUPLOT) ; $(call call_gnuplot)
oscillator_with_friction_plot.svg: oscillator_with_friction.txt $(GNUPLOT) ; $(call call_gnuplot)

Planetary_orbits_plot.svg: Newtonian_circular_motion.txt Newtonian_elliptical_motion.txt Relativistic_motion.txt $(GNUPLOT)
	@gnuplot -e "DATA1='Newtonian_circular_motion.txt'; DATA2='Newtonian_elliptical_motion.txt'; DATA3='Relativistic_motion.txt'; OUT='$@'" $(GNUPLOT)

three_body_problem_plot.svg: three_body_problem.txt $(GNUPLOT) ; $(call call_gnuplot)

plots: $(PLOTS)

# ---------------------------------------------------------------------
# Packaging: create folders and move outputs
# ---------------------------------------------------------------------
$(OUTA) $(OUTB) $(OUTC):
	@mkdir -p $@

.PHONY: package
package: $(OUTA) $(OUTB) $(OUTC) $(PLOTS)
	@echo "Organising output files ..."
	@mv $(MOVE_A) $(OUTA)/
	@mv $(MOVE_B) $(OUTB)/
	@mv $(MOVE_C) $(OUTC)/
	@echo "  ✔ $(OUTA) contains: $(MOVE_A)"
	@echo "  ✔ $(OUTB) contains: $(MOVE_B)"
	@echo "  ✔ $(OUTC) contains: $(MOVE_C)"

# ---------------------------------------------------------------------
# Convenience targets
# ---------------------------------------------------------------------
run: $(EXE)
	./$(EXE)

clean:
	rm -rf $(EXE) *.dll *.txt $(PLOTS) $(OUTA) $(OUTB) $(OUTC)
