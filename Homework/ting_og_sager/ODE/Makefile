# Makefile for ODE homework solution
CSFILES = Vector.cs GenList.cs ODE.cs Program.cs
EXE = ode_solution.exe

all: $(EXE)

$(EXE): $(CSFILES)
	mcs -optimize+ -out:$(EXE) $(CSFILES)

run: $(EXE)
	mono $(EXE)

clean:
	rm -f $(EXE) *.dat
