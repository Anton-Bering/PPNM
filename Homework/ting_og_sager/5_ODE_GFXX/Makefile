CC     = mcs
TARGET = main.exe

all: $(TARGET)
	mono $(TARGET) > Out.txt

$(TARGET): *.cs
	$(CC) -target:exe -out:$(TARGET) *.cs

clean:
	rm -f $(TARGET) Out.txt
