# Compiler and runtime settings (using Mono by default)
CSC = mcs
RUN = mono
# To use .NET SDK instead, uncomment below and adjust SDK path:
# SDKDIR = $(HOME)/.dotnet/sdk/6.0.102
# CSCPATH = $(SDKDIR)/Roslyn/bincore/csc.dll
# NETSTANDARD = $(SDKDIR)/ref/netstandard.dll
# CONFIG = $(SDKDIR)/vstest.console.runtimeconfig.json
# DOTNET = DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet
# CSC = $(DOTNET) $(CSCPATH) -reference:$(NETSTANDARD)
# RUN = $(DOTNET) exec --runtimeconfig $(CONFIG)

# Source files
CSFILES = main.cs lsfit.cs qr.cs matrix.cs vector.cs

# Default target: build the executable
lsfit.exe: $(CSFILES)
	$(CSC) -target:exe -out:lsfit.exe $(CSFILES)

# Run the program
run: lsfit.exe
	$(RUN) lsfit.exe

# Clean build artifacts
clean:
	rm -f lsfit.exe *.dll *.mdb Out.txt
