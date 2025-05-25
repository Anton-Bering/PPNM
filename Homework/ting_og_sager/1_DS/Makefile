all: main.exe Out.txt plot.png

main.exe: Main.cs Matrix.cs Vector.cs QR.cs
	mcs -out:main.exe Main.cs Matrix.cs Vector.cs QR.cs

Out.txt: main.exe
	mono main.exe > Out.txt

timing.txt: main.exe
	for N in $$(seq 10 10 300); do \
		mono main.exe -size:$$N >> timing.txt; \
	done

plot.png: timing.txt plot.gp
	gnuplot plot.gp

clean:
	rm -f main.exe Out.txt timing.txt plot.png

.PHONY: all clean