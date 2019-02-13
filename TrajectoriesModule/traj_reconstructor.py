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
import performance_test


class Position:
    x = 0
    y = 0
class Cluster:
    lista_posizioni=None
    id_segment=0
    isAssigned=False

class PositionCrossing:
    distanza=0
    id_segment=0
    position=None

def SfoltisciLista(lista,positionCrossing):
    listaAppoggio=list()
    print(str(positionCrossing.position.x )+" "+str(positionCrossing.position.y )+" "+str(positionCrossing.distanza ))

    for elem in lista:
        if (elem.position.x!=positionCrossing.position.x or
                elem.position.y != positionCrossing.position.y  ):
            if (elem.id_segment != positionCrossing.id_segment):
                print("ciaone")
                listaAppoggio.append(elem)
    return listaAppoggio



def GetPuntoPiuVicino(cluster, listaPunti, lista):

    for pos in listaPunti:
        distanza = kalman_filter.kalman_filter_position(cluster, pos)

        p=PositionCrossing()
        p.distanza=distanza
        p.id_segment=cluster.id_segment
        p.position=pos
        #print(str(pos.x)+" "+str(pos.y)+" "+str(distanza)+"c"+str(cluster.id_segment))
        lista.append(p)

    return


def GetDistanzaMinima(lista_segmenti, pos):
    linea=lista_segmenti[0][len(lista_segmenti[0])-1]
    parsedline = linea.split(' ')
    p = Position()
    j = 0
    p.x = float(parsedline[1].replace(',', '.'))
    p.y = float(parsedline[2].replace(',', '.'))
    distanza=GetDistanza(p,pos)

    for i in range(1,len(lista_segmenti)):
        linea = lista_segmenti[i][len(lista_segmenti[i]) - 1]
        parsedline = linea.split(' ')
        ps = Position()
        ps.x = float(parsedline[1].replace(',', '.'))
        ps.y = float(parsedline[2].replace(',', '.'))
        d2=GetDistanza(ps, pos)
        if(d2<10 or distanza<10):
            continue
        if(d2<distanza ):
            p=ps
            distanza=GetDistanza(p,pos)
            j=i

    return j
def EliminaVectorDaiCluster(lista_clusters,fv):
    for cluster in lista_clusters:
        for v in cluster:
            if(v.isCentroid==False and fv.position.x==v.position.x and fv.position.y==v.position.y):

                cluster.remove(v)
def setFeatureVectorsInCluster(posincrocio,centroide, listaPunti, cluster):

    for feature_vector in listaPunti:

        d = GetDistanza(centroide.position, feature_vector.position)
        if(d>=70):
            continue
        angolo=int(GetAngolo(posincrocio,centroide.position,feature_vector.position))
        if(angolo<=45):

            feature_vector.angle=angolo

            feature_vector.distance=GetDistanza(centroide.position,feature_vector.position)


            cluster.append(feature_vector)
            print(str(feature_vector.position.x) + " " + str(feature_vector.position.y) + "fv")


def GetCentroid(cluster):
    for feature_vector in cluster:
        if(feature_vector.isCentroid==True):
            return feature_vector

def GetAngolo(posincrocio, poscentroide,pos):
    ipotenusa= GetDistanza(pos,posincrocio)
    cateto=GetDistanza(posincrocio,poscentroide)

    if(cateto<ipotenusa):
        angolo=(math.acos(cateto/ipotenusa)*180)/math.pi
    else:
        angolo=(math.acos(ipotenusa/cateto)*180)/math.pi

    return angolo
def GetPuntiDopoIncrocio(puntoincrocio,tmincrocio, f,j,lista_clusters):
    listaPunti=list()
    c=0
    while(True):
        c+=1
        if(len(lista_clusters)==len(listaPunti)):
            return listaPunti
        linea= f.readline()
        j+=1
        parsedline= linea.split(' ')
        pos = Position()
        pos.x = float(parsedline[1].replace(',', '.'))
        pos.y = float(parsedline[2].replace(',', '.'))
        distanza = GetDistanza(pos, puntoincrocio)
        #print(linea+" ciao "+str(distanza))

        timestampPos = parsedline[0]
        if (distanza <= 71 and timestampPos != tmincrocio and distanza>1):


            listaPunti.append(pos)



def CalcolaAngolo(position1, position2):
    m=(position2.y-position1.y)/(position2.x-position1.x)
    angolo=(math.atan(m)*180)/math.pi
    return angolo
def GetDistanza(position1, position2):
    distanza = abs(float (math.sqrt((float (position1.x)-float (position2.x))**2 + (float (position1.y)- float (position2.y))**2)))
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
    incrocio=False
    flag=False

    lista_clusters=list()
    index=0
    listaPunti=None
    puntoIncrocio=Position()
    timestampIncrocio=""
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
            if(distanza>71):
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
            performance_test.test_kalmanfilter(pathDirectoryLog)
            return





        parsedline=linea.split(' ')
        p=Position()
        p.x=float(parsedline[1].replace(',','.'))
        p.y=float(parsedline[2].replace(',','.'))
        timestamp=parsedline[0]
        for i in range(0,len(listasegmenti)):
            s=listasegmenti[i][len(listasegmenti[i])-1]
            #print(len(listasegmenti))

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
                t0=datetime.strptime(parsed[0],("%H:%M:%S.%f"))

                parsed = listasegmenti[i][len(listasegmenti[i]) - 3].split(' ')
                lastPosition1 = Position()
                lastPosition1.x = float(parsed[1].replace(',', '.'))
                lastPosition1.y = float(parsed[2].replace(',', '.'))
                t1 = datetime.strptime(parsed[0], ("%H:%M:%S.%f"))
                cluster=Cluster()
                cluster.lista_posizioni=list()
                cluster.lista_posizioni.append(lastPosition1)
                cluster.lista_posizioni.append(lastPosition0)
                cluster.lista_posizioni.append(pos)
                cluster.id_segment=i
                lista_clusters.append(cluster)
                puntoIncrocio=pos
                timestampIncrocio=timestampPos
                tm=datetime.strptime(timestampPos,("%H:%M:%S.%f"))
                print (str(cluster.id_segment)+"id")

                print (str(len(cluster.lista_posizioni))+"len")
                print (str(len(lista_clusters))+"len2")

                #print(str(pos.x)+" "+str(pos.y)+"c")
                incrocio=True

                index = i



            if(distanza<=71 and timestampPos!=timestamp ):
                if(incrocio):

                    k=GetDistanzaMinima(listasegmenti, p)
                    listasegmenti[k].append(linea)

                    index = k
                    flag = True
                else:
                        listasegmenti[i].append(linea)

                        index = i
                        flag = True


            if(flag==False and i == len(listasegmenti) - 1):
                #print("ciaoo")
                segmentonuovo = list()
                segmentonuovo.append(linea)
                listasegmenti.append(segmentonuovo)
                flag=False
            if(i == len(listasegmenti) - 1):
                if (not lista_clusters):
                    continue
                incrocio = True
                listaPunti = GetPuntiDopoIncrocio(puntoIncrocio, timestampIncrocio, f, j, lista_clusters)
                print(str(len(listaPunti))+"lenp")
                listaPC=[]
                fi=0
                for m in range(0,len(listaPunti)):
                    for n in range(0,len(lista_clusters)):

                        d=kalman_filter.kalman_filter_position(lista_clusters[n],listaPunti[m])
                        pc=PositionCrossing()
                        pc.id_segment=lista_clusters[n].id_segment
                        pc.position=listaPunti[m]
                        pc.distanza=d
                        listaPC.append(pc)

                        fi+=1

                for gi in range(0,len(listaPC)):
                    print(str(listaPC[gi].distanza) + " " + str(listaPC[gi].id_segment) + " " +
                          str(listaPC[gi].position.x) + " " + str(listaPC[gi].position.y))
                while(listaPC):
                    pMin=listaPC[0]
                    for g in range(1,len(listaPC)):
                        if(listaPC[g].distanza<pMin.distanza):
                            pMin=listaPC[g]

                    #print( str(pMin.position.x) + " " + str(pMin.position.y)+" "+str(pMin.distanza))
                    t = datetime.strptime(timestampIncrocio, ("%H:%M:%S.%f"))
                    t += timedelta(seconds=1)
                    ##qui sotto assegno il feature vector al segmento giusto
                    listasegmenti[pMin.id_segment].append(
                        t.strftime("%H:%M:%S.%f") + " " + str(pMin.position.x) + " " + str(pMin.position.y))
                    #print(str(pMin.position.x)+" "+ str(pMin.position.y) +" "+str(pMin.id_segment))
                    listaPC=SfoltisciLista(listaPC, pMin)

                    print(str(len(listaPC))+"lenpc")


                listaPunti.clear()
                lista_clusters.clear()














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