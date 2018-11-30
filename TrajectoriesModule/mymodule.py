import glob
import errno
import os
import random
import datetime
import random
import math
from datetime import timedelta

class Position:
     x = 0
     y = 0

def goto(linenum):
        global line
        line = linenum

def CalcolaProdottoCartesiano(listaPosizioni1, listaPosizioni2):
    prodotto='{'
    for i in range(0,len(listaPosizioni1)):
        for j in range(0, len(listaPosizioni2)):
            if(i==len(listaPosizioni1)-1 and j==len(listaPosizioni2)-1):
                prodotto += "{" + "(" + listaPosizioni1[i].x + "," + listaPosizioni1[j].y + "),(" + listaPosizioni2[j].x + "," + listaPosizioni1[i].y + ")}"
            else:
                prodotto+="{"+"("+listaPosizioni1[i].x+","+listaPosizioni1[j].y+"),("+listaPosizioni2[j].x+","+listaPosizioni1[i].y+")}}"
    return prodotto

def MioMetodo(f):
    nuovalista=[]
    c=0
    for line in f:
        if (c ==86300):
            break

        vettorelinea = line.split(' ')
        position = Position()
        position.x = vettorelinea[1].rstrip()
        position.y = vettorelinea[2].rstrip()

        nuovalista.append(position)
        c += 1

    return nuovalista

def GetDistanza(position1, position2):
    distanza = int (math.sqrt((int (position1.x)-int (position2.x))**2 + (int (position1.y)- int (position2.y))**2))
    return distanza

def CalcolaPosizioneVicina(position, listaposizioni):
    distanza= GetDistanza(position,listaposizioni[0])
    posizionevicina=listaposizioni[0]
    for k in range(1, len(listaposizioni)):
        if(distanza> GetDistanza(position,listaposizioni[k])):
            distanza=GetDistanza(position,listaposizioni[k])
            posizionevicina=listaposizioni[k]

    return posizionevicina

def main():
    some_list = {}
    path= "C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\*.txt"
    i=1
    c=0

    #lettura dei pathlog della simulazione
    files = glob.glob(path)

    mydate = datetime.datetime(datetime.datetime.now().year, datetime.datetime.now().month,
                           datetime.datetime.now().day,0,0,0)
    posizioni=[]
    some_list[mydate.strftime("%H:%M:%S")]=None


    j=1
    while(j<86300):
        mydate = mydate + timedelta(seconds=1)
        some_list[mydate.strftime("%H:%M:%S")] = None

        j+=1


    lista=[]


    for name in files:
        try:
            with open(name) as f:
                lista.append(MioMetodo(f))

        except IOError as exc:  # Not sure what error this is
            if exc.errno != errno.EISDIR:
                raise



    j = 0
    while (j < 86300):
        if(j==0):
            mydate = datetime.datetime(datetime.datetime.now().year, datetime.datetime.now().month,
                                       datetime.datetime.now().day, 0, 0, 0)
        else:
            mydate = mydate + timedelta(seconds=1)

        k=random.randint(0, len(lista)-1)
        pos=[]
        pos.append(lista[k][j])

        if(k==1):
            pos.append(lista[0][j])

        else:
            pos.append(lista[1][j])
        some_list[mydate.strftime("%H:%M:%S")]=pos

        j += 1

    print(some_list["00:00:00"][0].x+" "+some_list["00:00:00"][0].y )
    print(some_list["00:00:00"][1].x+" "+some_list["00:00:00"][1].y )


    mappacalcolata={}

    mydate = datetime.datetime(datetime.datetime.now().year, datetime.datetime.now().month,
                               datetime.datetime.now().day, 0, 0, 0)
    posizionimappa = []
    mappacalcolata[mydate.strftime("%H:%M:%S")] = None

    j = 1
    while (j < 86300):
        mydate = mydate + timedelta(seconds=1)
        mappacalcolata[mydate.strftime("%H:%M:%S")] = None

        j += 1
    lista1=[]

    for h in range(0,len(some_list[("00:00:00")])):
        lista1.append(some_list[("00:00:00")][h])

    mappacalcolata[("00:00:00")]=lista1



    print(len(some_list[("00:00:00")]))
    j = 0
    h=0
    while (j < 86300):
        if(j==0):
            mydate = datetime.datetime(datetime.datetime.now().year, datetime.datetime.now().month,
                                       datetime.datetime.now().day, 0, 0, 0)
        else:
            mydate = mydate + timedelta(seconds=1)
        lista2 = []
        for m in range(0, len(some_list[mydate.strftime("%H:%M:%S")])):
            mydate2 = mydate + timedelta(seconds=1)
            if(mydate2.strftime("%H:%M:%S") not in some_list):
                break
            posizionevicina = CalcolaPosizioneVicina(some_list[mydate.strftime("%H:%M:%S")][m],some_list[mydate2.strftime("%H:%M:%S")])

            lista2.append(posizionevicina)
            h+=1
        mappacalcolata[mydate2.strftime("%H:%M:%S")]=(lista2)

        j += 1


    ##scrive i dati su file
    pathDataSetFile= "C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\DatasetPaths.txt"

    datasetfile= open(pathDataSetFile,"a+")

    j = 0
    s=''
    prodotto=''
    while (j < 86300):
        if(j==0):
            mydate = datetime.datetime(datetime.datetime.now().year, datetime.datetime.now().month,
                                       datetime.datetime.now().day, 0, 0, 0)
        else:
            mydate = mydate + timedelta(seconds=1)

        s += mydate.strftime("%H:%M:%S")
        if (mydate.strftime("%H:%M:%S") == "23:58:20"):
            return
        for j in range(0, len(some_list[mydate.strftime("%H:%M:%S")])):
            if(some_list[mydate.strftime("%H:%M:%S")][0].x==some_list[mydate.strftime("%H:%M:%S")][1].x
            and some_list[mydate.strftime("%H:%M:%S")][0].y==some_list[mydate.strftime("%H:%M:%S")][1].y):
                if(mydate.strftime("%H:%M:%S")!="00:00:00" and mydate.strftime("%H:%M:%S")!="23:58:19"):
                    timestampPrecedente = mydate- timedelta(seconds=1)
                    timestampSuccessivo=mydate+ timedelta(seconds=1)



                    prodotto=CalcolaProdottoCartesiano(some_list[timestampPrecedente.strftime("%H:%M:%S")], some_list[timestampSuccessivo.strftime("%H:%M:%S")])

            s +=  " "+mappacalcolata[mydate.strftime("%H:%M:%S")][j].x +" "+ mappacalcolata[mydate.strftime("%H:%M:%S")][j].y

        #print (s)
        datasetfile.write(s+" " +prodotto+"\n")
        s =''
        prodotto=''
        j+=1




if __name__=="__main__":
    main()


