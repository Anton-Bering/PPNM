Out.txt: main.exe
	mono main.exe > Out.txt

main.exe: main.cs sfuns.dll
	mcs -target:exe -out:main.exe -reference:sfuns.dll main.cs

sfuns.dll: sfuns.cs
	mcs -target:library -out:sfuns.dll sfuns.cs

clean:
	rm -f *.exe *.dll Out.txt
