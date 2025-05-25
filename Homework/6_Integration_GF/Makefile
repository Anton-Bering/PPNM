# Makefile to build and run the Recursive Adaptive Integration program

all: Out.txt

Program.exe: Main.cs AdaptiveIntegrator.cs
	mcs -optimize+ -out:Program.exe Main.cs AdaptiveIntegrator.cs

Out.txt: Program.exe
	mono Program.exe > Out.txt

clean:
	rm -f Program.exe Out.txt
