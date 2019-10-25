import datetime
import random
import math
from datetime import datetime
from datetime import timedelta
import sqlite3
import time
import kalman_filter
import performance_test
import collections

from collections import Counter
class Position:
    x = 0
    y = 0












def GetDistanza(position1, position2):
    distanza = abs(float (math.sqrt((float (position1.x)-float (position2.x))**2 + (float (position1.y)- float (position2.y))**2)))
    return distanza



def select_authorizer(*args):
    return sqlite3.SQLITE_OK
def RecontructPathLogs(pathDirectoryLog):
    pathLogUnico = pathDirectoryLog+"\\DatasetPaths.txt"
    f = open(pathLogUnico, "r")
    listasegmenti=list()
    j=0


    decisioniTotali=0
    while(True):
        flag=False
        if(j==0):

            linea1=f.readline()
            if linea1 == "":
                               ##il log unico è vuoto
                return

            linea2=f.readline()

            parsedline1=linea1.split('\t')
            parsedline2=linea2.split('\t')

            p0= Position()
            p0.x=float(parsedline1[3].replace(',','.'))
            p0.y =float(parsedline1[4].replace(',','.'))
            p1 = Position()


            p1.x =float(parsedline2[3].replace(',','.'))
            p1.y =float(parsedline2[4].replace(',','.'))
            distanza=GetDistanza(p0,p1)
            if(distanza>200):
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


        if(len(linea.strip()) == 0   ):
            print(len(listasegmenti))
            ConstructDatabase(listasegmenti, pathDirectoryLog)
            return

        parsedline=linea.split('\t')
        p=Position()
        p.x=float(parsedline[3].replace(',','.'))
        p.y=float(parsedline[4].replace(',','.'))
        timestamp=parsedline[0]+ "\t"+ parsedline[1]
        for i in range(0,len(listasegmenti)):
            s=listasegmenti[i][len(listasegmenti[i])-1]
            if(s=="fine_segmento"):
                continue

            parsedline = s.split('\t')
            pos = Position()
            pos.x = float(parsedline[3].replace(',','.'))
            pos.y = float(parsedline[4].replace(',','.'))
            distanza=GetDistanza(p,pos)
            timestampPos=parsedline[0]+ "\t"+ parsedline[1]
            if (distanza == 200 and timestamp == timestampPos):  ##qua trova un parallelismo
                # print(timestamp)
                # print(str(p.x)+" "+ str(p.y ))
                print("PARALLELISMO")
                # print(str(pos.x) +" "+str(pos.y))

                listasegmenti[i].append("fine_segmento")
                segmentonuovo = list()
                segmentonuovo.append(linea)
                listasegmenti.append(segmentonuovo)
                continue

            if(distanza==0 and timestamp==timestampPos  ): ##qua trova l'incrocio
                #print(timestamp)
                #print(str(p.x)+" "+ str(p.y ))
                print("INCCC")
                #print(str(pos.x) +" "+str(pos.y))

                listasegmenti[i].append("fine_segmento")
                incrocio=True
                segmentonuovo = list()
                segmentonuovo.append(linea)
                listasegmenti.append(segmentonuovo)
                continue





            if(distanza==200 and timestampPos!=timestamp ):


                flag=True
                listasegmenti[i].append(linea)



            if(flag==False and i == len(listasegmenti) - 1): #la posizione letta non è compatibile con nessun segmento quindi creo un nuovo segmento
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
        ##id_valore_fine = c.execute("SELECT id_valore FROM valore O



        # RDER BY id_valore DESC LIMIT 1").fetchone()[0]
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

    print(listasegmenti)
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


if __name__=="__main__":

    RecontructPathLogs("C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_26.06.2019_15.49.47.000")

