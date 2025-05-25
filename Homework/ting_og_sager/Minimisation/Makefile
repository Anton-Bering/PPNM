# Makefile til 'Minimization' opgaven

SOURCES = vector.cs matrix.cs QR.cs minimization.cs main.cs
TARGET = main.exe

# Kompilér
$(TARGET): $(SOURCES)
	mcs -out:$(TARGET) $(SOURCES)

# Kør programmet og vis output i terminalen
run: $(TARGET)
	mono $(TARGET)

# Kør og gem output i out.txt
out.txt: $(TARGET)
	mono $(TARGET) > out.txt

# Ryd op
clean:
	rm -f $(TARGET) out.txt
