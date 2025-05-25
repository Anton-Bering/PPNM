all: Out.txt plots

Out.txt: Splines.exe
	mono Splines.exe

Splines.exe: Main.cs LinearSpline.cs QuadraticSpline.cs CubicSpline.cs
	mcs -out:Splines.exe Main.cs LinearSpline.cs QuadraticSpline.cs CubicSpline.cs

plots: cos.txt sin.txt sqrt.txt plot.gnu
	gnuplot plot.gnu

clean: 
	rm -f Splines.exe cos.txt sin.txt sqrt.txt Out.txt cos.png sin.png sqrt.png sqrt_comparing.png comparison_results.txt
