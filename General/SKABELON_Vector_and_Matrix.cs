/* ------ vecotr_and_mactrics.cs : Implement of my own matrix/vector classes ------ */

defurlt_tolerance=1e-12
defurlt_value_min=0
defurlt_value_max=1

/* --- Metrixes --- */

random_matrix(n,m,value_min=defurlt_value_min, value_max=defurlt_value_max):
    Generates a random matrix of size n x m with values between value_min and value_max

transport_matrix(A): // A is a matrix
    returns the transpose of A 

identity_matrix(n):
    returns an n x n identity matrix

size_of_matrix(A)
    returns the size of nxm matrix A

size_of_square_matrix(A)
    if size_of_matrix(A)[0] == size_of_matrix(A)[1]:
        return size_of_matrix(A)[0]
    else
        Console.WriteLine("Error: It is not a square matrix.");

/* --- Chek if a metrixes is ... --- */

CHECK_matrix(A_name,what,yes_or_no);
    Console.WriteLine("TEST: is the matrix",A_name,whot,"?");
    If yes_or_no = yes:
        Console.WriteLine("RESULT: yes.");
    elif yes_or_no = no:
        Console.WriteLine("RESULT: no.");
    else:
        Console.WriteLine("Error, missing the indput 'yes' or 'no'.");    

CHECK_upper_triangular(A,A_name,whot="upper triangular"); // A is a matrix
    if A is upper triangular: 
        CHECK_matrix(A_name,whot,yes);
    else 
        CHECK_matrix(A_name,whot,no);    
    // eksempel CHECK_upper_triangular(A,"A"):
    //      If A is upper triangular then; 
    //          TEST: is the matrix A upper tringuler?
    //          RESULT: yes.

CHECK_lower_triangular(A,name_A):
    CHECK_upper_triangular(transport_matrix(A),name_A,"lower triangular");
    // eksempel CHECK_upper_triangular(A,"A"):
    //      If A is lower triangular then; 
    //          TEST: is the matrix A lower tringuler?
    //          RESULT: yes.

CHECK_matrix_equrel_matrix(A,B,name_A,name_B,tolerance=defurlt_tolerance):
    Console.WriteLine("TEST: is ",name_A,"=",name_B"(within a tolerance of ,"tolerance")?";
    if A = B: 
        Console.WriteLine("RESULT: no.");
    else  
        Console.WriteLine("RESULT: no.");

CHECK_identity_matrix(A,name_A,tolerance=defurlt_tolerance):
    I = identity_matrix(size_of_square_matrix(A))
    CHECK_matrix_equrel_matrix(A,I,name_A,"I",tolerance=defurlt_tolerance):

/* --- Vectoes --- */
random_vector(n,value_min=0, value_max=1):
    random_matrix(n,1,value_min, value_max)

/* ------ Main.cs : Test of my own implement matrix/vector classes ------ */

n = 3
m = 4
A = random_matrix(n,m)
print("Here is the random metrix", n, "x", m, "A:\n", A)

A_transport transport_matrix(A):
print("The transport of A is:\n", A_transport)

I = identity_matrix(n):
print("The identity matrix of size", n, ":\n", I)

A_size = size_of_matrix(A)
print("The size of A is:", A_size)

B = random_matrix(n,n)
print("Here is the random square matrix B:\n", B)

B_size = size_of_square_matrix(B)
print("The size of B is:", B_size)

print("I will now check if B is upper triangular:", CHECK_upper_triangular(B,"B"))
print("I will now check if B is lower triangular:", CHECK_lower_triangular(B,"B"))

print("I will now check if B is equal to A:", CHECK_matrix_equrel_matrix(B,A,"B","A"))

print("I will now check if B is an identity matrix:", CHECK_identity_matrix(B,"B"))

V = random_vector(n):
print("Here is the random vector V of size", n, ":\n", V)

/* ------ Makefile ------ */

Skal gener en fil med navn Out.txt når man skrive 'make'
alt det jeg har skrevet som 'print()' skal fremgå i Out.txt
