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
import kalman_filter
class Cluster:
    lista_posizioni=None
    id_segment=0
    isAssigned=False
class Position:
    x = 0
    y = 0
    id_seg=0

class Incrocio:
    position=None
    timestamp=None
    listaCluster=None
    id_val=0
    count=0
class Tupla:
    id_seg=0
    id_log=0

def isContainedDuplicati(lista, pos):
    contatore=0
    for elem in lista:
        if (pos.x==elem.x and pos.y==elem.y):
            contatore+=1

    return contatore

def isContainedDuplicatiId(lista, tupla, id_log):
    for elem in lista:
        if (tupla.id_log==elem.id_log and tupla.id_seg==elem.id_seg):
           return True
        elif(elem.id_log==id_log):
            return True

    return False
def GetDistanza(position1, position2):
    distanza = abs(float (math.sqrt((float (position1.x)-float (position2.x))**2 + (float (position1.y)- float (position2.y))**2)))
    return distanza
def traiettoriaCoincidente(listReal, listRic):
    print("ENTRAAAA")
    for i in range(0, len( listReal)):
        for j in range(0, len( listRic)):
            if(i==j):
                #print(str(posReal.x)+ " "+str(posReal.y))
                if(listReal[i].x!=listRic[j].x and listReal[i].y!=listRic[j].y ):
                    return False

    return True


def ContaDecisioni(arrayContatori,listaSegmentiReali, listaCluster):
    print(len(listaSegmentiReali))
    print(len(listaCluster))


    for l in listaSegmentiReali:
        print("\n")

        for pos in l:
            print(str(pos.x) + " " + str(pos.y)+" r")
    if(len(listaCluster)>len(listaSegmentiReali)):
        listaClusterNuova=list()
        while(len(listaClusterNuova)!=len(listaSegmentiReali)):
            for listReal in listaSegmentiReali:
                for cluster in listaCluster:

                    if (listReal[len(listReal)-1].x == cluster.lista_posizioni[len(cluster.lista_posizioni)-1].x and listReal[len(listReal)-1].y == cluster.lista_posizioni[len(cluster.lista_posizioni)-1].y):
                        listaClusterNuova.append(cluster)
        for cluster in listaClusterNuova:
            print("\n")

            for pos in cluster.lista_posizioni:
                print(str(pos.x) + " " + str(pos.y) + " fr")
        for listReal in listaSegmentiReali:
            for cluster in listaClusterNuova:

                if (listReal[0].x == cluster.lista_posizioni[0].x and listReal[0].y == cluster.lista_posizioni[0].y):
                    if (traiettoriaCoincidente(listReal, cluster.lista_posizioni) == True):

                        arrayContatori[0] += 1

                    else:

                        arrayContatori[1] += 1

        return arrayContatori
    else:
        for cluster in listaCluster:
            print("\n")

            for pos in cluster.lista_posizioni:
                print(str(pos.x) + " " + str(pos.y) + " fr")
        for listReal in listaSegmentiReali:
            for cluster in listaCluster:

                if(listReal[0].x==cluster.lista_posizioni[0].x and listReal[0].y==cluster.lista_posizioni[0].y):
                    if(traiettoriaCoincidente(listReal, cluster.lista_posizioni)==True):

                        arrayContatori[0] +=1

                    else:

                        arrayContatori[1] += 1

        return arrayContatori



def getIdSegmento(id_log,listaId):
    for elem in listaId:
        if(elem.id_log==id_log):
            return elem.id_seg

def getIdLog(id_segmento,listaId):
    for elem in listaId:
        if(elem.id_seg==id_segmento):
            return elem.id_log



def getPosizioniReali(cursor, incrocio,tipo, id_log):


    listaPos=list()


    t1 = datetime.strptime(incrocio.timestamp, ("%H:%M:%S.%f"))
    t1 -= timedelta(milliseconds=400)
    tm1 = t1.strftime("%H:%M:%S.%f")[:-3]

    t2 = datetime.strptime(incrocio.timestamp, ("%H:%M:%S.%f"))
    t2 -= timedelta(milliseconds=200)
    tm2=t2.strftime("%H:%M:%S.%f")[:-3]

    t3 = datetime.strptime(incrocio.timestamp, ("%H:%M:%S.%f"))

    t3 += timedelta(milliseconds=200)
    tm3 = t3.strftime("%H:%M:%S.%f")[:-3]

    print("TEMPO "+tm1)
    print("TEMPO "+tm2)

    print("TEMPO "+incrocio.timestamp)
    print("TEMPO "+tm3)


    array=cursor.execute("SELECT valore_segmento.posizione, valore_segmento.id_segmento FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore WHERE valore.timestamp_pos = ? OR valore.timestamp_pos = ? OR  valore.timestamp_pos = ? OR  valore.timestamp_pos = ? AND  valore_segmento.id_segmento = ? ;" ,(tm1,incrocio.timestamp, tm2,tm3,id_log,)  ).fetchall()

    for row in array:
        if(row[1]==id_log):
            print(row)
            s = row[0].strip('\n').split(' ')

            pos=Position()

            pos.x=float(s[0].replace(',','.'))
            pos.y=float(s[1].replace(',','.'))

            if(GetDistanza(pos,incrocio.position)<100):
                #print(str(incrocio.position.x) + " " + str(incrocio.position.y)+" "+incrocio.timestamp + " inc")

                #print(str(pos.x) + " " + str(pos.y) + " pos_suc" + tipo)

                listaPos.append(pos)
    return listaPos
def getDuplicates(lista):
    duplicates = []
    for elem in lista:

        if (isContainedDuplicati(lista, elem) > 1):
            duplicates.append(elem)
    return duplicates

def select_authorizer(*args):
    return sqlite3.SQLITE_OK
def isContained(incrocio, lista_incroci):
    for inc in lista_incroci:
        if(inc.position.x==incrocio.position.x and inc.position.y==incrocio.position.y
                and inc.timestamp==incrocio.timestamp):
            return True

    return False

def CreaListaId(listaPosSuccessiveReali, listaPosSuccessiveRicostruite):
    listaId=list()
    for elem in listaPosSuccessiveReali:
        for elemRic in listaPosSuccessiveRicostruite:
            if (elem.x == elemRic.x and elem.y == elemRic.y):
                tupla = Tupla()

                tupla.id_log = elem.id_seg

                tupla.id_seg = elemRic.id_seg
                if (isContainedDuplicatiId(listaId, tupla,tupla.id_log)):
                    continue
                print(str(tupla.id_log)+" "
               +str( tupla.id_seg))
                listaId.append(tupla)

    print(str(len(listaId))+" leee")
    return listaId
def test_kalmanfilter(decisioniTotali,lista_incroci,pathDirectoryLog):
    conn = sqlite3.connect(pathDirectoryLog + '\\realTrajectoriesDB.db')
    conn.set_authorizer(select_authorizer)
    cursor = conn.cursor()

    connDBSim = sqlite3.connect(pathDirectoryLog + '\\trajectoriesDB.db')
    connDBSim.set_authorizer(select_authorizer)
    cursorSim = connDBSim.cursor()
    cursor.execute("SELECT valore_segmento.posizione,valore_segmento.id_valore, valore.timestamp_pos, COUNT(*) count FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore GROUP BY valore_segmento.posizione, valore.timestamp_pos HAVING COUNT(*) > 1"  )





    cr=conn.cursor()
    cs=connDBSim.cursor()
    #devo ottenere e far matchare gli id dei log reali con i segmenti
    cr.execute("SELECT valore.posizione, valore_segmento.id_segmento FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore WHERE valore.timestamp_pos = ? ; ",("00:00:00.000",)  )
    cs.execute("SELECT valore.posizione, valore_segmento.id_segmento FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore WHERE valore.timestamp_pos = ? ; ",("00:00:00.000",)  )
    listaId=list()
    listaReali=list()
    listaRicostruite=list()
    id_count=0
    for row in cr:
        s = row[0]
        pos=Position()

        pos.x = float(s.split(' ')[0])
        pos.y =float( s.split(' ')[1])
        pos.id_seg=int(row[1])
        listaReali.append(pos)

    for rowSim in cs:
        s = rowSim[0]
        pos=Position()

        pos.x = float(s.split(' ')[0])
        pos.y = float(s.split(' ')[1])
        pos.id_seg=int(rowSim[1])
        listaRicostruite.append(pos)

    for elem in listaReali:
        for elemRic in listaRicostruite:
            if(elem.x==elemRic.x and elem.y==elemRic.y ):
                tupla= Tupla()
                tupla.id_log=elem.id_seg
                tupla.id_seg=elemRic.id_seg
                listaId.append(tupla)
                print(str(tupla.id_log)+" "+str(tupla.id_seg))



    listaRicostruite.clear()

    decisioniGiuste=0
    decisioniSbagliate=0
    arrayContatori=[]
    arrayContatori.append(decisioniGiuste)
    arrayContatori.append(decisioniSbagliate)
    flag=False
    listaDuplicati=None
    count=0
    listaDuplicati=[]
    listaSegmentiReali=list()
    for inc in lista_incroci:
        c4 = conn.cursor()
        c5=connDBSim.cursor()
        for id in listaId:
            listaPosReali=getPosizioniReali(c4,inc,"re", id.id_log)

            listaSegmentiReali.append(listaPosReali)



        if (not listaSegmentiReali ):
            break



            flag=False
        print(len( inc.listaCluster))

        arrayContatori=ContaDecisioni(arrayContatori,listaSegmentiReali, inc.listaCluster)
        listaSegmentiReali.clear()

    print(str(len(lista_incroci))+" incroci")

    print(str(arrayContatori[0])+" giuste")
    print(str(arrayContatori[1])+" sbagliate")



    conn.close()