Out.txt: main.exe
	mono main.exe > Out.txt

main.exe: main.cs
	mcs -target:exe -out:main.exe -reference:System.Numerics.dll main.cs

clean:
	rm -f *.exe Out.txt
