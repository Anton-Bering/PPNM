all: Out.txt activity_errorbars.png activity_fit_limits.png

LeastSquares.exe: Main.cs Matrix.cs Vector.cs LeastSquares.cs
	mcs -out:$@ $^

Out.txt: LeastSquares.exe
	mono $< > $@

data.txt fit.txt fit_limits.txt: Out.txt
	# Disse filer genereres af programmet, så vi behøver ikke ekstra regler

activity_errorbars.png: plot_errorbars.gp data.txt fit.txt
	gnuplot plot_errorbars.gp

activity_fit_limits.png: plot_fit_limits.gp fit_limits.txt
	gnuplot plot_fit_limits.gp

clean:
	rm -f LeastSquares.exe
	rm -f Out.txt data.txt fit.txt fit_limits.txt
	rm -f activity_errorbars.png activity_fit_limits.png