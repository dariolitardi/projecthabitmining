import glob
import errno
import os
import random
import datetime
import random
import math
from datetime import datetime
from datetime import timedelta
import discrete_traj_reconstructor
import trajectories_drawer
from tkinter import messagebox
from tkinter import filedialog
from tkinter import *
import sqlite3



def CalcolaMinimoTimestamp(listatimestamp):
    parsedstring=listatimestamp[0].split("_")

    idfilemin=parsedstring[1]
    tm=parsedstring[0]
    posizionemin=parsedstring[2]
    parsetm=tm.split(" ")[0]
    parsetime=tm.split(" ")[1]


    tm=parsetm.split("-")[2]+"."+parsetm.split("-")[1]+"."+parsetm.split("-")[0]+" "+parsetime
    timestampmin = datetime.strptime(tm,("%d.%m.%Y %H:%M:%S.%f"))

    c=0
    for i in range(1, len(listatimestamp)):
        parsedstring2 = listatimestamp[i].split("_")
        idfilemin2 = parsedstring2[1]
        tm2 = parsedstring2[0]
        parsedarray = tm2.split(' ')
        tm2 = parsedarray[0].split("-")[2] + "." + parsedarray[0].split("-")[1] + "." + parsedarray[0].split("-")[0] + " " + parsedarray[1]
        timestamp = datetime.strptime(tm2, ("%d.%m.%Y %H:%M:%S.%f"))
        posizionemin2 = parsedstring2[2]

        if (timestamp <timestampmin ):
            timestampmin = timestamp
            c=i
            idfilemin= idfilemin2
            posizionemin=posizionemin2



    s=timestampmin.strftime("%Y-%m-%d\t%H:%M:%S.%f")+"_"+str(c)+"_"+str(idfilemin)+"_"+ posizionemin
    return s

def LeggiLinea(listafile):
    listaPuntatori=list()
    for i in range(0, len(listafile)):
        stringa= listafile[i].readline()

        if (stringa == ""):
            continue
        parsedline=stringa.split('\t')
        linea =parsedline[0] + " "+parsedline[1]+"_"+str(i)+"_"+ parsedline[2]

        listaPuntatori.append(linea)
    return listaPuntatori

def goto(linenum):
        global line
        line = linenum

def LeggiFile(f, lista):
    linea=f.readline()
    while(linea!=""):
        lista.append(linea)
        linea = f.readline()



def main():
    root = Tk()
    root.directory = filedialog.askdirectory(initialdir="C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log", title="Select file")
    exists = os.path.isfile((root.directory+"\\DatasetPaths.txt"))
    if exists:

        os.remove(root.directory+"\\DatasetPaths.txt")

    exists2 = os.path.isfile(
        (root.directory+"\\trajectoriesDB.db"))
    if exists2:
        os.remove(root.directory+"\\trajectoriesDB.db")

    existsRealDB = os.path.isfile(
        (root.directory + "\\realTrajectoriesDB.db"))
    if existsRealDB:
        os.remove(root.directory + "\\realTrajectoriesDB.db")

    path= root.directory+"\\*.txt"


    #lettura dei pathlog della simulazione
    files = glob.glob(path)
    listafile=list()
    flag=False


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
    pathDataSetFile = root.directory+"\\DatasetPaths.txt"

    datasetfile = open(pathDataSetFile, "a+")
    listapuntatorilinee=None
    while(True):
        if(j==0):
            listapuntatorilinee=LeggiLinea(listafile)
            j+=1
        if( not listapuntatorilinee):
            datasetfile.close()
            #discrete_traj_reconstructor.RecontructPathLogs(root.directory)

            return


        tsminimo=CalcolaMinimoTimestamp(listapuntatorilinee)
        parsedstring=tsminimo.split("_")
        timestampmin=parsedstring[0]
        filetsminimo=int(parsedstring[1])
        idfiletsminimo=int(parsedstring[2])
        posizione=parsedstring[3]


        datasetfile.write(timestampmin+"\t"+posizione+"\t"+"ON"+"\n")


        ##rimuovo tsmin da listapuntatorilinee
        ##file tsminimo leggo una nuova linea e lo schiaffo in listapuntatorilinee

        parsedline=listafile[idfiletsminimo].readline().split("\t")

        if(parsedline[0] == ""  and len(listapuntatorilinee)!=0):

            datasetfile.write( "\n")
        if (parsedline[0] == "" and len(listapuntatorilinee)==0):
            #discrete_traj_reconstructor.RecontructPathLogs(root.directory)
            return
        if (parsedline[0] == ""):##fine del file
            datasetfile.close()

            #discrete_traj_reconstructor.RecontructPathLogs(root.directory)


            return

        if (len(parsedline)!=0): ##file ancora non finito da leggere
                listapuntatorilinee.append(parsedline[0]+" "+parsedline[1]+"_"+str(idfiletsminimo)+"_"+parsedline[2]+"_"+parsedline[3])

        del(listapuntatorilinee[filetsminimo])






if __name__=="__main__":

    main()




