all: erf.svg gamma.svg lngamma.svg

main.exe: main.cs
	mcs -target:exe -out:main.exe main.cs

erf.txt gamma.txt lngamma.txt: main.exe
	mono main.exe

erf.svg: erf.txt plot_erf.gp
	gnuplot plot_erf.gp

gamma.svg: gamma.txt plot_gamma.gp
	gnuplot plot_gamma.gp

lngamma.svg: lngamma.txt plot_lngamma.gp
	gnuplot plot_lngamma.gp

clean:
	rm -f *.exe *.txt *.svg
