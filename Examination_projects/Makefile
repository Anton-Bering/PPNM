# ------- Makefile: Quasi-Newton (Broyden/SR1) using Vector_and_Matrix.cs + QR -------
CS      := mcs
CFLAGS  := -target:exe -langversion:latest -optimize+
EXE     := main.exe
RUN     := mono $(EXE)
OUT     := Out.txt

SRC     := Main.cs QuasiNewton.cs Problems.cs \
           ../General/Vector_and_Matrix.cs \
           ../Homework/1_Linear_Equations/QR.cs

.PHONY: all run clean

all: $(OUT)

$(EXE): $(SRC)
	$(CS) $(CFLAGS) -out:$(EXE) $(SRC)

$(OUT): $(EXE)
	$(RUN) > $(OUT)
	@echo "Finished. Results written to $(OUT)"

run: $(EXE)
	$(RUN)

clean:
	rm -f $(EXE) $(OUT)
