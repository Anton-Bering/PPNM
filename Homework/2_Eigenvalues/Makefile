# ---------- Makefile for Homework 2: Eigenvalues (Jacobi-metoden) ----------

# Vælg C#-kompiler
CS := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)
ifndef CS
$(error Ingen C#-kompiler fundet. Installer dotnet-sdk eller mono-complete.)
endif

CSNAME := $(notdir $(CS))

ifeq ($(CSNAME),mcs)              # ---------- Mono ----------
CSTARGET   = -target:exe
RUN_MAIN   = mono main.exe
RUN_TIMING = mono numberops.exe
else                              # ---------- dotnet-csc ----
CSTARGET   = -target:exe -langversion:latest
RUN_MAIN   = ./main.exe
RUN_TIMING = ./numberops.exe
endif

# ---------------------------------------------------------------------------
# Fælles kildefiler (ligger i ../../General/)
COMMON_DIR := ../../General
COMMON_SRC := $(COMMON_DIR)/general_Vector_and_Matrix.cs \
              $(COMMON_DIR)/HW_points.cs

# Lokale kildefiler
CS_COMMON   := Jacobi.cs $(COMMON_SRC)
SRC_MAIN    := Main.cs $(CS_COMMON)
SRC_TIMING  := NumberOfOperations.cs $(CS_COMMON)

# Eksekverbare filer
EXE_MAIN    := main.exe
EXE_TIMING  := numberops.exe

# Uddata-filer
OUT             := Out.txt
TIMING_FILE     := number_of_operations.txt
GNUPLOT_SCRIPT  := plot.gpi
SVG             := eigenfunctions.svg

# ---------------------------------------------------------------------------
.PHONY: all run timing plots clean

all: $(OUT) $(SVG)

# ---------- Kompilér ----------
$(EXE_MAIN): $(SRC_MAIN)
	$(CS) $(CSTARGET) -out:$@ $^

$(EXE_TIMING): $(SRC_TIMING)
	$(CS) $(CSTARGET) -out:$@ $^

# ---------- Kør hovedprogram ----------
$(OUT): $(EXE_MAIN)
	$(RUN_MAIN)

# ---------- Lav timing-data til O(n³)-plottet ----------
$(TIMING_FILE): $(EXE_TIMING)
	@echo "# n   seconds" > $@
	@for n in 100 140 180 220 260 300 ; do \
		$(RUN_TIMING) $$n >> $@ ; \
	done
	@echo "Skrev timing-data til $@"

timing: $(TIMING_FILE)  # praktisk alias

# ---------- Plot ----------
$(SVG): $(GNUPLOT_SCRIPT) $(OUT) $(TIMING_FILE)
	@gnuplot $(GNUPLOT_SCRIPT)

plots: $(SVG)

# ---------- Hjælpe-mål ----------
run: $(EXE_MAIN)
	$(RUN_MAIN) && echo "--------------------------------" && cat $(OUT)

clean:
	-rm -f *.exe *.dll $(OUT) $(SVG) $(TIMING_FILE) \
	       numerically_eigenfunctions_*.txt analytic_eigenfunctions_*.txt \
	       varying_dr.txt varying_rmax.txt
