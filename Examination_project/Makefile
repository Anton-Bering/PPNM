CS := $(shell command -v csc 2>/dev/null || command -v mcs 2>/dev/null)
ifndef CS
$(error C# compiler not found)
endif

ifeq ($(notdir $(CS)),mcs)
    RUN       = mono $(EXE)
    CSTARGET  = -target:exe
else
    RUN       = ./$(EXE)
    CSTARGET  = -target:exe -langversion:latest
endif

EXE      := main.exe
SRC      := Main.cs
GENERAL  := ../General/general_Vector_and_Matrix.cs
GNUPLOT  := plot.gpi
PLOT_SVG := rosen_path.svg

CSFILES  := $(SRC) $(GENERAL)

.PHONY: all clean run plots

all: $(EXE) run plots

$(EXE): $(CSFILES)
	$(CS) $(CSTARGET) -out:$@ $(CSFILES)

run: $(EXE)
	$(RUN) > Out.txt

plots: $(PLOT_SVG)

$(PLOT_SVG): path_rosenbrock.dat $(GNUPLOT)
	gnuplot $(GNUPLOT)

clean:
	rm -f $(EXE) *.dll Out.txt path_rosenbrock.dat $(PLOT_SVG)
