N=100000000
TIME = /usr/bin/time --portability --append --output $@

Out.txt: main.exe
	> $@
	$(TIME) mono main.exe -terms:$(N) -threads:1 >> $@
	$(TIME) mono main.exe -terms:$(N) -threads:2 >> $@
	$(TIME) mono main.exe -terms:$(N) -threads:3 >> $@
	$(TIME) mono main.exe -terms:$(N) -threads:4 >> $@

main.exe: main.cs
	mcs -out:main.exe -target:exe main.cs

clean:
	rm -f *.exe *.txt
