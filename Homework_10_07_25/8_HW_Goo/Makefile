# Navn på den eksekverbare fil
EXE = Main.exe
# C# compiler
CSC = mcs
# Compiler flag
CSFLAGS = -out:$(EXE) -target:exe -r:System.Numerics.dll
# Kildekoder
SRCS = Main.cs Vector.cs Matrix.cs QRGS.cs ODE.cs Roots.cs
# Outputfiler
OUTPUT_TXT = Out.txt
OUTPUT_SVG = wave_function.svg
# Datafiler til plot
PLOT_DATA = wave_function.data exact_wave_function.data

# Standardmål: Byg alt
all: $(OUTPUT_TXT) $(OUTPUT_SVG)

# Regel for at generere output-tekstfilen
$(OUTPUT_TXT): $(EXE)
	mono $< > $@

# Regel for at generere SVG-plotfilen
$(OUTPUT_SVG): $(EXE) plot.gpi
	mono $(EXE) > /dev/null # Kør for at generere datafiler
	gnuplot plot.gpi

# Regel for at bygge den eksekverbare fil
$(EXE): $(SRCS)
	$(CSC) $(CSFLAGS) $(SRCS)

# Ryd op i genererede filer
clean:
	rm -f $(EXE) $(OUTPUT_TXT) $(OUTPUT_SVG) $(PLOT_DATA) *.dll
