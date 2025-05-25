# Makefile for building and running the Adaptive Integration project with Mono

# Output executable name
TARGET = AdaptiveIntegration.exe

# Source files
SOURCES = Integration.cs Program.cs

# Compiler settings
CSC = mcs
CSC_FLAGS = -optimize+

all: compile

compile:
	$(CSC) -target:exe -out:$(TARGET) $(SOURCES)

run: compile
	mono $(TARGET)

clean:
	rm -f $(TARGET)
