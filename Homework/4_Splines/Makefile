########################################################################
#  Makefile – PPNM Homework 4 “Splines”
#
#  Kørsel:
#     make        → kompilerer, kører Main.cs og genererer alle .svg-plots
#     make run    → kører kun det kompilerede program
#     make clean  → fjerner main.exe, datafiler og plots
#
#  Virker med både dotnet-SDK (csc) og Mono (mcs).
########################################################################

# ----- fælles C#-kilder (ligger to niveauer oppe) -----
GEN = ../../General/general_Vector_and_Matrix.cs \
      ../../General/HW_points.cs

# alle .cs-filer i denne mappe + fælles biblioteker
CSFILES = $(wildcard *.cs) $(GEN)

# ----- C#-kompilator & kørsel -----
CS   := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)
ifndef CS
$(error Ingen C#-kompilator fundet – installer dotnet-sdk eller mono-complete.)
endif

ifeq ($(notdir $(CS)),mcs)        # Mono
  CSTARGET = -target:exe
  RUN      = mono $(EXE)
else                               # dotnet csc
  CSTARGET = -target:exe -langversion:latest
  RUN      = ./$(EXE)
endif

# ----- filnavne -----
EXE       = main.exe
GNUSCRIPT = plot.gnu

# txt-data som Main.cs genererer
DATA = cos.txt quad.txt sin.txt log.txt sqrt.txt \
       Computed_bi_and_ci_sin.txt Computed_bi_and_ci_log.txt

# svg-plots som plot.gnu forventes at producere
PLOTS = cos.svg quad.svg sin.svg log.svg sqrt.svg sqrt_comparing.svg \
        bi_comparison_sin.svg ci_comparison_sin.svg \
        bi_comparison_log.svg ci_comparison_log.svg

# ----- targets -----
.PHONY: all run plots clean

all: $(PLOTS)

# 1) Byg binæren
$(EXE): $(CSFILES)
	$(CS) $(CSTARGET) -out:$@ $(CSFILES)

# 2) Kør programmet → lav .txt-data
$(DATA): $(EXE)
	$(RUN)

# 3) Kør gnuplot én gang – laver alle .svg-plots
$(PLOTS): $(GNUSCRIPT) $(DATA)
	@gnuplot $(GNUSCRIPT)

# Hjælpemål
run: $(EXE)
	$(RUN)

# 4) Ryd op
RM ?= rm
clean:
	-$(RM) -f $(EXE) *.dll $(DATA) $(PLOTS)
