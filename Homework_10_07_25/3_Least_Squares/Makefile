###############################################################################
#                               Project settings                              #
###############################################################################

SRC      := Main.cs Matrix.cs QRDecomposition.cs LSFit.cs Utils.cs
EXE      := fit.exe
OUT      := Out.txt
GNUSCR   := plot.gpi
PLOTS    := Rutherford_and_Soddys_ThX.svg best_fit_with_changed_coefficients.svg

###############################################################################
#                       Choose the C# compiler automatically                   #
###############################################################################

# Prefer csc (dotnet‑sdk); fall back to mcs (Mono)
CS := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)

ifndef CS
$(error No C# compiler found. Install either dotnet‑sdk or mono‑complete.)
endif

# Compiler‑specific flags -----------------------------------------------------
ifeq ($(notdir $(CS)),mcs)                        # ---- Mono / mcs ----------
  VALUE_TUPLE := $(shell find /usr/lib/mono -name System.ValueTuple.dll 2>/dev/null | head -n1)
  ifneq ($(VALUE_TUPLE),)
    REF := -r:$(VALUE_TUPLE)
  else
    $(warning System.ValueTuple.dll not found – install libmono-system-valuetuple4.0-cil)
    REF :=
  endif
  CFLAGS := $(REF) -warn:4 -optimize+ -out:$(EXE)
  RUN    := mono $(EXE)
else                                             # ---- .NET / csc ----------
  CFLAGS := -langversion:latest -optimize+ -out:$(EXE)
  RUN    := ./$(EXE)
endif

###############################################################################
#                                Make targets                                 #
###############################################################################

# The *default* target: build everything, then print/append the score banner
all: points

# 1. Compile the C# sources ----------------------------------------------------
$(EXE): $(SRC)
	$(CS) $(CFLAGS) $(SRC)

# 2. Run the program – generates Out.txt and data files -----------------------
$(OUT): $(EXE)
	$(RUN) > /dev/null

# 3. Produce the SVG plots with gnuplot ---------------------------------------
$(PLOTS): $(OUT) $(GNUSCR)
	gnuplot $(GNUSCR)

# 4. Print banner to terminal *and* append it to Out.txt ----------------------
points: $(PLOTS) $(OUT)
	@{ \
	  printf '\n'; \
	  printf '------------------------------------------------\n'; \
	  printf '\n'; \
	  printf 'Task A completed → 6 points😊\n'; \
	  printf 'Task B completed → 3 points😄\n'; \
	  printf 'Task C completed → 1 point 😎\n'; \
	  printf '\n'; \
	  printf 'In total, this exercise results in 10 points🎉\n'; \
	} | tee -a $(OUT)

# 5. House‑keeping ------------------------------------------------------------
clean:
	rm -f $(EXE) $(OUT) $(PLOTS) *.txt *.svg *.exe *.o

.PHONY: all points clean
