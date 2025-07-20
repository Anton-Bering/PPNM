## Makefile – Roots ##

# ────────────── Vælg C#-kompiler (dotnet sdk eller mono/mcs) ──────────────
CS := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)
ifndef CS
$(error Ingen C#-kompiler fundet. Installer dotnet-sdk eller mono-complete.)
endif

ifeq ($(notdir $(CS)),mcs)
    RUN      = mono $(EXE)
    CSTARGET = -target:exe
else
    RUN      = ./$(EXE)
    CSTARGET = -target:exe -langversion:latest
endif

# ────────────── Filer og afhængigheder ──────────────
EXE      := roots.exe            # navn på den resulterende binær
OUT      := Out.txt              # evt. tekstoutput (fra Main.cs)
GNUPLOT  := plot.gpi             # hvis du laver plots til opgaven

# Lokale kildefiler til “Roots”
SRC := Main.cs \
       ODESolver.cs \
       QRDecomposition.cs \
       RootFind.cs

# Fælles hjælpefiler (tilpas stien om nødvendigt)
GENERAL := ../../General/Vector_and_Matrix.cs \
           ../../General/HW_points.cs

CSFILES := $(SRC) $(GENERAL)

.PHONY: all run clean plots

# ────────────── Standardmål ──────────────
all: $(EXE)

# Kompilér
$(EXE): $(CSFILES)
	$(CS) $(CSTARGET) -out:$@ $(CSFILES)

# Kør programmet og producer Out.txt (hvis Main.cs skriver til fil)
$(OUT): $(EXE)
	$(RUN)

run: $(EXE)
	$(RUN)

# ────────────── Plot (valgfrit – fjern hvis ikke relevant) ──────────────
# Eksempel: roots_plot.svg: roots_data.txt $(GNUPLOT)
# 	@gnuplot -e "set terminal svg background 'white'; \
# 	            set output '$@';                       \
# 	            load '$(GNUPLOT)'"

plots:    # samlet mål hvis du senere tilføjer flere plots
	@echo "Ingen plots defineret endnu."

# ────────────── Ryd op ──────────────
RM ?= rm
clean:
	-$(RM) -f $(EXE) *.dll $(OUT) *.svg *.txt
