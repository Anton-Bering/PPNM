# Makefile for compiling the C# solution and running tests
# Default target: build and run the program, directing output to Out.txt
all: main.exe
	mono main.exe > Out.txt

# Compile the C# source files into an executable (requires mcs or csc)
main.exe: Main.cs Matrix.cs Vector.cs QR.cs
	mcs -out:main.exe Main.cs Matrix.cs Vector.cs QR.cs

# Target for performance measurement (Part C) – generates out.times.data using POSIX time
out.times.data: main.exe
	> $@
	for N in $$(seq 100 20 200); do \
		time --format "$$N %e" --output $@ --append \
		mono $< -size:$$N 1>/dev/null 2>/dev/null; \
	done

# Clean up generated files
clean:
	rm -f main.exe Out.txt out.times.data

