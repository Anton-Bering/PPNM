PROGRAM := Main.exe
CSFILES := Main.cs NeuralNetwork.cs

.PHONY: all txt plots clean

all: $(PROGRAM) plots

$(PROGRAM): $(CSFILES)
	mcs $(CSFILES) -out:$(PROGRAM)

txt: $(PROGRAM)
	mono $(PROGRAM)              # skriver *.txt + Out.txt

plots: txt plot.gpi
	gnuplot plot.gpi             # laver alle *.svg

clean:
	rm -f $(PROGRAM) *.txt *.svg # sletter også Out.txt mm.
