GEN = ../../General/general_Vector_and_Matrix.cs \
	../../General/HW_points.cs
	
all : Out.txt QR_factorize_time.svg

# Kør program:
Out.txt : main.exe
	@mono main.exe > Out.txt

# Kompilér kode:
main.exe : Main.cs QR.cs $(GEN)
	mcs -out:$@ Main.cs QR.cs $(GEN)

# Lav timing af hvor lang tid det tager at QR-faktorisere en NxN matrix
QR_factorize_time.txt : main.exe
	>$@
	for N in $$(seq 100 20 500); do \
		time --format "$$N %e" --output $@ --append \
		mono $< -size:$$N 1>out 2>err ;\
	done

# Lav plot:
QR_factorize_time.svg:  QR_factorize_time.txt plot.gpi
	@gnuplot plot.gpi

# Ryd op:
clean:
	rm -f *.exe *.txt *.svg *.log