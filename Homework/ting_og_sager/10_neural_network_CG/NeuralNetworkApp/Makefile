.PHONY: all run clean

# Standard: byg og kør, gem output i Out.txt
all: Out.txt

Out.txt: 
	dotnet build
	dotnet run > Out.txt

# Ekstra: kør uden at gemme
run:
	dotnet run

# Ryd op
clean:
	dotnet clean
	rm -f Out.txt
