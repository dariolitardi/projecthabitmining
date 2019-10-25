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
class Position:
    x = 0
    y = 0
    timestamp=None


class Incrocio:
    position=None
    timestamp=None



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
    for i in range(0, len( listReal)):
                if(listReal[i].x!=listRic[i].x and listReal[i].y!=listRic[i].y ):
                    return False


    return True


def get_posizioni(cursor, incrocio,list_pos):




    t1 = datetime.strptime(incrocio.timestamp, ("%H:%M:%S.%f"))
    t1 -= timedelta(milliseconds=1000)
    tm1 = t1.strftime("%H:%M:%S.%f")[:-3]

    t2 = datetime.strptime(incrocio.timestamp, ("%H:%M:%S.%f"))
    t2 -= timedelta(milliseconds=1000)
    tm2=t2.strftime("%H:%M:%S.%f")[:-3]

    t3 = datetime.strptime(incrocio.timestamp, ("%H:%M:%S.%f"))

    t3 += timedelta(milliseconds=1000)
    tm3 = t3.strftime("%H:%M:%S.%f")[:-3]
    '''
    print("TEMPO "+tm1)
    print("TEMPO "+tm2)

    print("TEMPO "+incrocio.timestamp)
    print("TEMPO "+tm3)
    '''

    array=cursor.execute("SELECT valore.posizione FROM valore WHERE valore.timestamp_pos = ? ;" ,(tm1,)  ).fetchall()
    for i in range(0, len(array)):
        list_sequence=list()
        list_pos.append(list_sequence)
    for j in range(0, len(array)):
        s= array[j]
        pos1=Position()
        pos1.x=s[0].split(' ')[0]
        pos1.y=s[0].split(' ')[1]
        pos1.timestamp=tm1
        if(j<len(list_pos)):

            list_pos[j].append(pos1)

    array = cursor.execute("SELECT valore.posizione FROM valore WHERE valore.timestamp_pos = ? ;", (tm2,)).fetchall()
    for j in range(0, len(array)):
        s = array[j]
        pos2 = Position()
        pos2.x = s[0].split(' ')[0]
        pos2.y = s[0].split(' ')[1]
        pos2.timestamp = tm2
        if(j<len(list_pos)):

            list_pos[j].append(pos2)

    array = cursor.execute("SELECT valore.posizione FROM valore WHERE valore.timestamp_pos = ? ;", (incrocio.timestamp,)).fetchall()
    for j in range(0, len(array)):
        s = array[j]
        pos_incrocio = Position()

        pos_incrocio.x = s[0].split(' ')[0]
        pos_incrocio.y = s[0].split(' ')[1]
        pos_incrocio.timestamp = incrocio.timestamp
        if(j<len(list_pos)):

            list_pos[j].append(pos_incrocio)



    array = cursor.execute("SELECT valore.posizione FROM valore WHERE valore.timestamp_pos = ? ;", (tm3,)).fetchall()
    for j in range(0, len(array)):
        s = array[j]
        pos3 = Position()

        pos3.x = s[0].split(' ')[0]
        pos3.y = s[0].split(' ')[1]
        pos3.timestamp = pos3.timestamp
        if(j<len(list_pos)):
            list_pos[j].append(pos3)
    return list_pos
def check_sequence(real_sequence, ric_sequence):
    if(len(real_sequence)!=len(ric_sequence)):
        return False
    for i in range(0, len( real_sequence)):

        if(real_sequence[i].x!=ric_sequence[i].x and
                real_sequence[i].y != ric_sequence[i].y and real_sequence[i].timestamp!=ric_sequence[i].timestamp ):
            return False

    return True

def count_decisions(list_real_pos, ric_sequence,arrayContatori):

    for real_sequence in list_real_pos:
            if(check_sequence(real_sequence, ric_sequence)):

                arrayContatori[0]+=1
                return

    arrayContatori[1] += 1


    return
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

def test_kalmanfilter(lista_incroci,pathDirectoryLog):
    conn_real = sqlite3.connect(pathDirectoryLog + '\\realTrajectoriesDB.db')
    conn_real.set_authorizer(select_authorizer)
    cursor_real = conn_real.cursor()

    conn_ric = sqlite3.connect(pathDirectoryLog + '\\trajectoriesDB.db')
    conn_ric.set_authorizer(select_authorizer)
    cursor_ric = conn_ric.cursor()
    decisioniGiuste=0
    decisioniSbagliate=0
    contatoreIncroci=0

    arrayContatori=[]
    arrayContatori.append(decisioniGiuste)
    arrayContatori.append(decisioniSbagliate)
    arrayContatori.append(contatoreIncroci)

    #devo ottenere e far matchare gli id dei log reali con i segmenti
    for elem in lista_incroci:
        list_real_pos=list()
        list_ric_pos=list()
        arrayContatori[2]+=1
        get_posizioni(cursor_real,elem, list_real_pos)
        get_posizioni(cursor_ric,elem, list_ric_pos)
        for ric_sequence in list_ric_pos:
            count_decisions(list_real_pos, ric_sequence, arrayContatori)

        list_real_pos.clear()
        list_ric_pos.clear()




    print(str(arrayContatori[2])+" incroci")

    print(str(arrayContatori[0])+" giuste")
    print(str(arrayContatori[1])+" sbagliate")

    conn_real.close()
    conn_ric.close()