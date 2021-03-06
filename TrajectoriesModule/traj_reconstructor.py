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
class Incrocio:
    position=None
    timestamp=None

class Position:
    x = 0
    y = 0
class Cluster:
    lista_posizioni=None
    id_segment=0
    isAssigned=False

class EstimatedPosition:
    distanza=0
    id_segment=0
    position=None


def isContained(lista_incroci, inc):
    for elem in lista_incroci:
        if (inc.position.x==elem.position.x and inc.position.y==elem.position.y and inc.timestamp==elem.timestamp):
            return True

    return False

def isContainedDuplicati(lista, pos):
    contatore=0
    for elem in lista:
        if (pos.x==elem.x and pos.y==elem.y):
            contatore+=1

    return contatore

def isContainedPuntiPC(listaPC, punto):
    for elem in listaPC:
        if (punto.x==elem.position.x and punto.y==(elem.position.y)):
            return True

    return False

def IsContainedDup(list_distances,elem):
    counter=0
    for i in range(0, len(list_distances)):
        if(list_distances[i].distanza==elem.distanza):
            counter+=1


    return counter

def GetDuplicates(list_distances):
    list_duplicates=list()

    for i in range(0, len(list_distances)):
        if(IsContainedDup(list_distances,list_distances[i])>1 and list_distances[i] not in list_duplicates):
            list_duplicates.append(list_distances[i])

    if(len(list_duplicates)==0):
        return None

    return list_duplicates

def GetPuntoVicino(list_distances):
    min=list_distances[0]
    for i in range(1, len(list_distances)):
        if(list_distances[i].distanza<min.distanza):
            min=list_distances[i]


    return min.id_segment

def GetDistanza(position1, position2):
    distanza = abs(float (math.sqrt((float (position1.x)-float (position2.x))**2 + (float (position1.y)- float (position2.y))**2)))
    return distanza


def select_authorizer(*args):
    return sqlite3.SQLITE_OK
def RecontructPathLogs(pathDirectoryLog):
    listaIncroci=list()
    pathLogUnico = pathDirectoryLog+"\\DatasetPaths.txt"
    f = open(pathLogUnico, "r")
    listasegmenti=list()
    j=0
    incrocio=False
    lista_incroci=list()
    lista_clusters=list()
    counter=0
    contatore=0
    index_closed_segment=0
    timestampIncrocio=""
    decisioniTotali=0
    list_distances=list()
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
            if(distanza>20):
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






        parsedline=linea.split(' ')
        p=Position()
        p.x=float(parsedline[1].replace(',','.'))
        p.y=float(parsedline[2].replace(',','.'))
        timestamp=parsedline[0]
        if (len(linea.strip()) == 0 or timestamp == "03:00:00.000"):
            print(len(listasegmenti))
            print(str(contatore) + " cont")
            print("LEN "+str(len(lista_incroci)))
            ConstructDatabase(listasegmenti, pathDirectoryLog)
            performance_test.test_kalmanfilter(lista_incroci, pathDirectoryLog)
            return

        for i in range(0,len(listasegmenti)):
            s=listasegmenti[i][len(listasegmenti[i])-1]

            if(s=="fine_segmento" ):
                continue

            parsedline = s.split(' ')
            pos = Position()
            pos.x = float(parsedline[1].replace(',','.'))
            pos.y = float(parsedline[2].replace(',','.'))
            distanza=GetDistanza(p,pos)
            timestampPos=parsedline[0]
            if(distanza==0 and timestamp==timestampPos and  len(listasegmenti[i]) >= 4):
                incrocio = Incrocio()
                incrocio.timestamp = timestamp
                incrocio.position = p
                lista_incroci.append(incrocio)


                continue

            if(distanza<=10 and timestampPos!=timestamp and  len(listasegmenti[i]) < 4):

                listasegmenti[i].append(linea)

                index = i
                flag = True
                continue

                # print(timestamp)
                # print(str(p.x)+" "+ str(p.y ))
                # print(str(pos.x) +" "+str(pos.y))
            if( s!="fine_segmento" and len(listasegmenti[i]) >= 4):
                parsed = listasegmenti[i][len(listasegmenti[i]) - 2].split(' ')
                lastPosition0 = Position()
                lastPosition0.x = float(parsed[1].replace(',', '.'))
                lastPosition0.y = float(parsed[2].replace(',', '.'))
                t0 = datetime.strptime(parsed[0], ("%H:%M:%S.%f"))

                parsed = listasegmenti[i][len(listasegmenti[i]) - 3].split(' ')

                lastPosition1 = Position()
                lastPosition1.x = float(parsed[1].replace(',', '.'))
                lastPosition1.y = float(parsed[2].replace(',', '.'))
                t1 = datetime.strptime(parsed[0], ("%H:%M:%S.%f"))
                cluster = Cluster()
                cluster.lista_posizioni = list()
                cluster.lista_posizioni.append(lastPosition1)
                cluster.lista_posizioni.append(lastPosition0)
                cluster.lista_posizioni.append(pos)
                cluster.id_segment = i
                lista_clusters.append(cluster)
                puntoIncrocio = pos
                timestampIncrocio = timestampPos
                tm = datetime.strptime(timestampPos, ("%H:%M:%S.%f"))
                print(str(pos.x) + " " + str(pos.y) + " " + timestampIncrocio)
                index = i
                d=kalman_filter.kalman_filter_position(cluster, p)
                ep= EstimatedPosition()
                ep.position=p
                ep.distanza=d
                ep.id_segment=i
                list_distances.append(ep)
                if (i == len(listasegmenti) - 1):
                    id_segment = GetPuntoVicino(list_distances)
                    listasegmenti[id_segment].append(timestamp + " " + str(p.x) + " " + str(p.y))
                    list_distances.clear()
                continue
            if (i == len(listasegmenti) - 1 and s=="fine_segmento"):
                print("ENTRAAAA")

                nuovo_segmento = list()
                nuovo_segmento.append(timestamp + " " + str(p.x) + " " + str(p.y))
                listasegmenti.append(nuovo_segmento)












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