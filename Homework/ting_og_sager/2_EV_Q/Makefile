# Filnavne
PROJECT = JDWCSProject
CSV_OUTPUT = delta_r_vs_epsilon0.csv
CSV_RMAX = rmax_vs_epsilon0.csv
CSV_EIG = eigenfunctions.csv
CSV_BENCHMARK = benchmark.csv

# Default target
all: build run delta_r_plot rmax_plot eigenfunction_plot

# Byg C# projektet
build:
	dotnet build

# Kør hovedprogram og gem output
run:
	dotnet run

# Plot ε0 vs Δr
delta_r_plot: $(CSV_OUTPUT)
	gnuplot -e "\
	set terminal svg size 800,600 background 'white'; \
	set output 'delta_r_vs_epsilon0.svg'; \
	set title 'ε₀ vs Δr'; \
	set xlabel 'Δr'; \
	set ylabel 'ε₀'; \
	set grid; \
	plot '$(CSV_OUTPUT)' using 1:2 with linespoints; \
	"

# Plot ε0 vs rmax
rmax_plot: $(CSV_RMAX)
	gnuplot -e "\
	set terminal svg size 800,600 background 'white'; \
	set output 'rmax_vs_epsilon0.svg'; \
	set title 'ε₀ vs r_{max}'; \
	set xlabel 'r_{max}'; \
	set ylabel 'ε₀'; \
	set xrange [2:10]; \
	set grid; \
	plot '$(CSV_RMAX)' using 1:2 with linespoints; \
	"

# Plot eigenfunctions
eigenfunction_plot: $(CSV_EIG)
	gnuplot -e "\
	set terminal svg size 800,600 background 'white'; \
	set output 'eigenfunctions.svg'; \
	set title 'Eigenfunctions'; \
	set xlabel 'r'; \
	set ylabel 'ψ'; \
	set grid; \
	set xrange [0:20]; \
	f1(x) = 2*x*exp(-x); \
	f2(x) = -x*(1-x/2)*exp(-x/2)/sqrt(2); \
	f3(x) = 2*x*(1-2*x/3 + 2*x*x/27)*exp(-x/3)/sqrt(27); \
	plot '$(CSV_EIG)' using 1:2 with points title 'ψ₁-data', \
	     '$(CSV_EIG)' using 1:3 with points title 'ψ₂-data', \
	     '$(CSV_EIG)' using 1:4 with points title 'ψ₃-data', \
	     f1(x) with lines title 'Analytical ψ₁' linewidth 3, \
	     f2(x) with lines title 'Analytical ψ₂' linewidth 3, \
	     f3(x) with lines title 'Analytical ψ₃' linewidth 3; \
	"

# Ryd op
clean:
	rm -f *.svg *.csv bin/ obj/
