import glob
import errno
import os
import random
import datetime
import random
import math
from datetime import datetime
from datetime import timedelta

class Position:
     x = 0
     y = 0
def CalcolaMinimoTimestamp(listatimestamp):
    parsedstring=listatimestamp[0].split(',')
    idfilemin=parsedstring[1]
    tm=parsedstring[0]
    posizionemin=parsedstring[2]
    timestampmin = datetime.strptime(tm,("%H:%M:%S"))
    c=0
    for i in range(1, len(listatimestamp)):
        parsedstring2 = listatimestamp[i].split(',')
        idfilemin2 = parsedstring2[1]
        tm2 = parsedstring2[0]
        posizionemin2 = parsedstring2[2]

        timestamp = datetime.strptime(tm2, ("%H:%M:%S"))
        if (timestamp <timestampmin ):
            timestampmin = timestamp
            c=i
            idfilemin= idfilemin2
            posizionemin=posizionemin2


    s= timestampmin.strftime("%H:%M:%S")+","+str(c)+","+str(idfilemin)+","+ posizionemin
    return s

def LeggiLinea(listafile):
    listaPuntatori=list()
    for i in range(0, len(listafile)):
        stringa= listafile[i].readline()
        if (stringa == ""):
            continue
        parsedline=stringa.split(' ')

        linea =parsedline[0] + ","+str(i)+","+parsedline[1]+ " "+parsedline[2]


        listaPuntatori.append(linea)
    return listaPuntatori

def goto(linenum):
        global line
        line = linenum



def main():
    exists = os.path.isfile(("C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\DatasetPaths.txt"))
    if exists:
        os.remove("C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\DatasetPaths.txt")


    path= "C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\*.txt"


    #lettura dei pathlog della simulazione
    files = glob.glob(path)
    listafile=list()


    for name in files:
        try:
            with open(name) as f:
                f= open(name, "r")
                listafile.append(f)
        except IOError as exc:  # Not sure what error this is
            if exc.errno != errno.EISDIR:
                raise

    j=0
    ##scrive i dati su file
    pathDataSetFile = "C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\DatasetPaths.txt"

    datasetfile = open(pathDataSetFile, "a+")
    listapuntatorilinee=None
    while(True):
        if(j==0):
            listapuntatorilinee=LeggiLinea(listafile)
            j+=1

        if( not listapuntatorilinee):
            return


        tsminimo=CalcolaMinimoTimestamp(listapuntatorilinee)
        parsedstring=tsminimo.split(',')
        timestampmin=parsedstring[0]
        filetsminimo=int(parsedstring[1])
        idfiletsminimo=int(parsedstring[2])
        posizione=(parsedstring[3])

        ##scrivi tsminimo in log

        datasetfile.write(timestampmin+" "+posizione)


        ##rimuovo tsmin da listapuntatorilinee
        ##file tsminimo leggo una nuova linea e lo schiaffo in listapuntatorilinee
        parsedline=listafile[idfiletsminimo].readline().split(' ')
        line=parsedline[0]

        if(line=="" and len(listapuntatorilinee)!=0):
            datasetfile.write("\n")
        if (line == "" and len(listapuntatorilinee) == 0):
            return
        if (line != ""):

                listapuntatorilinee.append(line+","+str(idfiletsminimo)+","+parsedline[1]+" "+parsedline[2])

        del(listapuntatorilinee[filetsminimo])





if __name__=="__main__":
    main()


