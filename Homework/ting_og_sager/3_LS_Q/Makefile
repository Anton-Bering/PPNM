# ---- Libraries and Source ---- #
MAIN = main.cs
LIBS = Vector.cs Matrix.cs QRGS.cs LSFit.cs
TARGET = main.exe

# ---- Output Files ---- #
OUTTXT = Out.txt
PLOT = datafit.svg
DATA = datafile.csv

# ---- Default Target ---- #
all: $(OUTTXT) $(PLOT)

# ---- Compile C# code ---- #
$(TARGET): $(MAIN) $(LIBS)
	mcs $(MAIN) $(LIBS) -out:$(TARGET)

# ---- Run program and write output ---- #
$(OUTTXT): $(TARGET)
	mono $(TARGET) > $(OUTTXT)

# ---- Extract values from output ---- #
A = $(shell grep 'a =' $(OUTTXT) | awk '{print $$3}')
B = $(shell awk '/c:/{getline; print $$2}' $(OUTTXT))
DA = $(shell grep 'Uncertainties:' $(OUTTXT) | awk '{print $$2}')
DB = $(shell grep 'Uncertainties:' $(OUTTXT) | awk '{print $$3}')

# ---- Plot with gnuplot ---- #
$(PLOT): $(DATA) $(OUTTXT)
	@echo "A = $(A), B = $(B), DA = $(DA), DB = $(DB)"
	gnuplot -e "\
	set terminal svg size 800,600 background 'white'; \
	set output '$(PLOT)'; \
	set title 'Decay of 224Ra with fit'; \
	set xlabel 't (days)'; \
	set ylabel 'Activity'; \
	f(x) = $(A)*exp(-$(B)*x); \
	fp(x) = ($(A)+$(DA))*exp(-($(B)-$(DB))*x); \
	fm(x) = ($(A)-$(DA))*exp(-($(B)+$(DB))*x); \
	plot '$(DATA)' with yerrorbars title 'data', \
		 f(x) title sprintf('Fit: %.f*exp(-%.2f t)', $(A), $(B)), \
		 fp(x) with lines lt 1 lc 'red' title sprintf('Fit +σ'), \
		 fm(x) with lines lt 1 lc 'green' title sprintf('Fit −σ');"

# ---- Clean ---- #
clean:
	rm -f *.exe *.svg *.txt
