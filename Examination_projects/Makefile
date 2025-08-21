CS      := mcs
CFLAGS  := -target:exe -langversion:latest -optimize+
EXE     := main.exe
RUN     := mono $(EXE)
OUT     := Out.txt

# Liste af plot-filer vi vil generere
PLOTS   := higgs_fit.png ann_fit.png

# Liste af datafiler som C# programmet genererer
DATA    := higgs_data.txt higgs_fit_curve.txt ann_approximation.txt

SRC     := Main.cs QuasiNewton.cs Problems.cs \
           ../General/Vector.cs ../General/VectorHelpers.cs \
           ../General/Matrix.cs ../General/MatrixHelpers.cs \
           ../Homework/1_Linear_Equations/QR.cs

.PHONY: all run clean

# 'all' bygger nu både output-tekstfilen OG plots'ne
all: $(OUT) $(PLOTS)

# Regel for at bygge eksekverbare fil
$(EXE): $(SRC)
	$(CS) $(CFLAGS) -out:$(EXE) $(SRC)

# Regel for at køre programmet og generere ALLE output-filer (.txt)
# Denne regel køres kun én gang, selvom flere filer afhænger af den.
$(OUT) $(DATA): $(EXE)
	$(RUN) > $(OUT)

# Regel for at lave Higgs-plottet.
# Afhænger af scriptet og de nødvendige datafiler.
higgs_fit.png: plot_higgs.gpi higgs_data.txt higgs_fit_curve.txt
	gnuplot $<

# Regel for at lave ANN-plottet.
ann_fit.png: plot_ann.gpi ann_approximation.txt
	gnuplot $<

# 'run' kører bare programmet og viser output i terminalen (som før)
run: $(EXE)
	$(RUN)

# 'clean' sletter nu også alle genererede data- og plot-filer.
clean:
	rm -f $(EXE) $(OUT) $(DATA) $(PLOTS)