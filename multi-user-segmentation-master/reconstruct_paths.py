import os
import string
import sqlite3
def select_authorizer(*args):
    return sqlite3.SQLITE_OK
class SensorLog:
    b_steps=[]
class B_step:
    closed_segments=[]
    compat_segments=[]
    crossing_points=[]

class Joined_Segment:
    id_closed=0
    id_compat=0
    average=0
    segment=None
    subsequence=None
    index_crossing=0
    position_crossing=None
def insert_in_database(b_set,id, c):



    for i in range(0, len(b_set)):
        joined_segment=b_set[i]
        seq = joined_segment.segment
        index=joined_segment.index_crossing
        pos = joined_segment.position_crossing




        c.execute("INSERT INTO segment VALUES (?, ?, ?, ?, ?);", ( None,seq, id, pos, index))

    return



def construct_database(final_array_sub_seq,pathDirectoryLog):
    exists = os.path.isfile(
        (pathDirectoryLog ))
    if exists:
        os.remove(pathDirectoryLog )
    ##creo il database dove andrò a salvare i vari log
    conn = sqlite3.connect(pathDirectoryLog)
    conn.set_authorizer(select_authorizer)
    c = conn.cursor()

    # Create tables
    c.execute('''CREATE TABLE segment(id_segment INTEGER PRIMARY KEY AUTOINCREMENT, sequence TEXT ,id_b_step INTEGER, crossing_positions TEXT, index_crossing INTEGER)''')


    # Save (commit) the changes
    conn.commit()






    for i in range(0,len(final_array_sub_seq)):
        insert_in_database(final_array_sub_seq[i],i, c)

    conn.commit()

    conn.close()

def is_contained(i, segment,sub_sequence ):
    flag=False
    if(i<len(segment)):
        if (sub_sequence[0] == segment[ i][0] ):
            if( i+1<len(segment)):
                if (sub_sequence[1] == segment[ i+1][0]):
                    if(i+2<len(segment)):
                        if (sub_sequence[2] == segment[ i+2][0]):
                            if (i + 3 < len(segment)):
                                if (sub_sequence[3] == segment[i + 3][0]):
                                    if (i + 4 < len(segment)):
                                        if (sub_sequence[4] == segment[i + 4][0]):
                                            if (i + 5 < len(segment)):
                                                if (sub_sequence[5] == segment[i + 5][0]):
                                                    flag = True

    return flag


def find_subsequence(segment, sub_sequence):
    c = 0

    for i in range(0, len(segment)):

        if(is_contained(i, segment, sub_sequence)==True):
            c+=1


    return c


def calculate_max_averages(joined_segment, b_step):
    max_joined=b_step[0]
    for i in range(1, len(b_step)):
        if(b_step[i].id_closed==joined_segment.id_closed):
            if(max_joined.average<=b_step[i].average):
                max_joined=b_step[i]

    return max_joined

def calculate_averages(subsequence, sensor_log):
    count_occurences=0
    total=0
    average=0
    for i in range(0, len(sensor_log.b_steps)):

        for j in range(0, len(sensor_log.b_steps[i].closed_segments)):
            count_occurences+=sensor_log.b_steps[i].closed_segments[j].count(subsequence)
            if(sensor_log.b_steps[i].closed_segments[j].count(subsequence)>0):
                total+=1
        for m in range(0, len(sensor_log.b_steps[i].compat_segments)):
            count_occurences += sensor_log.b_steps[i].compat_segments[m].count(subsequence)
            if (sensor_log.b_steps[i].compat_segments[m].count(subsequence) > 0):
                total += 1

    if(count_occurences==0):
        return 0
    average= count_occurences


    return average

def get_crossing_point(array, point):
    for i in range(0, len(array)):

        if(array[i].split("_")[0]==point):

            my_array = array[i].split("_")

            sensor_name = translate(my_array[0])

            timestamp = translate_timestamp(int(my_array[1]))
            #print(sensor_name)
            if (sensor_name != None and timestamp != None):
                return sensor_name+"_"+timestamp
    return None




def get_indexes(array):
    c=0
    seq=array[0][6]
    for i in range(1, len(array)):
        if(seq!=array[i][6]):
            seq=array[i][6]
            c += 1

    return c
def isContained(array, sensor):
    for i in range(0, len( array)):
        if(array[i][0][0]==sensor):
            return True
    return False

def translate_timestamp(n_line):
    path = "C:\\Users\\Dario\\Desktop\\multi-user-segmentation-master\\data\\DatasetPaths.txt"
    f = open(path, "r")
    line = f.readline()
    if(line==""):
        return None
    counter=1
    while (line != ""):
        if(n_line==counter):
            parsed_line=line.split("\t")

            timestamp=parsed_line[0]+" "+parsed_line[1]
            return timestamp

        line = f.readline()
        counter+=1

    return None


def translate(my_char):
    path = "C:\\Users\\Dario\\Desktop\\multi-user-segmentation-master\\data\\DatasetPaths_simplified_dict.txt"
    f=open(path,"r")
    line=f.readline()
    while(line!=""):
        parsed_line=line.split("\t\t")
        symbol_dict=parsed_line[0].strip()
        if(symbol_dict==str(my_char).strip()):

            return parsed_line[1].strip()

        line=f.readline()

    return None


def max_is_contained(max_joined_segment,b_step):
    for i in range(0, len(b_step)):
        if(b_step[i].id_closed==max_joined_segment.id_closed ):
            return True
    return False

def reconstruct(ssl):
    array_sub_seq=[]
    sensor_log= SensorLog()


    for i in range(0, len(ssl.b_steps)):

        b_step= B_step()

        b_step.closed_segments=[]
        b_step.compat_segments=[]

        for j in range(0, len(ssl.b_steps[i].closed_segments)):
            if (len(ssl.b_steps[i].closed_segments[j]) <= 4  ):
                continue
            s = ""

            for h  in range(0,len(ssl.b_steps[i].closed_segments[j])):
                s+=ssl.b_steps[i].closed_segments[j][h][0]

            b_step.closed_segments.append(s)

        for m in range(0, len(ssl.b_steps[i].compat_segments)):
            if(len(ssl.b_steps[i].compat_segments[m])<=4  ):
                continue

            x = ""

            for u in range(0, len(ssl.b_steps[i].compat_segments[m])):
                x+=ssl.b_steps[i].compat_segments[m][u][0]


            b_step.compat_segments.append(x)
            b_step.crossing_points=ssl.b_steps[i].crossing_points
            #print(  b_step.compat_segments)

        sensor_log.b_steps.append(b_step)




    final_array=[]
    
    for i in range(0, len(sensor_log.b_steps)):

        b_step=[]

        for j in range(0, len(sensor_log.b_steps[i].closed_segments)):

            for m in range(0, len(sensor_log.b_steps[i].compat_segments)):
                if(len(sensor_log.b_steps[i].closed_segments[j]+sensor_log.b_steps[i].compat_segments[m])!=0):
                    joined_segment=Joined_Segment()
                    joined_segment.id_closed=j
                    joined_segment.id_compat=m
                    joined_segment.subsequence=sensor_log.b_steps[i].closed_segments[j][-3:]+sensor_log.b_steps[i].compat_segments[m][:3]

                    joined_segment.segment=sensor_log.b_steps[i].closed_segments[j]+sensor_log.b_steps[i].compat_segments[m]
                    joined_segment.index_crossing=len(sensor_log.b_steps[i].closed_segments[j])-1
                    joined_segment.position_crossing=get_crossing_point(sensor_log.b_steps[i].crossing_points, sensor_log.b_steps[i].compat_segments[m]
                    [:1])
                    b_step.append( joined_segment)
        if( b_step):
            final_array.append(b_step)
    print(len(final_array))

    array_joined_segments=[]
    for i in range(0, len(final_array)):
        for j in range(0, len(final_array[i])):
            joined_segment=final_array[i][j]
            joined_segment.occurences=calculate_averages(joined_segment.subsequence, sensor_log)

    for i in range(0, len(final_array)):
        b_step=[]
        for j in range(0, len(final_array[i])):
            joined_segment = final_array[i][j]
            max_joined_segment=calculate_max_averages(joined_segment,final_array[i])
            if(max_is_contained(max_joined_segment, b_step)==False):

                b_step.append(max_joined_segment)
        array_joined_segments.append(b_step)
    for i in range(0, len(array_joined_segments)):
        print(i)

        for j in range(0, len(array_joined_segments[i])):
            joined_segment = array_joined_segments[i][j]

            print(joined_segment.segment)
            print(joined_segment.id_closed)
            print(joined_segment.position_crossing)


    print(len(final_array))

    print(len(array_joined_segments))


    construct_database(array_joined_segments,"C:\\Users\\Dario\\Desktop\\multi-user-segmentation-master\\data\\trajectoriesDB.db")




    return




