## --------- ODE homework · compact Makefile ---------------------------

# 1) Compiler, flags, and runner
CS      := mcs
CFLAGS  := -target:exe -langversion:latest
EXE     := main.exe
RUN     := mono $(EXE)

# 2) External tools
GNUPLOT := plot.gpi
OUT     := Out.txt

# 3) Output folders
DIR_A   := Out_Task_A
DIR_B   := Out_Task_B
DIR_C   := Out_Task_C

# 4) Data and plot files
DATA  := sho_data.txt exp_data.txt poly_data.txt \
         lotka_volterra.txt oscillator_with_friction.txt \
         Newtonian_circular_motion.txt Newtonian_elliptical_motion.txt \
         Relativistic_motion.txt three_body_problem.txt

PLOTS := sho_plot.svg exp_plot.svg poly_plot.svg \
         lotka_volterra_plot.svg oscillator_with_friction_plot.svg \
         Planetary_orbits_plot.svg three_body_problem_plot.svg

# 5) Source files
SRC         := Main.cs OdeSolver.cs RKStepper.cs ODEModels.cs
COMMON_DIR  := ../../General
COMMON_SRC  := $(COMMON_DIR)/general_Vector_and_Matrix.cs \
               $(COMMON_DIR)/HW_points.cs
CSFILES     := $(SRC) $(COMMON_SRC)

# 6) Which files belong where
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

# 7) Top-level targets
.PHONY: all clean package
all: package

# ---------------------------------------------------------------------
# Build
$(EXE): $(CSFILES)
	$(CS) $(CFLAGS) -out:$@ $^

# ---------------------------------------------------------------------
# Run once → generates Out.txt + all *.txt data tables
$(OUT) $(DATA): $(EXE)
	$(RUN) > $(OUT)

# ---------------------------------------------------------------------
# Produce all SVG plots in one call (grouped target, needs GNU make ≥ 4.3)
$(PLOTS)&: $(DATA) $(GNUPLOT)
	@gnuplot $(GNUPLOT)

# ---------------------------------------------------------------------
# Create output folders on demand
$(DIR_A) $(DIR_B) $(DIR_C):
	@mkdir -p $@

# ---------------------------------------------------------------------
# Move results into the three task folders
package: $(DIR_A) $(DIR_B) $(DIR_C) $(PLOTS)
	@echo "Organising output files …"
	@mv $(MOVE_A) $(DIR_A)/
	@mv $(MOVE_B) $(DIR_B)/
	@mv $(MOVE_C) $(DIR_C)/
	@echo "  ✔ $(DIR_A): $(MOVE_A)"
	@echo "  ✔ $(DIR_B): $(MOVE_B)"
	@echo "  ✔ $(DIR_C): $(MOVE_C)"

# ---------------------------------------------------------------------
# House-keeping
RM ?= rm
clean:
	-$(RM) -f $(EXE) $(OUT) $(DATA) $(PLOTS)
	-$(RM) -rf $(DIR_A) $(DIR_B) $(DIR_C)
