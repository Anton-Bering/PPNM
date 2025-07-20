## Makefile – homework 8 (GOO) ##
# ────────────────────────────────────────────────────────────────────────────────
# Bygger C#-koden, kører programmet, genererer data + plot og rydder op.
# Auto-lokaliserer fælles Vector/Matrix-klasse.
# ────────────────────────────────────────────────────────────────────────────────

CS := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)
ifndef CS
$(error Ingen C#‐kompiler fundet. Installér dotnet-sdk eller mono-complete.)
endif

ifeq ($(notdir $(CS)),mcs)            # Mono
    RUN     = mono $(EXE)
    CSTARGET= -target:exe
else                                    # dotnet
    RUN     = ./$(EXE)
    CSTARGET= -target:exe -langversion:latest
endif

#  Eksekverbar og artefakter
EXE     := main.exe
OUT     := Out.txt
DATA    := wave_function.data exact_wave_function.data
PLOTS   := wave_function.svg
GNUPLOT := plot.gpi

#  Find fælles Vector/Matrix-implementering
#  – Søg i denne mappe, forældre­mapper *og* evt. underkataloget "General".
#  – Brug evt. LIBDIR=... for at pege på en bestemt mappe.
GEN_CS_AUTO := $(firstword \
    $(wildcard Vector_and_Matrix.cs) \
    $(wildcard ../Vector_and_Matrix.cs) \
    $(wildcard ../../Vector_and_Matrix.cs) \
    $(wildcard ../../../Vector_and_Matrix.cs) \
    $(wildcard ../General/Vector_and_Matrix.cs) \
    $(wildcard ../../General/Vector_and_Matrix.cs) \
    $(wildcard ../../../General/Vector_and_Matrix.cs))

ifdef LIBDIR
GEN_CS := $(abspath $(LIBDIR))/Vector_and_Matrix.cs
else
GEN_CS := $(GEN_CS_AUTO)
endif

ifeq ($(strip $(GEN_CS)),)
$(error "Vector_and_Matrix.cs" blev ikke fundet. Kopiér filen hertil eller kør `make LIBDIR=/sti/til/mappe`.)
endif

CSFILES := Main.cs ODE.cs QRGS.cs Roots.cs $(GEN_CS)

#  Standard‐mål
.PHONY: all run plots clean

all: $(OUT) $(PLOTS)

$(EXE): $(CSFILES)
	$(CS) $(CSTARGET) -out:$@ $(CSFILES)

#  Kør programmet – gem stdout (Out.txt) og producer datafiler
$(OUT) $(DATA): $(EXE)
	$(RUN) > $(OUT)

#  Generér plot
wave_function.svg: $(DATA) $(GNUPLOT)
	@gnuplot -e "set terminal svg background 'white'; set output '$@'; load '$(GNUPLOT)'"

run: $(EXE)
	$(RUN)

plots: $(PLOTS)

clean:
	@echo "Rydder op…"
	-rm -f $(EXE) *.dll $(OUT) $(DATA) $(PLOTS)
