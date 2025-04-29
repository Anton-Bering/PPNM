Out.txt: main.exe input.txt
	cat input.txt | mono main.exe > Out.txt

main.exe: main.cs genlist.dll
	mcs -target:exe -out:main.exe -reference:genlist.dll main.cs

genlist.dll: genlist.cs
	mcs -target:library -out:genlist.dll genlist.cs

input.txt:
	echo "129.24 24.8 4.847\n88.6 33.745 7.78\n30.39 99.811 6.723\n-1.33 96.3 2.23" > input.txt

clean:
	rm -f *.exe *.dll *.txt
