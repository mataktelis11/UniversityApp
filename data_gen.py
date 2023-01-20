# Using readlines()
file1 = open('MOCK_DATA.csv', 'r')
Lines = file1.readlines()
f = open("myfile.txt", "x")  
count = 0
# Strips the newline character
for line in Lines:
    line= line[:-1]
    count +=1
    arr = line.split(',')
    if count==1:
        continue
    if count<=11:
        #insert into dbo.Secretaries(SecretaryId, Name, Surname, Department, userid) values (1, 'Jennie', 'Sklaveniti', 'IT', 999);
        l= ["insert into dbo.Secretaries(SecretaryId, Name, Surname, Department, userid) values (",str(int(arr[0])),", '",arr[1],"', '"+arr[2]+"', 'IT', '"+arr[0]+"');\n"]
        f.writelines(l)
    elif count <=31:
        #insert into dbo.Professors (ProfessorId, AFM, Name, Surname, Department, userid) values (1, 100, 'Timmy', 'Alepis', 'IT', 677);
        l= ["insert into dbo.Professors (ProfessorId, AFM, Name, Surname, Department, userid) values (",str(int(arr[0])-10),",'500",str(int(arr[0])-10),"','",arr[1],"', '"+arr[2]+"', 'IT', '"+arr[0]+"');\n"]
        f.writelines(l)
    else:
        if int(arr[0])-30<10:
            l= ["insert into dbo.Students (studentId, Registration_number, Name, Surname, Department, userid) values (",str(int(arr[0])-30),",'1900",str(int(arr[0])-30),"','",arr[1],"', '"+arr[2]+"', 'IT', '"+arr[0]+"');\n"]
        elif int(arr[0])-30<100:
            l= ["insert into dbo.Students (studentId, Registration_number, Name, Surname, Department, userid) values (",str(int(arr[0])-30),",'190",str(int(arr[0])-30),"','",arr[1],"', '"+arr[2]+"', 'IT', '"+arr[0]+"');\n"]
        else:
            l= ["insert into dbo.Students (studentId, Registration_number, Name, Surname, Department, userid) values (",str(int(arr[0])-30),",'19",str(int(arr[0])-30),"','",arr[1],"', '"+arr[2]+"', 'IT', '"+arr[0]+"');\n"]
        f.writelines(l)

file1.close()
f.close()
    
# 10 first -> secretaries
# 20 professors
# 300 students
