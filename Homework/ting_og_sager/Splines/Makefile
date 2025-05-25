# Makefile for building the Splines project
CSC = mcs
TARGET = splines.exe

SRC = Spline.cs main.cs

$(TARGET): $(SRC)
	$(CSC) -target:exe -out:$(TARGET) $(SRC)

run: $(TARGET)
	mono $(TARGET) > Out.txt
