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
def ContaDecisioni( arrayContatori, listaId,listaPosSuccessiveRicostruite, listaPosSuccessiveReali):
    for pos in listaPosSuccessiveRicostruite:
        for posReal in listaPosSuccessiveReali:
            if(pos.x==posReal.x and pos.y==posReal.y):

                if(pos.id_seg==getIdLog(pos.id_seg,listaId)):
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
def getPosizioniSuccessive(cursor, incrocio):
    m=incrocio.id_val+1
    n=incrocio.id_val+incrocio.count+1
    listaPos=list()
    for i in range(m, n):
        row=cursor.execute("SELECT valore_segmento.posizione, valore_segmento.id_segmento FROM valore_segmento WHERE valore_segmento.id_valore = ? ;" ,(i,)  ).fetchone()
        pos=Position()
        s=row[0].split(' ')
        pos.x=float(s[0])
        pos.y=float(s[1])
        pos.id_seg= int(row[1])

        listaPos.append(pos)
    return listaPos
def select_authorizer(*args):
    return sqlite3.SQLITE_OK
class Position:
    x = 0
    y = 0
    id_seg=0

class Incrocio:
    position=None
    timestamp=None
    id_val=0
    count=0
class Tupla:
    id_seg=0
    id_log=0


def test_kalmanfilter(pathDirectoryLog):
    conn = sqlite3.connect(pathDirectoryLog + '\\realTrajectoriesDB.db')
    conn.set_authorizer(select_authorizer)
    cursor = conn.cursor()

    connDBSim = sqlite3.connect(pathDirectoryLog + '\\trajectoriesDB.db')
    connDBSim.set_authorizer(select_authorizer)
    cursorSim = connDBSim.cursor()

    cursor.execute("SELECT valore_segmento.posizione,valore_segmento.id_valore, valore.timestamp_pos, COUNT(*) count FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore GROUP BY valore_segmento.posizione HAVING COUNT(*) > 1"  )

    decisioniTotaliGiuste=0
    listaIncroci=list()
    for row in cursor:
        incrocio=Incrocio()
        pos=Position()
        s=row[0]
        pos.x=float(s.split(' ')[0])
        pos.y=float(s.split(' ')[1])

        incrocio.position=pos
        incrocio.timestamp=row[2]
        incrocio.id_val=int(row[1])
        decisioniTotaliGiuste+=int(row[3])
        incrocio.count=int(row[3])
        listaIncroci.append(incrocio)

    print(str(decisioniTotaliGiuste)+" decisioni totali")

    #devo ottenere e far matchare gli id dei log reali con i segmenti

    cursor.execute("SELECT valore.posizione, valore_segmento.id_segmento FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore WHERE valore.timestamp_pos = ? ; ",("00:00:00.000",)  )
    cursorSim.execute("SELECT valore.posizione, valore_segmento.id_segmento FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore WHERE valore.timestamp_pos = ? ; ",("00:00:00.000000",)  )
    listaId=list()
    listaReali=list()
    listaRicostruite=list()
    for row in cursor:
        s = row[0]
        pos=Position()

        pos.x = float(s.split(' ')[0])
        pos.y =float( s.split(' ')[1])
        pos.id_seg=int(row[1])
        listaReali.append(pos)


    for rowSim in cursorSim:
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
    listaRicostruite.clear()
    listaReali.clear()

    decisioniGiuste=0
    decisioniSbagliate=0
    arrayContatori=[]
    arrayContatori.append(decisioniGiuste)
    arrayContatori.append(decisioniSbagliate)

    for inc in listaIncroci:
        listaPosSuccessiveReali=getPosizioniSuccessive(cursor,inc)
        listaPosSuccessiveRicostruite=getPosizioniSuccessive(cursorSim,inc)


        arrayContatori=ContaDecisioni(arrayContatori, listaId,listaPosSuccessiveRicostruite, listaPosSuccessiveReali)


    print(str(arrayContatori[0])+" giuste")
    print(str(arrayContatori[1])+" sbagliate")



    conn.close()