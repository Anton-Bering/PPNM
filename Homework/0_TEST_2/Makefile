# Makefile
#   Type `make` to build, run, and capture output in Out.txt
#   Tested with GNU make + Mono (mcs).  Adjust compiler if you prefer dotnet.

CS_FILES = Main.cs vecotr_and_mactrics.cs
EXEC     = program.exe
OUT      = Out.txt

all: $(OUT)

$(EXEC): $(CS_FILES)
	@mcs -optimize+ -warn:4 -out:$(EXEC) $(CS_FILES)

$(OUT): $(EXEC)
	@mono $(EXEC) > $(OUT)

clean:
	@rm -f $(EXEC) $(OUT)
.PHONY: all clean
