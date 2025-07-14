CC     = mcs
TARGET = main.exe

all: $(TARGET)
	mono $(TARGET)
	gnuplot plot.gpi

$(TARGET): *.cs
	$(CC) -target:exe -out:$(TARGET) *.cs

clean:
	rm -f $(TARGET) *.txt *.svg plot.gp *.png
