Out_cmd.txt: main.exe
	mono main.exe -numbers:1,2,3,4,5 > Out_cmd.txt

Out_stdin.txt: main.exe input.txt
	cat input.txt | mono main.exe mode:stdin 2> Out_stdin.txt

output.txt: main.exe input.txt
	mono main.exe -input:input.txt -output:output.txt

main.exe: main.cs
	mcs -target:exe -out:main.exe main.cs

input.txt:
	echo "1\n2\n3\n4\n5" > input.txt

clean:
	rm -f *.exe *.txt
