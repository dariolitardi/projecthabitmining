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

def GetDistanza(position1, position2):
    distanza = abs(float (math.sqrt((float (position1.x)-float (position2.x))**2 + (float (position1.y)- float (position2.y))**2)))
    return distanza
def ContaDecisioni( arrayContatori, listaId,listaPosSuccessiveRicostruite, listaPosSuccessiveReali):
    print(len(listaPosSuccessiveRicostruite))
    print(len(listaPosSuccessiveReali))

    for pos in listaPosSuccessiveRicostruite:
        for posReal in listaPosSuccessiveReali:

            if(pos.x==posReal.x and pos.y==posReal.y ):
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
def getPosizioniSuccessive(cursor, incrocio,tipo):


    listaPos=list()
    t = datetime.strptime(incrocio.timestamp, ("%H:%M:%S.%f"))
    t += timedelta(milliseconds=1000)
    tm=t.strftime("%H:%M:%S.%f")[:-3]


    array=cursor.execute("SELECT valore_segmento.posizione, valore_segmento.id_segmento FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore WHERE valore.timestamp_pos = ? ;" ,(tm,)  ).fetchall()

    for row in array:

        s = row[0].strip('\n').split(' ')

        pos=Position()

        pos.x=float(s[0].replace(',','.'))
        pos.y=float(s[1].replace(',','.'))
        pos.id_seg= int(row[1])

        if(GetDistanza(pos,incrocio.position)<80):
            print(str(incrocio.position.x) + " " + str(incrocio.position.y)+" "+incrocio.timestamp + " inc")

            print(str(pos.x) + " " + str(pos.y) + " pos_suc" + tipo)

            listaPos.append(pos)
    return listaPos
def select_authorizer(*args):
    return sqlite3.SQLITE_OK
def isContained(incrocio, lista_incroci):
    for inc in lista_incroci:
        if(inc.position.x==incrocio.position.x and inc.position.y==incrocio.position.y
                and inc.timestamp==incrocio.timestamp):
            return True

    return False
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


def test_kalmanfilter(decisioniTotali,lista_incroci,pathDirectoryLog):
    conn = sqlite3.connect(pathDirectoryLog + '\\realTrajectoriesDB.db')
    conn.set_authorizer(select_authorizer)
    cursor = conn.cursor()

    connDBSim = sqlite3.connect(pathDirectoryLog + '\\trajectoriesDB.db')
    connDBSim.set_authorizer(select_authorizer)
    cursorSim = connDBSim.cursor()
    cursor.execute("SELECT valore_segmento.posizione,valore_segmento.id_valore, valore.timestamp_pos, COUNT(*) count FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore GROUP BY valore_segmento.posizione, valore.timestamp_pos HAVING COUNT(*) > 1"  )

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
        incrocio.count=int(row[3])
        if(isContained(incrocio,lista_incroci) and not isContained(incrocio,listaIncroci)):
            listaIncroci.append(incrocio)



    cr=conn.cursor()
    cs=connDBSim.cursor()
    #devo ottenere e far matchare gli id dei log reali con i segmenti
    cr.execute("SELECT valore.posizione, valore_segmento.id_segmento FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore WHERE valore.timestamp_pos = ? ; ",("00:00:00.000",)  )
    cs.execute("SELECT valore.posizione, valore_segmento.id_segmento FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore WHERE valore.timestamp_pos = ? ; ",("00:00:00.000",)  )
    listaId=list()
    listaReali=list()
    listaRicostruite=list()
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
    listaRicostruite.clear()
    listaReali.clear()

    decisioniGiuste=0
    decisioniSbagliate=0
    arrayContatori=[]
    arrayContatori.append(decisioniGiuste)
    arrayContatori.append(decisioniSbagliate)
    for inc in listaIncroci:
        c4 = conn.cursor()
        c5=connDBSim.cursor()
        listaPosSuccessiveReali=getPosizioniSuccessive(c4,inc,"re")

        listaPosSuccessiveRicostruite=getPosizioniSuccessive(c5,inc,"ric")


        if (not listaPosSuccessiveReali or not listaPosSuccessiveRicostruite):
            break

        arrayContatori=ContaDecisioni(arrayContatori, listaId,listaPosSuccessiveRicostruite, listaPosSuccessiveReali)
        listaPosSuccessiveReali.clear()
        listaPosSuccessiveRicostruite.clear()

    print(str(decisioniTotali)+" decisioni")

    print(str(arrayContatori[0])+" giuste")
    print(str(arrayContatori[1])+" sbagliate")



    conn.close()