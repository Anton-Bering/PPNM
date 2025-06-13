# ---------- Makefile ----------
CSFLAGS = -warn:4 -optimize+ -target:exe
SRC     = Main.cs Integrator.cs ClenshawCurtis.cs
EXE     = integrate.exe

.PHONY: all plots clean

# ---------- standard‑mål ---------------------------------------------------
all: Out.txt error_function.txt erf1.txt plots

$(EXE): $(SRC)
	@echo "Compiling C# ..."
	mcs $(CSFLAGS) -out:$@ $(SRC)

Out.txt error_function.txt erf1.txt: $(EXE)
	@echo "Running program ..."
	mono $(EXE)

plots: error_function.svg erf1.svg

# ---------- plot‑regler ----------------------------------------------------
error_function.svg: error_function.txt error_function_tabulated_values.txt plot.gpi
	gnuplot -e "datafile='error_function.txt'; tabfile='error_function_tabulated_values.txt'; outfile='error_function.svg'; loglog=0" plot.gpi

erf1.svg: erf1.txt plot.gpi
	gnuplot -e "datafile='erf1.txt'; outfile='erf1.svg'; loglog=1" plot.gpi

# ---------- oprydning ------------------------------------------------------
#  Bemærk: 'error_function_tabulated_values.txt' bevares!
clean:
	rm -f $(EXE) Out.txt error_function.txt erf1.txt *.svg
