import glob
import errno
import os
import random
import datetime
import random
import math
from datetime import datetime
from datetime import timedelta

from tkinter import messagebox
from tkinter import filedialog
from tkinter import *
import sqlite3
from threading import Thread

import itertools

class B_step:
    segments=[]
    crossing=None
    index_crossing=0

def select_authorizer(*args):
    return sqlite3.SQLITE_OK

class MioThread (Thread):
   def __init__(self, list_logs, log_file, root):
      Thread.__init__(self)
      self.list_logs = list_logs
      self.log_file = log_file
      self.root = root
   def run(self):
       line = self.log_file.readline()
       file_list=[]

       while (line != ""):
           parsedline = line.split('\t')
           file_list.append(parsedline[2]+"_"+parsedline[0] + " " + parsedline[1])

           line = self.log_file.readline()
       self.list_logs.append(file_list)



def translate(s_name):
    path = "C:\\Users\\Dario\\Desktop\\multi-user-segmentation-master\\data\\DatasetPaths_simplified_dict.txt"
    f=open(path,"r")
    line=f.readline()
    while(line!=""):
        parsed_line=line.split("\t\t")
        sensor_name=parsed_line[1].rstrip("\n").strip()
        if(s_name.rstrip("\n").strip()==sensor_name):
            return parsed_line[0].rstrip("\n").strip()

        line=f.readline()

    return None

def find_crossing(log_file, entry,index_crossing, segment):
    sequence=[]
    if(len(segment)<12):
        return None
    for i in range(0, len(log_file)):
        if(log_file[i].strip()==entry.strip() and i>=index_crossing):

            #print(log_file[i].strip())
            #print(entry.strip())


            for j in range(0, index_crossing+1):
                sequence.append(log_file[i-index_crossing+j].split("_")[0])
            for k in range(index_crossing+1, len(segment)):
                if(i+k<len(log_file)):
                    sequence.append( log_file[i+k].split("_")[0])

            return sequence

    return None


def translate_sequence(sequence):
    translated_sequence=""
    for i in range(0, len(sequence)):

        translated_sequence+=translate(sequence[i])


    return translated_sequence


def check_sequence(segment, translated_sequence,index_crossing):
    counter=0
    len1=index_crossing+1
    len2=index_crossing+6

   

    for i in range(index_crossing-5,len1):

            if (segment[i] == translated_sequence[i]):
                counter+=1
    for j in range(index_crossing,len2):
        if (segment[j] == translated_sequence[j]):
           counter+=1



    return counter


def find_segment(segment,crossing,list_log_files, index_crossing):




    for j in range(0, len(list_log_files)):
            sequence=find_crossing(list_log_files[j], crossing, index_crossing, segment)

            if(sequence!=None):
                print(sequence)
                print(segment)
                translated_sequence=translate_sequence(sequence)
                print(translated_sequence)
                print(index_crossing)
                print()

                if (len(translated_sequence) < len(segment)):
                    continue
                if(check_sequence(segment,translated_sequence,index_crossing)>=4):

                    return True

    return False


def read_path_logs():
    root = Tk()
    root.directory = filedialog.askdirectory(initialdir="C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log",
                                             title="Select file")


    path = root.directory + "\\*.txt"

    # lettura dei pathlog della simulazione
    files = glob.glob(path)
    listafile = list()

    for name in files:
        try:
            with open(name) as f:
                f = open(name, "r")
                if(not "DatasetPaths.txt" in name):
                    listafile.append(f)
        except IOError as exc:  # Not sure what error this is
            if exc.errno != errno.EISDIR:
                raise
    list_logs=[]

    for log_file in listafile:
        thread=MioThread(list_logs, log_file, root)
        thread.start()
        thread.join()

    return list_logs

def validate_algorithm():
    list_log_files=read_path_logs()
    conn = sqlite3.connect("C:\\Users\\Dario\\Desktop\\multi-user-segmentation-master\\data\\trajectoriesDB.db")
    conn.set_authorizer(select_authorizer)
    c = conn.cursor()

    array = c.execute("SELECT * FROM segment;").fetchall()
    final_array=[]
    for segment, orders_iter in itertools.groupby(array,lambda x: x[2]):
        b_step = list(orders_iter)
        final_array.append(b_step)
    conn.close()

    correct_decisions=0
    wrong_decisions = 0
    for i in range(0, len(final_array)):
        for j in range(0, len(final_array[i])):
            segment = final_array[i][j][1]
            crossing =  final_array[i][j][3]
            index_crossing = int( final_array[i][j][4])
            if (find_segment(segment, crossing, list_log_files, index_crossing)):
                correct_decisions += 1
                break
            if (j==len(final_array[i])-1 and find_segment(segment, crossing, list_log_files, index_crossing)==False):
                wrong_decisions += 1


    print(correct_decisions)
    print(wrong_decisions)





    return


if __name__ == '__main__':
    validate_algorithm()