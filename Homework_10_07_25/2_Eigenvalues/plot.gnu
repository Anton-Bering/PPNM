#! /usr/bin/env gnuplot
#
#  plot.gpi  – master plot driver
#
#  Purpose : Run every stand‑alone *.gnuplot* script in one go
#            so a single command creates all figures.
#  Usage   : gnuplot plot.gpi               (normally invoked via Makefile)
#
# ------------------------------------------------------------------------

reset                # start from a clean state for each sub‑script
set term svg enhanced size 700,500 font "Helvetica,12"

# --- list each standalone plot script here ------------------------------
load 'plot_dr.gnuplot'               # produces  varying_dr.svg
load 'plot_rmax.gnuplot'             # produces  rmax.svg
load 'plot_eigenfunctions.gnuplot'   # produces  eigenfunctions.svg
load 'plot_number_of_operations.gnuplot'   # produces  number_of_operations.svg
# ------------------------------------------------------------------------

unset output
exit
