# Linear Equations

In this folder is my solution to the 1st homework assignment (week 9).   

The full description of the assignment can be found [here](https://fedorov.sdfeu.org/prog/homeworks/lineq.htm).

## Objective

Implement functions to do the following:

**1)** Solve linear equations  
**2)** Calculate matrix inverse  
**3)** Calculate matrix determinant  

The method used is the *modified Gram-Schmidt orthogonalization*.

## Tasks

**a)** Solve linear equations using QR-decomposition by modified Gram-Schmidt orthogonalization (6 points)   
**b)** Compute matrix inverse using Gram-Schmidt QR factorization (3 points)  
**c)** Count the number of operations for QR-decomposition (1 points)

## Kompilering og kørsel

- **Kompilering**: Kør `make` i projektmappen for at kompilere C#-koden til `main.exe`.  
- **Kør test (del A & B)**: Kør `make run` for at eksekvere programmets tests. Programmet vil udskrive verificering af QR-dekomponeringen, løsning af ligninger og invers-beregning til konsollen (delopgave A og B). Alternativt kan man køre programmet manuelt med `mono main.exe`.  
- **Gem output**: Kør `make out.txt` for at gemme testoutput i filen `Out.txt`.  
- **Ydelsesmåling (del C)**: Kør `make out.times.data` for automatisk at måle køretiden for QR-faktorisering på forskellige matrixstørrelser (N = 100, 120, ..., 200). Dette benytter POSIX `time`-værktøjet til at registrere køretiden for hver størrelse. Resultaterne gemmes i filen `out.times.data`, hvor hver linje indeholder N og den målte tid (sekunder).  
- **Plot graf**: Kør `make plot` for at generere grafen `timing.pdf`. Scriptet `plot.gnu` anvender Gnuplot til at plotte kørselstiderne som funktion af N og laver et fit af kurven `a * N^3` til dataene.  
- **Åbn graf**: Åbn `timing.pdf` manuelt eller kør `make open` (kræver xdg-open på Linux) for at se grafen.

*Forudsætninger*: C#-kompilator (f.eks. `mcs` fra Mono) skal være installeret for at bygge programmet, og Gnuplot skal være tilgængelig for at generere plottet.
