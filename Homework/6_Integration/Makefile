## --------- Homework Integration ---------------------------

CS      := mcs
CFLAGS  := -target:exe -langversion:latest
EXE     := main.exe
RUN     := mono $(EXE)
GNUPLOT := plot.gpi
OUT     := Out.txt

DIR_A   := Out_Task_A

DATA_A	:= error_function.txt \
		   erf1.txt

DATA  := $(DATA_A)

PLOTS_A := error_function.svg \
		   erf1.svg

PLOTS  := $(PLOTS_A)

SRC         := ClenshawCurtis.cs  Integrator.cs  Main.cs
COMMON_DIR  := ../../General
COMMON_SRC  := $(COMMON_DIR)/HW_points.cs
CSFILES     := $(SRC) $(COMMON_SRC)

MOVE_A := $(DATA_A) $(PLOTS_A)

.PHONY: all clean package
all: package

$(EXE): $(CSFILES)
	$(CS) $(CFLAGS) -out:$@ $^

$(OUT) $(DATA): $(EXE)
	$(RUN) > $(OUT)

$(PLOTS)&: $(DATA) $(GNUPLOT)
	@gnuplot $(GNUPLOT)

$(DIR_A):
	@mkdir -p $@

package: $(PLOTS) | $(DIR_A)
	mv $(MOVE_A) $(DIR_A)

RM ?= rm
clean:
	-$(RM) -f $(EXE) $(OUT) $(DATA) $(PLOTS)
	-$(RM) -rf $(DIR_A)