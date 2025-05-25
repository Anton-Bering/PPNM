all: Out.txt

main.exe: Main.cs Newton.cs Functions.cs Utils.cs
	mcs -out:main.exe Main.cs Newton.cs Functions.cs Utils.cs

Out.txt: main.exe higgs.data.txt
	mono main.exe < higgs.data.txt > Out.txt 2> log

clean:
	rm -f main.exe Out.txt log
