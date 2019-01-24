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
import time



class Position:
    x = 0
    y = 0
def CalcolaAngolo(position1, position2):
    m=(position2.y-position1.y)/(position2.x-position1.x)
    angolo=(math.atan(m)*180)/math.pi
    return angolo
def GetDistanza(position1, position2):
    distanza = float (math.sqrt((float (position1.x)-float (position2.x))**2 + (float (position1.y)- float (position2.y))**2))
    return distanza
def Confronto(listaVettoriVelocitaMedie, velocitaMedia):
    return
def CalcolaVelocitaVettorialeMedia(position1, position2, t1, t2):
    ##r1=[] ##questi sono i raggi vettori r1 e r2
    ##r2=[]
    ##pOrigine = Position()
    ##pOrigine.x=0
    ##pOrigine.y=0
    ##alpha1=(math.cos(CalcolaAngolo(pOrigine,position1))*180)/math.pi ##converte anche da radianti a gradi
    ## rx1= GetDistanza(position1, pOrigine)*alpha1
    ##ry1= GetDistanza(position1, pOrigine)*alpha1
    ##r1.append(rx1)
    ## r1.append(ry1)

    ##alpha2 = (math.cos(CalcolaAngolo(pOrigine, position2)) * 180) / math.pi
    ##rx2 = GetDistanza(position2, pOrigine) * alpha2
    ##ry2 = GetDistanza(position2, pOrigine) * alpha2
    ##r2.append(rx2)
    ##r2.append(ry2)
    velocita=[]
    intervallo_tempo=t2-t1
    print(intervallo_tempo.total_seconds())

    velocita.append((position2.x-position1.x)/(intervallo_tempo.total_seconds()))
    velocita.append((position2.y-position1.y)/(intervallo_tempo.total_seconds()))

    return velocita


def select_authorizer(*args):
    return sqlite3.SQLITE_OK
def RecontructPathLogs(pathDirectoryLog):

    pathLogUnico = pathDirectoryLog+"\\DatasetPaths.txt"
    f = open(pathLogUnico, "r")
    listasegmenti=list()
    j=0

    flag=False
    index=0
    while(True):
        if(j==0):

            linea1=f.readline()

            if linea1 == "":
                               ##il log unico è vuoto
                return

            linea2=f.readline()

            parsedline1=linea1.split(' ')
            parsedline2=linea2.split(' ')

            p0= Position()
            p0.x=float(parsedline1[1].replace(',','.'))
            p0.y =float(parsedline1[2].replace(',','.'))
            p1 = Position()


            p1.x =float(parsedline2[1].replace(',','.'))
            p1.y =float(parsedline2[2].replace(',','.'))
            distanza=GetDistanza(p0,p1)
            if(distanza>=25):
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
        if(linea=="" or j==1000 ):
            print(len(listasegmenti))
            ConstructDatabase(listasegmenti, pathDirectoryLog)
            return





        parsedline=linea.split(' ')
        p=Position()
        p.x=float(parsedline[1].replace(',','.'))
        p.y=float(parsedline[2].replace(',','.'))
        timestamp=parsedline[0]
        for i in range(0,len(listasegmenti)):
            s=listasegmenti[i][len(listasegmenti[i])-1]
            if(s=="fine_segmento"):
                continue
            parsedline = s.split(' ')
            pos = Position()
            pos.x = float(parsedline[1].replace(',','.'))
            pos.y = float(parsedline[2].replace(',','.'))
            distanza=GetDistanza(p,pos)
            timestampPos=parsedline[0]
            if(distanza==0 and timestamp==timestampPos): ##qua trova l'incrocio
                parsed=listasegmenti[i][len(listasegmenti[i])-2].split(' ')
                lastPosition0=Position()
                lastPosition0.x = float(parsed[1].replace(',', '.'))
                lastPosition0.y = float(parsed[2].replace(',', '.'))
                t0=datetime.strptime(parsed[0],("%H:%M:%S"))



                tm=datetime.strptime(timestampPos,("%H:%M:%S"))
                print(tm.strftime("%H:%M:%S")+" "+t0.strftime("%H:%M:%S"))
                v1=CalcolaVelocitaVettorialeMedia(lastPosition0, pos,t0,tm)
                p2=Position()
                p2.x=pos.x+v1[0]
                p2.y=pos.y+v1[1]
                print(str(p2.x)+" "+str(p2.y))
                lineaNuova= f.readline()
                lineaNuovaParsata=lineaNuova.split(' ')
                posNuova = Position()
                posNuova.x = float(lineaNuovaParsata[1].replace(',', '.'))
                posNuova.y = float(lineaNuovaParsata[2].replace(',', '.'))
                if(p2.x==posNuova.x and p2.y==posNuova.y):
                    listasegmenti[i].append(lineaNuova)
                    index = i
                    flag = True
                    continue
                else:
                    listasegmenti[index].append("fine_segmento")
                    listasegmenti[i].append("fine_segmento")
                    nuovosegmento=list()
                    nuovosegmento.append(linea)
                    nuovosegmento.append(f.readline())
                    listasegmenti.append(nuovosegmento)
                    nuovosegmento = list()
                    nuovosegmento.append(linea)

                    nuovosegmento.append(f.readline())
                    listasegmenti.append(nuovosegmento)

            if(distanza<=71 and timestampPos!=timestamp):

                        listasegmenti[i].append(linea)
                        index=i
                        flag=True


            if(flag==False ):
                segmentonuovo = list()
                segmentonuovo.append(linea)
                listasegmenti.append(segmentonuovo)
                flag=False











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
    conn = sqlite3.connect(pathDirectoryLog+'\\trajectoriesDB.db')
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