CS      := mcs
RUN     := mono $(EXE)
EXE     := main.exe
OUT     := Out.txt
GNUPLOT := plot.gpi
DIR_A 	:= Out_Task_A
DIR_B 	:= Out_Task_B
DIR_C 	:= Out_Task_C

DATA	:= sho_data.txt exp_data.txt poly_data.txt \
            lotka_volterra.txt oscillator_with_friction.txt \
            Newtonian_circular_motion.txt Newtonian_elliptical_motion.txt \
            Relativistic_motion.txt three_body_problem.txt

PLOTS   := sho_plot.svg exp_plot.svg poly_plot.svg \
            lotka_volterra_plot.svg oscillator_with_friction_plot.svg \
            Planetary_orbits_plot.svg three_body_problem_plot.svg

# Kildekoden til denne opgave
SRC        := Main.cs OdeSolver.cs RKStepper.cs ODEModels.cs
COMMON_DIR := ../../General
COMMON_SRC := $(COMMON_DIR)/general_Vector_and_Matrix.cs \
		  $(COMMON_DIR)/HW_points.cs

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

CSFILES := $(SRC) $(COMMON_SRC)

.PHONY: all clean

all: $(OUT) $(PLOTS)

$(EXE): $(CSFILES)
	$(CS) -out:$@ $^

# Kør programmet for at generere Out.txt og datatabeller
$(OUT) $(DATA)&: $(EXE) | $(DIR_A) $(DIR_B) $(DIR_C)
	mono $(EXE) > $(OUT)

# Ét kald til gnuplot producerer begge .svg‑filer
$(PLOTS): $(DATA) $(GNUPLOT) | $(DIR_A) $(DIR_B) $(DIR_C)
	@gnuplot $(GNUPLOT)

RM ?= rm

# Opretter mappen, hvis den ikke findes:
$(DIR_A):
	@mkdir -p $(DIR_A)
$(DIR_B):
	@mkdir -p $(DIR_B)
$(DIR_C):
	@mkdir -p $(DIR_C)


clean:
	-$(RM) -f $(EXE)  $(OUT) $(DATA) $(PLOTS)
	-$(RM) -r $(DIR_A) $(DIR_B) $(DIR_C)