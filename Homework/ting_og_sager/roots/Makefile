CC = mcs
TARGET = main.exe
SRC = vector.cs matrix.cs QR.cs ode.cs root.cs main.cs

$(TARGET): $(SRC)
	$(CC) -out:$(TARGET) $(SRC)

run: $(TARGET)
	mono $(TARGET)

out.txt: $(TARGET)
	mono $(TARGET) > out.txt

clean:
	rm -f $(TARGET) out.txt
