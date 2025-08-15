## --------- Makefile: Homework 4 Splines ---------------------------

CS      := mcs
CFLAGS  := -target:exe -langversion:latest
EXE     := main.exe
RUN     := mono $(EXE)
GNUPLOT := plot.gnu
OUT     := Out.txt

DIR_A   := Out_Task_A
DIR_B   := Out_Task_B
DIR_C   := Out_Task_C

DATA_A	:= cos.txt \
	        quad.txt

DATA_B	:= sin.txt \
	        log.txt \
		Computed_bi_and_ci_sin.txt \
		Computed_bi_and_ci_log.txt \

DATA_C := sqrt.txt

DATA  := $(DATA_A) $(DATA_B) $(DATA_C)

PLOTS_A := cos.svg \
		quad.svg		

PLOTS_B := sin.svg \
		log.svg	\
		bi_comparison_sin.svg \
		ci_comparison_sin.svg \
		bi_comparison_log.svg \
		ci_comparison_log.svg \

PLOTS_C := sqrt.svg \
		sqrt_comparing.svg

PLOTS  := $(PLOTS_A) $(PLOTS_B) $(PLOTS_C)

SRC         := Main.cs CubicSpline.cs QuadraticSpline.cs LinearSpline.cs
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