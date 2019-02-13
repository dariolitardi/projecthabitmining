import glob
import errno
import os
import random
import datetime
import random
import math
from datetime import datetime
from datetime import timedelta
import traj_reconstructor
import trajectories_drawer
from tkinter import messagebox
from tkinter import filedialog
from tkinter import *
import sqlite3
class Position:
     x = 0
     y = 0

def select_authorizer(*args):
    return sqlite3.SQLITE_OK


def GetValoreIniziale(c, id_seg):
    id_val_iniziale=c.execute("SELECT min(id_valore) FROM valore_segmento WHERE id_segmento= ?", (id_seg,)).fetchone()[0]
    c.connection.commit()
    return id_val_iniziale
def GetValoreFinale(c, id_seg):
    id_val_finale=c.execute("SELECT max(id_valore) FROM valore_segmento WHERE id_segmento= ?", (id_seg,)).fetchone()[0]
    c.connection.commit()

    return id_val_finale
def InserimentiInDB(linea, c, conn):
    id_segmento=0
    parsedstring=linea.split(' ')
    id_valore_inizio=0
    id_valore_fine=0

    if(parsedstring[0]=="fine_segmento"):
        ##id_valore_fine = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]
       ## c.execute("INSERT INTO segmento VALUES (?, ?, ?) ;", (None, None, None))
        ##id_valore_fine = c.execute("SELECT id_valore FROM valore_segmento  WHERE id_segmento= ? ORDER BY id_valore DESC LIMIT 1",(int(id_segmento),)).fetchone()[0]
        # id_valore_inizio = c.execute("SELECT id_valore FROM valore_segmento WHERE id_segmento= ? ",( int(id_segmento),)).fetchone()[0]



        return

    timestamp=parsedstring[0]
    posizione=parsedstring[1]+ " "+ parsedstring[2]
    id_segmento = float(parsedstring[3])
    c.execute("INSERT INTO valore VALUES (?, ?, ?) ;", (None, timestamp, posizione))

    id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]


    c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_segmento, posizione))






def ConstructDatabase(listasegmenti,pathDirectoryLog):
    ##creo il database dove andrò a salvare i vari log
    conn = sqlite3.connect(pathDirectoryLog+'\\realTrajectoriesDB.db')
    conn.set_authorizer(select_authorizer)
    c = conn.cursor()


    # Create tables
    c.execute('''CREATE TABLE valore(id_valore INTEGER PRIMARY KEY AUTOINCREMENT, timestamp_pos TEXT, posizione TEXT)''')
    c.execute('''CREATE TABLE segmento(id_segmento INTEGER PRIMARY KEY AUTOINCREMENT, id_valore_inizio INTEGER, id_valore_fine INTEGER)''')
    c.execute('''CREATE TABLE valore_segmento(id_valore INTEGER, id_segmento INTEGER, posizione TEXT, FOREIGN KEY(id_valore) REFERENCES valore(id_valore), FOREIGN KEY(id_segmento) REFERENCES segmento(id_segmento))''')


    # Save (commit) the changes
    conn.commit()

    ##calcolo segmento con lunghezza piu grandezza in modo da usare la max length per il ciclo degli inserimenti
    maxlength=len(listasegmenti[0])
    for h in range(1, len (listasegmenti)):

        if(maxlength<len(listasegmenti[h])):
            maxlength=len(listasegmenti[h])

    for h in range(0, len(listasegmenti)):
        c.execute("INSERT INTO segmento VALUES (?, ?, ?);", (None, None, None))
    conn.commit()
    mialista=list()

    for j in range(0, maxlength):
        for i in range(0, len(listasegmenti)):
            if(j<=len( listasegmenti[i])-1):
                id_segmento=i+1
                id_valore=j+1



                if(listasegmenti[i][j]=="fine_segmento"):
                    mialista.append(listasegmenti[i][j] + " " + str(id_segmento)+" " +str(id_valore) )
                else:

                        mialista.append(listasegmenti[i][j] + " " + str(id_segmento)+" " +str(id_valore))




    mialista.sort()




    for entry in mialista:
        InserimentiInDB(entry, c, conn)

    ##ottengo gli id dei segmenti
    c.execute("SELECT id_segmento FROM segmento")
    conn.commit()

    array=[]
    for row in c:

        id_seg=float(row[0])
        array.append(id_seg)
    ##aggiorna la tabella segmento con i id_valore inizio e fine esatti
    for elem in array:
        id_valore_iniziale = GetValoreIniziale(c, elem)
        id_valore_finale = GetValoreFinale(c, elem)


        c.execute(" UPDATE segmento SET id_valore_inizio = ? , id_valore_fine = ? WHERE id_segmento= ?;",
                  (id_valore_iniziale, id_valore_finale, elem,))

    conn.commit()
    conn.close()


def CalcolaMinimoTimestamp(listatimestamp):
    parsedstring=listatimestamp[0].split('-')
    idfilemin=parsedstring[1]
    tm=parsedstring[0]
    posizionemin=parsedstring[2]
    timestampmin = datetime.strptime(tm,("%H:%M:%S.%f"))
    c=0
    for i in range(1, len(listatimestamp)):
        parsedstring2 = listatimestamp[i].split('-')
        idfilemin2 = parsedstring2[1]
        tm2 = parsedstring2[0]
        posizionemin2 = parsedstring2[2]

        timestamp = datetime.strptime(tm2, ("%H:%M:%S.%f"))
        if (timestamp <timestampmin ):
            timestampmin = timestamp
            c=i
            idfilemin= idfilemin2
            posizionemin=posizionemin2


    s= timestampmin.strftime("%H:%M:%S.%f")+"-"+str(c)+"-"+str(idfilemin)+"-"+ posizionemin
    return s

def LeggiLinea(listafile):
    listaPuntatori=list()
    for i in range(0, len(listafile)):
        stringa= listafile[i].readline()

        if (stringa == ""):
            continue
        parsedline=stringa.split(' ')

        linea =parsedline[0] + "-"+str(i)+"-"+parsedline[1]+ " "+parsedline[2]


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
    listaSegmentiDB=list()
    for name in files:
        try:
            with open(name) as f:
                f = open(name, "r")
                listaSeg=list()
                LeggiFile(f, listaSeg)
                listaSegmentiDB.append(listaSeg)
        except IOError as exc:  # Not sure what error this is
            if exc.errno != errno.EISDIR:
                raise

    ConstructDatabase(listaSegmentiDB,root.directory)
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

            traj_reconstructor.RecontructPathLogs(root.directory)
            #trajectories_drawer.TestApp().run()
            return


        tsminimo=CalcolaMinimoTimestamp(listapuntatorilinee)
        parsedstring=tsminimo.split('-')
        timestampmin=parsedstring[0]
        filetsminimo=int(parsedstring[1])
        idfiletsminimo=int(parsedstring[2])
        posizione=(parsedstring[3])


        datasetfile.write(timestampmin+" "+posizione)


        ##rimuovo tsmin da listapuntatorilinee
        ##file tsminimo leggo una nuova linea e lo schiaffo in listapuntatorilinee

        parsedline=listafile[idfiletsminimo].readline().split(' ')
        line=parsedline[0]

        if(line=="" and len(listapuntatorilinee)!=0):
            datasetfile.write( "\n")
        if (line == "" and len(listapuntatorilinee) == 0):
            return

        if (line != ""):

                listapuntatorilinee.append(line+"-"+str(idfiletsminimo)+"-"+parsedline[1]+" "+parsedline[2])

        del(listapuntatorilinee[filetsminimo])






if __name__=="__main__":
    main()




