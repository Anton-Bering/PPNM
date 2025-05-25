all: main.exe

main.exe: Main.cs Matrix.cs Vector.cs Jacobi.cs
	mcs -out:main.exe Main.cs Matrix.cs Vector.cs Jacobi.cs

run: main.exe
	mono main.exe -rmax 20 -dr 2.0

clean:
	rm -f main.exe