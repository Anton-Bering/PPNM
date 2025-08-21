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

DATA_A	:= sho_data.txt \
		exp_data.txt \
		poly_data.txt \
		oscillator_with_friction.txt \
		lotka_volterra.txt

DATA_B	:= Newtonian_circular_motion.txt \
          Newtonian_elliptical_motion.txt \
          Relativistic_motion.txt

DATA_C := three_body_problem.txt

DATA  := $(DATA_A) $(DATA_B) $(DATA_C)

PLOTS_A := sho_plot.svg \
		  exp_plot.svg \
		  poly_plot.svg \
		  oscillator_with_friction_plot.svg \
		  lotka_volterra_plot.svg

PLOTS_B := Planetary_orbits_plot.svg

PLOTS_C := three_body_problem_plot.svg

PLOTS  := $(PLOTS_A) $(PLOTS_B) $(PLOTS_C)

# 5) Source files
SRC         := Main.cs OdeSolver.cs RKStepper.cs ODEModels.cs
COMMON_DIR  := ../../General
COMMON_SRC  := $(COMMON_DIR)/Vector.cs \
               $(COMMON_DIR)/HW_points.cs
CSFILES     := $(SRC) $(COMMON_SRC)

# 6) Which files belong where
MOVE_A := $(DATA_A) $(PLOTS_A)
MOVE_B := $(DATA_B) $(PLOTS_B)
MOVE_C := $(DATA_C) $(PLOTS_C)


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
package: $(PLOTS) | $(DIR_A) $(DIR_B) $(DIR_C)
	mv $(MOVE_A) $(DIR_A)/
	mv $(MOVE_B) $(DIR_B)/
	mv $(MOVE_C) $(DIR_C)/

# ---------------------------------------------------------------------
# House-keeping
RM ?= rm
clean:
	-$(RM) -f $(EXE) $(OUT) $(DATA) $(PLOTS)
	-$(RM) -rf $(DIR_A) $(DIR_B) $(DIR_C)
