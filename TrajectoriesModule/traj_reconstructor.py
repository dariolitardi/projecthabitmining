import glob
import errno
import os
import random
import datetime
import random
import math
from datetime import datetime
from datetime import timedelta
import sqlite3


class Position:
    x = 0
    y = 0

def GetDistanza(position1, position2):
    distanza = int (math.sqrt((int (position1.x)-int (position2.x))**2 + (int (position1.y)- int (position2.y))**2))
    return distanza



def select_authorizer(*args):
    return sqlite3.SQLITE_OK
def RecontructPathLogs():


    pathLogUnico = "C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\DatasetPaths.txt"
    f = open(pathLogUnico, "r")
    listasegmenti=list()
    j=0
    flag=False
    index=0
    while(True):
        if(j==0):
            linea1=f.readline()
            if(linea1==""): ##il log unico è vuoto
                return
            linea2=f.readline()
            parsedline1=linea1.split(' ')
            parsedline2=linea2.split(' ')

            p0= Position()
            p0.x=int(parsedline1[1])
            p0.y =int(parsedline1[2])
            p1 = Position()


            p1.x =int(parsedline2[1])
            p1.y =int(parsedline2[2])
            distanza=GetDistanza(p0,p1)
            if(distanza>35):
                lista0=list()
                lista1=list()
                lista0.append(linea1)
                lista1.append(linea2)
                listasegmenti.append(lista0)
                listasegmenti.append(lista1)

            else:
                lista0 = list()
                lista0.append(linea1)
                lista0.append(linea2)
                listasegmenti.append(lista0)

        j+=1
        linea=f.readline()
        if(linea=="" or j==100 ):
            print(len(listasegmenti))
            ConstructDatabase(listasegmenti)
            return





        parsedline=linea.split(' ')
        p=Position()
        p.x=int(parsedline[1])
        p.y=int(parsedline[2])
        timestamp=parsedline[0]
        for i in range(0,len(listasegmenti)):
            s=listasegmenti[i][len(listasegmenti[i])-1]
            if(s=="fine_segmento"):
                continue
            parsedline = s.split(' ')
            pos = Position()
            pos.x = int(parsedline[1])
            pos.y = int(parsedline[2])
            distanza=GetDistanza(p,pos)
            timestampPos=parsedline[0]
            if(distanza==0 and timestamp==timestampPos):
                listasegmenti[index].append("fine_segmento")
                listasegmenti[i].append("fine_segmento")
                nuovosegmento=list()
                nuovosegmento.append(f.readline())
                listasegmenti.append(nuovosegmento)
                nuovosegmento = list()
                nuovosegmento.append(f.readline())
                listasegmenti.append(nuovosegmento)

            if(distanza<=35 and timestampPos!=timestamp):

                        listasegmenti[i].append(linea)
                        index=i
                        flag=True


            if(flag==False ):
                segmentonuovo = list()
                segmentonuovo.append(linea)
                listasegmenti.append(segmentonuovo)
                flag=False











def InserimentiInDB(segmento, c, conn):
    id_valore_inizio=0
    id_valore_fine=0
    c.execute("INSERT INTO segmento VALUES (?, ?, ?);", (None, None, None))
    id_segmento = c.execute("SELECT id_segmento FROM segmento ORDER BY id_segmento DESC LIMIT 1").fetchone()[0]
    id_valore_inizio=0
    for k in range(0, len(segmento)):

        if(segmento[k]=="fine_segmento"):

            id_valore_fine = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]
            c.execute("UPDATE segmento SET id_valore_inizio = ? ,id_valore_fine = ? WHERE id_segmento = ? ;",
                      (id_valore_inizio, id_valore_fine, id_segmento,))

            continue
        parsedstring=segmento[k].split(' ')
        timestamp=parsedstring[0]
        posizione=parsedstring[1]+ " "+ parsedstring[2]
        c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamp, posizione))
        id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]

        c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_segmento, posizione))
        if (k == 0):
            id_valore_inizio = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]

        if (k == len(segmento)-1):
            id_valore_fine = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]

    c.execute("UPDATE segmento SET id_valore_inizio = ? ,id_valore_fine = ? WHERE id_segmento = ? ;", ( id_valore_inizio,id_valore_fine,id_segmento,))
    conn.commit()


def ConstructDatabase(listasegmenti):
    ##creo il database dove andrò a salvare i vari log
    conn = sqlite3.connect('C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\trajectoriesDB.db')
    conn.set_authorizer(select_authorizer)
    c = conn.cursor()


    # Create tables
    c.execute('''CREATE TABLE valore(id_valore INTEGER PRIMARY KEY AUTOINCREMENT, timestamp_pos TEXT, posizione TEXT)''')
    c.execute('''CREATE TABLE segmento(id_segmento INTEGER PRIMARY KEY AUTOINCREMENT, id_valore_inizio INTEGER, id_valore_fine INTEGER)''')
    c.execute('''CREATE TABLE valore_segmento(id_valore INTEGER, id_segmento INTEGER, posizione TEXT, FOREIGN KEY(id_valore) REFERENCES valore(id_valore), FOREIGN KEY(id_segmento) REFERENCES segmento(id_segmento))''')


    # Save (commit) the changes
    conn.commit()




    for i in range(0, len(listasegmenti)):

        InserimentiInDB(listasegmenti[i], c, conn)

    conn.close()