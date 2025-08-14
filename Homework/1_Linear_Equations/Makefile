## --------- Makefile: Homework Linear_Equations ----------------------

CS      := mcs
CFLAGS  := -target:exe -langversion:latest -optimize+ -warn:4
EXE     := main.exe
RUN     := mono $(EXE)

GNUPLOT := plot.gpi
OUT     := Out.txt

# -------- Task C ----------------------------------------------------
TIME    := /usr/bin/time
REPEATS := 5

DIR_C   := Out_Task_C
DATA_C  := QR_factorize_time_fit.log \
           out.times.raw \
           out.times.data \
           out.times.stats

PLOTS_C := QR_factorize_time.svg
# --------------------------------------------------------------------

DATA    := $(DATA_C)
PLOTS   := $(PLOTS_C)

SRC := Main.cs \
       QR.cs

COMMON_DIR := ../../General
COMMON_SRC := $(COMMON_DIR)/Vector.cs \
              $(COMMON_DIR)/VectorHelpers.cs \
              $(COMMON_DIR)/Matrix.cs \
              $(COMMON_DIR)/MatrixHelpers.cs \
              $(COMMON_DIR)/HW_points.cs

CSFILES := $(SRC) $(COMMON_SRC)

MOVE_C := $(DATA_C) $(PLOTS_C)

.PHONY: all clean package timing plot

all: package

$(EXE): $(CSFILES)
	$(CS) $(CFLAGS) -out:$@ $^

$(OUT): $(EXE)
	$(RUN) > $@

# -------- Task C -----------------------------------------------------
# Rå målinger (N, tid) gentaget 'REPEATS' gange pr. N
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

# Gennemsnit pr. N -> out.times.data (N mean)
out.times.data: out.times.raw
	awk '{sum[$$1]+=$$2; cnt[$$1]++} END{for(n in sum) printf "%d %.6f\n", n, sum[n]/cnt[n]}' $< | sort -n > $@

# Gennemsnit + standardafvigelse pr. N -> out.times.stats (N mean sd)
out.times.stats: out.times.raw
	awk '{n=$$1; x=$$2; sum[n]+=x; sumsq[n]+=x*x; cnt[n]++} \
	     END{for(n in sum){m=sum[n]/cnt[n]; \
	                       sd=sqrt((sumsq[n]-sum[n]*sum[n]/cnt[n])/(cnt[n]-1)); \
	                       printf "%d %.6f %.6f\n", n, m, sd}}' $< | sort -n > $@

# Plot (gnuplot læser out.times.stats og skriver SVG + fit-log)
$(PLOTS): out.times.stats $(GNUPLOT)
	@gnuplot $(GNUPLOT)

# Convenience-targets
timing: out.times.stats
plot: $(PLOTS)

$(DIR_C):
	@mkdir -p $@

package: $(OUT) $(PLOTS) $(DATA) | $(DIR_C)
	mv $(MOVE_C) $(DIR_C)/

RM ?= rm
clean:
	-$(RM) -f $(EXE) $(OUT) $(DATA) $(PLOTS)
	-$(RM) -rf $(DIR_C)

# fit.log
