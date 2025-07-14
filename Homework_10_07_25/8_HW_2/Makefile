# ---------- RootFinding / Makefile ----------
CSHARP  ?= mcs               # switch to 'csc' if you prefer dotnet
SRC     := Main.cs Vector.cs Matrix.cs Newton.cs Functions.cs Hydrogen.cs
EXE     := root_finding.exe

# Default target --------------------------------------------------------------
all : Out.txt wave_function.svg

# Build, run and post‑process --------------------------------------------------
$(EXE) : $(SRC)
	$(CSHARP) -out:$@ $(SRC) -r:System.Numerics.dll

# Run the program – it creates Out.txt, data files for gnuplot, etc.
Out.txt wave_function.dat analytic_wf.dat : $(EXE)
	mono $(EXE)

# Produce the SVG plot ---------------------------------------------------------
wave_function.svg : plot.gpi wave_function.dat analytic_wf.dat
	gnuplot $<

# House‑keeping ----------------------------------------------------------------
clean :
	rm -f $(EXE) *.txt *.dat *.svg

.PHONY : all clean
# -----------------------------------------------------------------------------#
