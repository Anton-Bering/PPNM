# Makefile for building and running the C# least-squares fit solution

CSFLAGS = -r:ScottPlot.dll -r:System.Drawing.Common.dll   # reference ScottPlot and drawing (adjust path if needed)

# If ScottPlot.dll is not in the current directory, you may need to install via NuGet or provide the correct reference path.

all: Out.txt

Out.txt: main.exe
	@# Run the program, redirecting console output to Out.txt
	mono main.exe > Out.txt

main.exe: Main.cs LsFit.cs Utils.cs
	mcs $(CSFLAGS) -out:main.exe Main.cs LsFit.cs Utils.cs

clean:
	rm -f main.exe Out.txt decay_fit.png
