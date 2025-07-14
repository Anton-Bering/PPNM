## ------------ ODE homework (C# + gnuplot) ----------------------------

# 1) Kompiler
CS      := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)
CFLAGS  := -target:exe -langversion:latest
EXE     := main.exe

# 2) Kilde­filer
SRC     := Main.cs OdeSolver.cs RKStepper.cs ODEModels.cs
GENERAL := ../../General/general_Vector_and_Matrix.cs \
	../../General/HW_points.cs
CSFILES := $(SRC) $(GENERAL)

# 3) Data- og plot-filer (tilpas hvis du ændrer navne i Main.cs / plot.gpi)
DATA_TXT := sho_data.txt exp_data.txt poly_data.txt \
            lotka_volterra.txt oscillator_with_friction.txt \
            Newtonian_circular_motion.txt Newtonian_elliptical_motion.txt \
            Relativistic_motion.txt three_body_problem.txt
PLOTS    := sho_plot.svg exp_plot.svg poly_plot.svg \
            lotka_volterra_plot.svg oscillator_with_friction_plot.svg \
            Planetary_orbits_plot.svg three_body_problem_plot.svg
GNUPLOT  := plot.gpi

.PHONY: all run plots clean

# 4) Standard-mål
all: $(EXE) $(DATA_TXT) $(PLOTS)

$(EXE): $(CSFILES)
	$(CS) $(CFLAGS) -out:$@ $(CSFILES)

# Programmet skriver både Out.txt og alle *.txt datafiler
$(DATA_TXT): $(EXE)
	./$(EXE)

# 5) Plots
define call_gnuplot
	@gnuplot -e "DATA='$(firstword $^)' ; OUT='$@'" $(GNUPLOT)
endef

sho_plot.svg: sho_data.txt  $(GNUPLOT) ; $(call call_gnuplot)
exp_plot.svg: exp_data.txt  $(GNUPLOT) ; $(call call_gnuplot)
poly_plot.svg: poly_data.txt $(GNUPLOT) ; $(call call_gnuplot)

lotka_volterra_plot.svg: lotka_volterra.txt $(GNUPLOT) ; $(call call_gnuplot)
oscillator_with_friction_plot.svg: oscillator_with_friction.txt $(GNUPLOT) ; $(call call_gnuplot)

Planetary_orbits_plot.svg: Newtonian_circular_motion.txt Newtonian_elliptical_motion.txt Relativistic_motion.txt $(GNUPLOT)
	@gnuplot -e "DATA1='Newtonian_circular_motion.txt'; DATA2='Newtonian_elliptical_motion.txt'; DATA3='Relativistic_motion.txt'; OUT='$@'" $(GNUPLOT)

three_body_problem_plot.svg: three_body_problem.txt $(GNUPLOT) ; $(call call_gnuplot)

plots: $(PLOTS)

run: $(EXE)
	./$(EXE)

clean:
	rm -f $(EXE) *.dll *.txt $(PLOTS)
