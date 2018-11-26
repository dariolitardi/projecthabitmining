import glob
import errno
import os
import random
import datetime
from datetime import timedelta

class Position:
     x=""
     y=""

def goto(linenum):
        global line
        line = linenum

def MioMetodo(f):
    nuovalista=[]
    c=0
    for line in f:
        if (c == 86300):
            break

        vettorelinea = line.split(' ')
        position = Position()
        position.x = vettorelinea[1].rstrip()
        position.y = vettorelinea[2].rstrip()

        nuovalista.append(position)
        c += 1

    return nuovalista

def main():
    some_list = {}
    path= "C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\*.txt"
    i=1
    c=0
##    list = os.listdir(path)  # dir is your directory path
  ##  number_files = len(list)
    #lettura dei pathlog della simulazione
    files = glob.glob(path)
    ##s = random.randint(1, number_files + 1)
    mydate = datetime.datetime(datetime.datetime.now().year, datetime.datetime.now().month,
                           datetime.datetime.now().day,0,0,0)
    posizioni=[]
    some_list[mydate.strftime("%H:%M:%S")]=posizioni


    j=1
    while(j<86300):
        mydate = mydate + timedelta(seconds=1)
        some_list[mydate.strftime("%H:%M:%S")] = posizioni

        j+=1


    lista=[]


    for name in files:
        try:
            with open(name) as f:
                lista.append(MioMetodo(f))

        except IOError as exc:  # Not sure what error this is
            if exc.errno != errno.EISDIR:
                raise




    j = 0
    while (j < 86300):
        if(j==0):
            mydate = datetime.datetime(datetime.datetime.now().year, datetime.datetime.now().month,
                                       datetime.datetime.now().day, 0, 0, 0)
        else:
            mydate = mydate + timedelta(seconds=1)

        for k in range(0,len(lista)):
            some_list[mydate.strftime("%H:%M:%S")].append(lista[k][j])

        j += 1


    print( some_list[("00:00:00")][0].x+ " "+some_list[("00:00:00")][0].y)
    print( some_list[("00:00:00")][1].x+ " "+some_list[("00:00:00")][1].y)

    print( some_list[("00:00:00")][2].x+ " "+some_list[("00:00:00")][2].y)
    print( some_list[("00:00:00")][3].x+ " "+some_list[("00:00:00")][3].y)

if __name__=="__main__":
    main()


