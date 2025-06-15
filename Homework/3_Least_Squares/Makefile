###############################################################################
#                               Project settings                              #
###############################################################################

SRC      := Main.cs Matrix.cs QRDecomposition.cs LSFit.cs Utils.cs
EXE      := fit.exe            # binær fil der bliver lavet
OUT      := Out.txt
GNUSCR   := plot.gpi
PLOTS    := Rutherford_and_Soddys_ThX.svg best_fit_with_changed_coefficients.svg

###############################################################################
#                           Vælg C#‑kompiler automatisk                       #
###############################################################################

# Finder først csc (fra dotnet‑sdk). Hvis den ikke findes, bruger vi mcs (Mono)
CS := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)

ifndef CS
$(error Ingen C#‑kompiler fundet. Installer enten dotnet‑sdk eller mono‑complete.)
endif

# --- compile‑ og run‑indstillinger afhænger af, hvilken compiler vi fandt ----
ifeq ($(notdir $(CS)),mcs)                # -------- Mono / mcs --------------
  # prøv at finde System.ValueTuple.dll
  VALUE_TUPLE := $(shell find /usr/lib/mono -name System.ValueTuple.dll 2>/dev/null | head -n1)
  ifneq ($(VALUE_TUPLE),)
    REF := -r:$(VALUE_TUPLE)
  else
    $(warning System.ValueTuple.dll blev ikke fundet – installer pakken libmono-system-valuetuple4.0-cil)
    REF :=
  endif
  CFLAGS := $(REF) -warn:4 -optimize+ -out:$(EXE)
  RUN    := mono $(EXE)
else                                      # -------- .NET / csc --------------
  CFLAGS := -langversion:latest -optimize+ -out:$(EXE)
  RUN    := ./$(EXE)
endif

###############################################################################
#                               Make‑regler                                   #
###############################################################################

# Standard‑target – lav alt
all: $(OUT) $(PLOTS)

# Kompilér kildekoden
$(EXE): $(SRC)
	$(CS) $(CFLAGS) $(SRC)

# Kør programmet, det skriver Out.txt + hjælpedatafiler til plots
$(OUT): $(EXE)
	$(RUN) > /dev/null

# Lav plots efter at data‑filerne er genereret
$(PLOTS): $(OUT) $(GNUSCR)
	gnuplot $(GNUSCR)

# Ryd op
clean:
	rm -f $(EXE) $(OUT) $(PLOTS) *.txt *.svg *.exe *.o

.PHONY: all clean
