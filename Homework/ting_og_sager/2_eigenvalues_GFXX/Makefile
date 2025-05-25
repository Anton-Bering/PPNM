all: main.exe run

main.exe: Main.cs Jacobi.cs Matrix.cs Vector.cs
	@mcs -optimize -target:exe -out:main.exe Main.cs Jacobi.cs Matrix.cs Vector.cs

run: main.exe
	@mono main.exe -rmax 10 -dr 0.3 > Out.txt

clean:
	@rm -f main.exe Out.txt

