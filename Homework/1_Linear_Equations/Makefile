## --------- Makefile: Homework Linear_Equations ---------

CS      := mcs
CFLAGS  := -target:exe -langversion:latest -optimize+ -warn:4
EXE     := main.exe
RUN     := mono $(EXE)
OUT     := Out.txt

# Task C:
TIME    := /usr/bin/time
GNUPLOT := plot.gpi
REPEATS := 5

# Kildekode
SRC := Main.cs \
       QR.cs

COMMON_DIR := ../../General
COMMON_SRC := $(COMMON_DIR)/Vector.cs \
              $(COMMON_DIR)/VectorHelpers.cs \
              $(COMMON_DIR)/Matrix.cs \
              $(COMMON_DIR)/MatrixHelpers.cs \
              $(COMMON_DIR)/HW_points.cs

CSFILES := $(SRC) $(COMMON_SRC)

.PHONY: all clean timing plot

all: $(OUT) QR_factorize_time.svg

# Kompilér
$(EXE): $(CSFILES)
	$(CS) $(CFLAGS) -out:$@ $^

# Kør program → Out.txt
$(OUT): $(EXE)
	$(RUN) > $@

# -------- Task C: Timing (gentagelser for stabilt gennemsnit) --------
# Rå målinger (N, tid) gentaget REPEATS gange pr. N
out.times.raw: $(EXE)
	> $@
	# Warm-up (JIT/initialisering) uden timing
	$(RUN) -size:100 >/dev/null 2>&1 || true
	# Mål N = 100,120,...,200
	for N in $$(seq 100 20 200); do \
		for r in $$(seq 1 $(REPEATS)); do \
			$(TIME) --format "$$N %e" --output $@ --append \
			$(RUN) -size:$$N >/dev/null 2>&1 || exit 1; \
		done; \
	done

# Gennemsnit + standardafvigelse pr. N -> out.times.stats (N mean sd)
out.times.stats: out.times.raw
	awk '{n=$$1; x=$$2; sum[n]+=x; sumsq[n]+=x*x; cnt[n]++} \
	     END{for(n in sum){m=sum[n]/cnt[n]; \
	                       sd=sqrt((sumsq[n]-sum[n]*sum[n]/cnt[n])/(cnt[n]-1)); \
	                       printf "%d %.6f %.6f\n", n, m, sd}}' $< | sort -n > $@

# Plot (gnuplot bruger out.times.stats og skriver QR_factorize_time.svg)
QR_factorize_time.svg: out.times.stats $(GNUPLOT)
	@gnuplot $(GNUPLOT)

# Convenience-targets
timing: out.times.stats
plot: QR_factorize_time.svg

# Ryd op
clean:
	- rm -f $(EXE) $(OUT) out.times.raw out.times.stats QR_factorize_time.svg QR_factorize_time_fit.log fit.log out err
