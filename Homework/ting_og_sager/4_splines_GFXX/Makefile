all: Out.txt

Out.txt: Splines.exe
	mono Splines.exe > Out.txt

Splines.exe: Main.cs LinearSpline.cs QuadraticSpline.cs CubicSpline.cs
	mcs -out:Splines.exe Main.cs LinearSpline.cs QuadraticSpline.cs CubicSpline.cs

clean:
	rm -f Splines.exe Out.txt
