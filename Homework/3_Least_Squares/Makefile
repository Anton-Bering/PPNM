CS      := mcs
CFLAGS  := -target:exe -langversion:latest
EXE     := main.exe
RUN     := mono $(EXE)
OUT     := Out.txt
GNUPLOT := plot.gpi

RAW     := In_Task_A/Rutherford_and_Soddys_ThX.txt
GEN_A   := Rutherford_and_Soddys_ThX_log.txt Fit_the_ThX_data_with_exponential_function.txt
GEN_C   := bestfit_curves.txt
DATA    := $(GEN_A) $(GEN_C)

PLOTS_A := Rutherford_and_Soddys_ThX.svg
PLOTS_C := best_fit_with_changed_coefficients.svg
PLOTS   := $(PLOTS_A) $(PLOTS_C)

SRC        := Main.cs LSFit.cs Utils.cs ../1_Linear_Equations/QR.cs
COMMON_DIR := ../../General
COMMON_SRC := $(COMMON_DIR)/Vector.cs \
              $(COMMON_DIR)/VectorHelpers.cs \
              $(COMMON_DIR)/Matrix.cs \
              $(COMMON_DIR)/MatrixHelpers.cs \
              $(COMMON_DIR)/HW_points.cs
CSFILES    := $(SRC) $(COMMON_SRC)

DIR_A := Out_Task_A
DIR_B := Out_Task_B
DIR_C := Out_Task_C

MOVE_A := $(GEN_A) $(PLOTS_A)
MOVE_B :=
MOVE_C := $(GEN_C) $(PLOTS_C)

.PHONY: all clean package plots dirs

all: package

$(EXE): $(CSFILES)
	$(CS) $(CFLAGS) -out:$@ $^

STAMP := .stamp-run
$(STAMP): $(EXE) $(RAW)
	$(RUN) > $(OUT)
	@touch $@

$(OUT) $(DATA): $(STAMP)

plots: $(DATA) $(GNUPLOT)
	@gnuplot $(GNUPLOT)

dirs:
	@mkdir -p $(DIR_A) $(DIR_B) $(DIR_C)

package: plots dirs
	mv $(MOVE_A) $(DIR_A)/
	@if [ -n "$(MOVE_B)" ]; then mv $(MOVE_B) $(DIR_B)/; fi
	mv $(MOVE_C) $(DIR_C)/

RM ?= rm
clean:
	-$(RM) -f $(EXE) $(OUT) $(DATA) $(PLOTS) $(STAMP)
	-$(RM) -rf $(DIR_A) $(DIR_B) $(DIR_C)