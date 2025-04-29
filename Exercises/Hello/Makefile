Out.txt : hello.exe
	mono hello.exe > Out.txt

hello.exe : hello.cs
	mcs -target:exe -out:hello.exe hello.cs

clean:
	rm -f Out.txt hello.exe
