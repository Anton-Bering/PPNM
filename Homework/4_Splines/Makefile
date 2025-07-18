## Makefile – PPNM Homework 4 Splines ##

CS      := mcs
RUN     := mono $(EXE)
EXE     := main.exe
OUT     := Out.txt
GNUPLOT := plot.gnu
DIR_A 	:= Out_Task_A
DIR_B 	:= Out_Task_B
# I de tidliger homework opgaver har jeg bruge plot.gpi

# Filer, som læses af hovedprogrammet
# INDATA := XXX.txt
# Bruger ikke for denne homework opgave

# Filer, som hovedprogrammet genererer under kørslen og som læses af gnuplot
DATA	:= $(DIR_A)/cos.txt $(DIR_A)quad.txt $(DIR_B)sin.txt $(DIR_B)log.txt sqrt.txt \
       Computed_bi_and_ci_sin.txt Computed_bi_and_ci_log.txt

# Plot‑filer produceret af plot.gpi
PLOTS   := $(DIR_A)/cos.svg $(DIR_A)quad.svg $(DIR_B)sin.svg $(DIR_B)log.svg sqrt.svg sqrt_comparing.svg \
        bi_comparison_sin.svg ci_comparison_sin.svg \
        bi_comparison_log.svg ci_comparison_log.svg

# Kildekoden til denne opgave
SRC        := Main.cs CubicSpline.cs QuadraticSpline.cs LinearSpline.cs
COMMON_DIR := ../../General
COMMON_SRC := $(COMMON_DIR)/general_Vector_and_Matrix.cs \
		  $(COMMON_DIR)/HW_points.cs

CSFILES := $(SRC) $(COMMON_SRC)

.PHONY: all clean

all: $(OUT) $(PLOTS)

$(EXE): $(CSFILES)
	$(CS) -out:$@ $^

# Kør programmet for at generere Out.txt og datatabeller
$(OUT) $(DATA): $(EXE) $(DIR_A) $(DIR_B)
	mono $(EXE) > $(OUT)

# Ét kald til gnuplot producerer begge .svg‑filer
$(PLOTS): $(DATA) $(GNUPLOT) | $(DIR_A)
	@gnuplot $(GNUPLOT)

RM ?= rm

# Opretter mappen, hvis den ikke findes:
$(DIR_A):
	@mkdir -p $(DIR_A)
$(DIR_B):
	@mkdir -p $(DIR_B)


clean:
	-$(RM) -f $(EXE)  $(OUT) $(DATA) $(PLOTS)
	-$(RM) -r $(DIR_A) $(DIR_B)

