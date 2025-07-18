## Makefile – Ordinary Least‑Squares Fit ##

CS := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)
ifndef CS
$(error Ingen C#‑kompiler fundet. Installer dotnet‑sdk eller mono‑complete.)
endif

ifeq ($(notdir $(CS)),mcs)
    RUN     = mono $(EXE)
    CSTARGET= -target:exe
else
    RUN     = ./$(EXE)
    CSTARGET= -target:exe -langversion:latest
endif

EXE     := main.exe
OUT     := Out.txt
GNUPLOT := plot.gpi

# Filer, som hovedprogrammet genererer under kørslen
# (Fjernet: Rutherford_and_Soddys_ThX.txt her fra:)
DATA    := Rutherford_and_Soddys_ThX_log.txt \
          bestfit_curves.txt

# Plot‑filer produceret af plot.gpi
PLOTS   := Rutherford_and_Soddys_ThX.svg \
          best_fit_with_changed_coefficients.svg

# Kildekoden til denne opgave
SRC     := Main.cs LSFit.cs QRDecomposition.cs Utils.cs
GENERAL := ../../General/general_Vector_and_Matrix.cs \
		  ../../General/HW_points.cs

CSFILES := $(SRC) $(GENERAL)

.PHONY: all run plots clean

all: $(OUT) $(PLOTS)

$(EXE): $(CSFILES)
	$(CS) $(CSTARGET) -out:$@ $(CSFILES)

# Kør programmet for at generere Out.txt og datatabeller
$(OUT) $(DATA): $(EXE)
	$(RUN) > $(OUT)


# Ét kald til gnuplot producerer begge .svg‑filer
$(PLOTS): $(DATA) $(GNUPLOT)
	@gnuplot $(GNUPLOT)

run: $(EXE)
	$(RUN)
	@echo "------------------------------------------------"
	@cat $(OUT)

RM ?= rm

clean:
	-$(RM) -f $(EXE) *.dll $(OUT) $(DATA) $(PLOTS)
