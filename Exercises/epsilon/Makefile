Out.txt: main.exe
	mono main.exe > Out.txt

main.exe: main.cs
	mcs -target:exe -out:main.exe main.cs

clean:
	rm -f main.exe Out.txt
