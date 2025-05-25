# Name of the output file
OUTFILE = out.txt

# Default target
all: build run

# Build the project
build:
	dotnet build

# Run the project and save output to out.txt
run:
	dotnet run > $(OUTFILE)

# Clean build artifacts
clean:
	dotnet clean
	rm -f $(OUTFILE)
