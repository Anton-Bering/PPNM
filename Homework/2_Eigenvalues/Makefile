# ---------- Makefile for Homework 2: Eigenvalues (Jacobi-metoden) ----------

# Fælles kildefiler (ligger i ../../General/)
COMMON_DIR := ../../General
COMMON_SRC := $(COMMON_DIR)/Vector.cs \
              $(COMMON_DIR)/VectorHelpers.cs \
              $(COMMON_DIR)/Matrix.cs \
              $(COMMON_DIR)/MatrixHelpers.cs \
              $(COMMON_DIR)/HW_points.cs

# Lokale kildefiler
CS_COMMON   := Jacobi.cs $(COMMON_SRC)
SRC_MAIN    := Main.cs $(CS_COMMON)
SRC_TIMING  := NumberOfOperations.cs $(CS_COMMON)
GNUPLOT_SCRIPT  := plot.gpi

# Eksekverbare filer
EXE_MAIN    := main.exe
EXE_TIMING  := numberops.exe
RUN_MAIN   = mono $(EXE_MAIN)
RUN_TIMING = mono $(EXE_TIMING)

# ---------------------------------------------------------------------------

# Uddata-filer
OUT             := Out.txt
SVG             := number_of_operations.svg  eigenfunctions.svg varying_dr.svg varying_rmax.svg

# Genererede plotdata-filer
TIMING_FILE  := number_of_operations.txt
CONVERGENCE_DR_PLOT_DATA := varying_dr.txt
CONVERGENCE_RMAX_PLOT_DATA := varying_rmax.txt
WAVE_FUNCTION_PLOT_DATA := \
	numerically_eigenfunctions_n1.txt \
	numerically_eigenfunctions_n2.txt \
	numerically_eigenfunctions_n3.txt \
	analytic_eigenfunctions_n1.txt \
	analytic_eigenfunctions_n2.txt \
	analytic_eigenfunctions_n3.txt

MAIN_PLOT_DATA := \
	$(CONVERGENCE_DR_PLOT_DATA) \
	$(CONVERGENCE_RMAX_PLOT_DATA) \
	$(WAVE_FUNCTION_PLOT_DATA)

ALL_PLOT_DATA := 	$(TIMING_FILE) $(MAIN_PLOT_DATA)

# ---------------------------------------------------------------------------

.PHONY: all run timing plots clean
all : $(OUT) $(SVG)

# ---------- Kompilér ----------
$(EXE_MAIN) : $(SRC_MAIN)
	mcs  -out:$@ $^

$(EXE_TIMING): $(SRC_TIMING)
	mcs -out:$@ $^

# ---------- Kør hovedprogram ----------
$(OUT) $(MAIN_PLOT_DATA) : $(EXE_MAIN)
	$(RUN_MAIN) > $@ 

# ---------- Lav timing-data til O(n³)-plottet ----------

#$(TIMING_FILE) : $(EXE_TIMING)
#	@echo "# n   seconds" > $@
#	@for n in 100 140 180 220 260 300 ; do \
#		$(RUN_TIMING) $$n >> $@ ; \
	done
#	@echo "Skrev timing-data til $@"

# Lavet om så det køre i parlel (TASK C:)
Ns := $(shell seq 50 5 300)
$(TIMING_FILE): $(EXE_TIMING)
	@echo "# n   seconds" > $@
	@for N in $(Ns); do \
		time -o $@.$$N -f "$$N %e" $(RUN_TIMING) $$N  & \
	done; \
	wait
	>$@
	@for N in $(Ns); do \
		cat $@.$$N >> $@; \
		rm -f $@.$$N; \
	done	
	@echo "Skrev timing-data til $@"

timing: $(TIMING_FILE)  # praktisk alias
 :  $(EXE_MAIN)

# ---------- Plot ----------
$(SVG) : $(GNUPLOT_SCRIPT) $(ALL_PLOT_DATA)
	@gnuplot $(GNUPLOT_SCRIPT)
plots: $(SVG)

clean:
	-rm -f $(EXE_MAIN) $(EXE_TIMING) $(OUT) $(SVG) $(ALL_PLOT_DATA) fit.log



