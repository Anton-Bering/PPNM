## Makefile – Ordinary Least‑Squares Fit ##

CS       := mcs
RUN     = mono $(EXE)
EXE     := main.exe
OUT     := Out.txt
GNUPLOT := plot.gpi

# Filer, som læses af hovedprogrammet
INDATA := Rutherford_and_Soddys_ThX.txt

# Filer, som hovedprogrammet genererer under kørslen og som læses af gnuplot
DATA    :=  \
          Rutherford_and_Soddys_ThX_log.txt \
          bestfit_curves.txt

# Plot‑filer produceret af plot.gpi
PLOTS   := Rutherford_and_Soddys_ThX.svg \
          best_fit_with_changed_coefficients.svg

# Kildekoden til denne opgave
SRC        := Main.cs LSFit.cs Utils.cs
COMMON_DIR := ../../General
COMMON_SRC := $(COMMON_DIR)/general_Vector_and_Matrix.cs \
		  $(COMMON_DIR)/HW_points.cs \
		  ../1_Linear_Equations/QR.cs

CSFILES := $(SRC) $(COMMON_SRC)

.PHONY: all clean

all: $(OUT) $(PLOTS)

$(EXE): $(CSFILES)
	$(CS) -out:$@ $^

# Kør programmet for at generere Out.txt og datatabeller
$(OUT) $(DATA) &: $(EXE)
	$(RUN) > $(OUT)

# Ét kald til gnuplot producerer begge .svg‑filer
$(PLOTS) &: $(DATA) $(GNUPLOT)
	@gnuplot $(GNUPLOT)

RM ?= rm

clean:
	-$(RM) -f $(EXE)  $(OUT) $(DATA) $(PLOTS) fit.log Fit_the_ThX_data_with_exponential_function.txt