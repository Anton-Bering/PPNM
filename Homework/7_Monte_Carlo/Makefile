## --------- Makefile: Homework Monte_Carlo ---------------------------

CS      := mcs
CFLAGS  := -target:exe -langversion:latest
EXE     := main.exe

RUN     := mono $(EXE)

GNUPLOT := plot.gnu
OUT     := Out.txt

DIR_A   := Out_Task_A
DIR_B   := Out_Task_B
DIR_C   := Out_Task_C

DATA_A	:= Estimate_the_area_of_a_unit_circle.txt \
			Estimate_GaussianBell2D.txt \
			Estimate_SpecialIntegral.txt \
			Estimate_LessSungularIntegral.txt

DATA_B	:= Estimate_QMC_vs_MC.txt \
			QuasiVsPseudoResults.txt

DATA_C := StratifiedSamplingResults.txt \
			StratifiedSamplingErrors.txt

DATA  := $(DATA_A) $(DATA_B) $(DATA_C)

PLOTS_A := Estimate_the_area_of_a_unit_circle_error.svg \
			Estimate_GaussianBell2D_error.svg \
			Estimate_LessSungularIntegral_error.svg

PLOTS_B := Estimate_QMC_vs_MC_error.svg

PLOTS_C := StratifiedSamplingError.svg

PLOTS  := $(PLOTS_A) $(PLOTS_B) $(PLOTS_C)

SRC         := Main.cs \
				QuasiRandomMC.cs \
				StratifiedSamplingComparison.cs \
				PlainMC.cs \
				QuasiVsPseudoComparison.cs \
				StratifiedMC.cs \
				GenericMCCalculator.cs

COMMON_DIR  := ../../General
COMMON_SRC  := $(COMMON_DIR)/HW_points.cs

CSFILES     := $(SRC) $(COMMON_SRC)

MOVE_A := $(DATA_A) $(PLOTS_A)
MOVE_B := $(DATA_B) $(PLOTS_B)
MOVE_C := $(DATA_C) $(PLOTS_C)

.PHONY: all clean package
all: package

$(EXE): $(CSFILES)
	$(CS) $(CFLAGS) -out:$@ $^

$(OUT) $(DATA): $(EXE)
	$(RUN) > $(OUT)

$(PLOTS)&: $(DATA) $(GNUPLOT)
	@gnuplot $(GNUPLOT)

$(DIR_A) $(DIR_B) $(DIR_C):
	@mkdir -p $@

package: $(PLOTS) | $(DIR_A) $(DIR_B) $(DIR_C)
	mv $(MOVE_A) $(DIR_A)/
	mv $(MOVE_B) $(DIR_B)/
	mv $(MOVE_C) $(DIR_C)/

RM ?= rm
clean:
	-$(RM) -f $(EXE) $(OUT) $(DATA) $(PLOTS)
	-$(RM) -rf $(DIR_A) $(DIR_B) $(DIR_C)
