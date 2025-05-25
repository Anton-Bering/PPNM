# Makefile to build and run the C# project
PROGRAM = NeuralNetSolver.exe

all: $(PROGRAM) Out.txt plots

$(PROGRAM): Main.cs NeuralNetwork.cs
	mcs -out:$(PROGRAM) Main.cs NeuralNetwork.cs

Out.txt: $(PROGRAM)
	mono $(PROGRAM) > Out.txt

plots: plot.png plot_B.png plot_C.png plot_residual.png plot_cost.png

plot.png: plot_data.txt plot.gp
	gnuplot plot.gp

plot_B.png: derivatives_data.txt plot_B.gp
	gnuplot plot_B.gp

plot_C.png: solution_data.txt plot_C.gp
	gnuplot plot_C.gp

plot_residual.png: residual_data.txt plot_residual.gp
	gnuplot plot_residual.gp

plot_cost.png: cost_data.txt plot_cost.gp
	gnuplot plot_cost.gp

clean:
	rm -f $(PROGRAM) Out.txt *.txt *.png
