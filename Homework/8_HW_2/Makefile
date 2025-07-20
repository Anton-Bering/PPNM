## ─────────────────── Makefile ───────────────────
# Vælger først en C#-kompiler (dotnet csc eller mono mcs)
CS := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)
ifndef CS
$(error Ingen C#-kompiler fundet. Installér dotnet-sdk eller mono-complete)
endif

ifeq ($(notdir $(CS)),mcs)          # Mono
    RUN      = mono $(EXE)
    CSTARGET = -target:exe
else                                 # dotnet csc
    RUN      = ./$(EXE)
    CSTARGET = -target:exe -langversion:latest
endif

# ─────────── filnavne ───────────
EXE      := main.exe
OUT      := Out.txt
GNUPLOT  := plot.gpi

DATA     := wave_function.dat analytic_wf.dat
PLOTS    := wave_function.svg

# --- begyndelsen af Makefile ---
GENERAL := ../../General/Vector_and_Matrix.cs \
           ../../General/HW_points.cs
CSFILES  := $(wildcard *.cs) $(GENERAL)

.PHONY: all run clean

## Standard-target: byg alt, kør programmet og lav plot
all: $(OUT) $(PLOTS)

# ───────── kompilering ─────────
$(EXE): $(CSFILES)
	$(CS) $(CSTARGET) -out:$@ $(CSFILES)

# ───────── kørsel ─────────
# Kørsel genererer Out.txt + datamapper
$(OUT) $(DATA): $(EXE)
	$(RUN)

# ───────── plotting ─────────
$(PLOTS): $(DATA) $(GNUPLOT)
	@gnuplot $(GNUPLOT)

# Praktisk alias til hurtig kørsel
run: $(EXE)
	$(RUN)
	@echo "------------------------------------------------"
	@cat $(OUT)

# ───────── oprydning ─────────
RM ?= rm
clean:
	-$(RM) -f $(EXE) *.dll $(OUT) $(DATA) $(PLOTS)
## ────────────────────────────────────────────────
