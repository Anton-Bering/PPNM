all: run

run: main.exe
	mono main.exe > Out.txt

main.exe: Main.cs Vector.cs Matrix.cs QRDecomposition.cs RootFind.cs ODESolver.cs
	mcs -out:main.exe Main.cs Vector.cs Matrix.cs QRDecomposition.cs RootFind.cs ODESolver.cs

clean:
	rm -f *.exe *.dll Out.txt

