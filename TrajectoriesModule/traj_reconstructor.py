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
        if(linea=="" or j==50):


            ConstructDatabase(listasegmenti)
            return

        parsedline=linea.split(' ')
        p=Position()
        p.x=int(parsedline[1])
        p.y=int(parsedline[2])
        timestamp=parsedline[0]
        flag=False
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
                listasegmenti[i].append(linea)
                listasegmenti[i].append("fine_segmento")
                segmentonuovo = list()
                segmentonuovo.append(linea)
                listasegmenti.append(segmentonuovo)
                break
            if(distanza<=35 and timestampPos!=timestamp):
                listasegmenti[i].append(linea)
                break
            if(i==len(listasegmenti)-1 and distanza>35):
                segmentonuovo = list()
                segmentonuovo.append(linea)
                listasegmenti.append(segmentonuovo)










def InserimentiInDB(segmento, c, conn):
    id_valore_inizio=0
    id_valore_fine=0
    c.execute("INSERT INTO segmento VALUES (?, ?, ?);", (None, None, None))
    id_segmento = c.execute("SELECT id_segmento FROM segmento ORDER BY id_segmento DESC LIMIT 1").fetchone()[0]

    for k in range(0, len(segmento)):
        parsedstring=segmento[k].split(' ')
        timestamp=parsedstring[0]
        posizione=parsedstring[1]+ " "+ parsedstring[2]
        c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamp, posizione))
        id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]

        c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_segmento, posizione))

        if(k==0):

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


'''''
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
            timestamppos0=parsedline1[0]
            p1 = Position()
            timestamppos1=parsedline2[0]

            p1.x =int(parsedline2[1])
            p1.y =int(parsedline2[2])
            distanza=GetDistanza(p0,p1)
            if(distanza>35):
                s = str(p0.x) + " " + str(p0.y)
                s1 = str(p1.x) + " " + str(p1.y)


                c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamppos0, s))
                id_valore=c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]
                c.execute("INSERT INTO segmento VALUES (?, ?, ?)",(None, id_valore, id_valore))

                id_segmento0=c.execute("SELECT id_segmento FROM segmento ORDER BY id_segmento DESC LIMIT 1").fetchone()[0]

                c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_segmento0, s))

                c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamppos1, s1))
                id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]

                c.execute("INSERT INTO segmento VALUES (?, ?, ?);", (None, id_valore, id_valore))

                id_segmento1 = c.execute("SELECT id_segmento FROM segmento ORDER BY id_segmento DESC LIMIT 1").fetchone()[0]

                c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_segmento1, s1))
                conn.commit()
                lista.append(id_segmento0)
                lista.append(id_segmento1)


            else:
                s = str(p0.x) + " " + str(p0.y)
                s1 = str(p1.x) + " " + str(p1.y)
                c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamppos0, s))
                id_valore0 = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]
                c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamppos1, s1))
                id_valore1 = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]

                c.execute("INSERT INTO segmento VALUES (?, ?, ?);", (None, id_valore0, id_valore1))



                id_segmento0 = c.execute("SELECT id_segmento FROM segmento ORDER BY id_segmento DESC LIMIT 1").fetchone()[0]
                c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore0, id_segmento0,s))
                c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore1, id_segmento0,s1))
                conn.commit()

                lista.append(id_segmento0)
                lista.append( id_segmento0)


        j+=1
        linea = f.readline()
        if(linea=="" or j==20):
            conn.close()
            return

        parsedline = linea.split(' ')

        p = Position()
        p.x = int(parsedline[1])
        p.y = int(parsedline[2])
        timestamppos=parsedline[0]
        d0=GetDistanza(p, p0)
        d1=GetDistanza(p, p1)

        if(d0==0 and timestamppos0==timestamppos): ##controllo incroci (si chiude il segmento)
            s = str(p.x) + " " + str(p.y)

            c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamppos, s))
            id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]
            c.execute("UPDATE segmento SET id_valore_fine = ?", (id_valore,))

            c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_segmento0, s))

            conn.commit()
            p0=p1
            linea = f.readline()

            if (linea == ""):
                conn.close()
                return

            parsedline = linea.split(' ')

            p1 = Position()
            p1.x = int(parsedline[1])
            p1.y = int(parsedline[2])
            continue
        if (d1 == 0 and timestamppos1 == timestamppos):##controllo incroci (si chiude il segmento)
            s = str(p.x) + " " + str(p.y)

            c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamppos, s))
            id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]
            c.execute("UPDATE segmento SET id_valore_fine = ?", (id_valore,))

            c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_segmento1, s))

            conn.commit()
            p0=p0
            linea = f.readline()

            if (linea == ""):
                conn.close()
                return

            parsedline = linea.split(' ')

            p1 = Position()
            p1.x = int(parsedline[1])
            p1.y = int(parsedline[2])

            continue
        if(d0<=35):            # mettilo nel segmento di p0
            s=str(p.x)+" "+str(p.y)

            c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamppos, s))
            id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]

            id_segmento0=lista[0]
            c.execute("UPDATE segmento SET id_valore_fine = ?", ( id_valore,))

            c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_segmento0, s))

            conn.commit()

            p0 = p1
            p1 = p
            lista[0]=lista[1]
            lista[1]=id_segmento0
            continue

        if (d1 <= 35):            # mettilo nel segmento di p1
            s = str(p.x) + " " + str(p.y)

            c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamppos, s))
            id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]
            c.execute("UPDATE segmento SET id_valore_fine = ?", ( id_valore,))
            id_segmento1=lista[1]

            c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_segmento1, s))

            conn.commit()
            lista.clear()
            p0 = p1
            p1 = p
            lista[0] = lista[1]
            lista[1] = id_segmento1
            continue
        if (d0>35 and d1>35):  # mettilo in nuovo segmento
            s=str(p.x)+" "+str(p.y)
            c.execute("INSERT INTO valore VALUES (?, ?, ?);", (None, timestamppos, s))
            id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]

            c.execute("INSERT INTO segmento VALUES (?, ?, ?);", (None, id_valore, id_valore))
            id_seg = c.execute("SELECT id_segmento FROM segmento ORDER BY id_segmento DESC LIMIT 1").fetchone()[0]

            id_valore = c.execute("SELECT id_valore FROM valore ORDER BY id_valore DESC LIMIT 1").fetchone()[0]

            c.execute("INSERT INTO valore_segmento VALUES (?, ?, ?);", (id_valore, id_seg, s))

            conn.commit()
            p0 = p1
            p1 = p
            lista[0] = lista[1]
            lista[1] = id_seg
            continue
'''''