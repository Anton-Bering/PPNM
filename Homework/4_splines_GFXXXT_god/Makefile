all: Out.txt plots

Out.txt: Splines.exe
	mono Splines.exe

Splines.exe: Main.cs LinearSpline.cs QuadraticSpline.cs CubicSpline.cs
	mcs -out:Splines.exe Main.cs LinearSpline.cs QuadraticSpline.cs CubicSpline.cs

plots: cos.txt sin.txt sqrt.txt plot.gnu
	gnuplot plot.gnu

clean: 
	rm -f Splines.exe cos.txt sin.txt sqrt.txt Out.txt cos.svg sin.svg sqrt.svg sqrt_comparing.svg comparison_results.txt Computed_bi_and_ci.txt bi_comparison.svg ci_comparison.svg
