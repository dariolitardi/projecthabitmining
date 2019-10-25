from pykalman import KalmanFilter
import numpy as np
import matplotlib.pyplot as plt
import time
from numpy import *
from numpy.linalg import inv
from numpy  import array
from filterpy.common import Q_discrete_white_noise


import pylab as pl

class Position:
    x = 0
    y = 0
class Cluster:
    lista_posizioni=None
    id_segment=0
import math

def GetDistanza(position1, position2):
    distanza = abs(float (math.sqrt((float (position1.x)-float (position2.x))**2 + (float (position1.y)- float (position2.y))**2)))
    return distanza
def kalman_filter_position(cluster,position):
    c=Cluster()
    c.lista_posizioni=cluster.lista_posizioni
    c.id_segment=cluster.id_segment
    c.lista_posizioni.append(position)
    a= list()
    #print(len(c.lista_posizioni))
    for ps in c.lista_posizioni:
        tup1 =(ps.x,ps.y)
        #print(tup1)

        a.append(tup1)


    measurements = np.asarray(a)


    initial_state_mean = [measurements[0, 0],
                        0,
                        measurements[0, 1],
                        0]

    transition_matrix = [[1, 1, 0, 0],
                     [0, 1, 0, 0],
                     [0, 0, 1, 1],
                     [0, 0, 0, 1]]

    observation_matrix = [[1, 0, 0, 0],
                      [0, 0, 1, 0]]

    time_before = time.time()
    n_real_time = 3
    observation_covariance =[

                     [ 100, 0],
                     [ 0, 100]]
    kf3 = KalmanFilter(transition_matrices = transition_matrix,
                  observation_matrices = observation_matrix,

                  initial_state_mean = initial_state_mean,
                       observation_covariance= observation_covariance,
                       em_vars=['transition_covariance', 'initial_state_covariance']
               )
    means, covariances = kf3.filter(measurements)


    kf3 = kf3.em(measurements, n_iter=8)
    (filtered_state_means, filtered_state_covariances) = kf3.filter(measurements)
    #print("Time to build and train kf3: %s seconds" % (time.time() - time_before))
    n_timesteps = 4
    n_dim_state =  4
    filtered_state_means2 = np.zeros((n_timesteps, n_dim_state))
    filtered_state_covariances2 = np.zeros((n_timesteps, n_dim_state, n_dim_state))
    i=0
    for t in range(0,4):
        #print("ciao")

        if t == 0:
            filtered_state_means2[t] = kf3.initial_state_mean
            filtered_state_covariances2[t] =  kf3.initial_state_covariance

        if(t+1<4):

            filtered_state_means2[t + 1], filtered_state_covariances2[t + 1] = (
            kf3.filter_update(
                means[-1],
            covariances[-1],
             measurements[t+1]

                )
            )
        #print(filtered_state_means2[i])

        i = i + 1
    pos_calcolata=Position()
    pos_calcolata.x=filtered_state_means2[3][0]
    pos_calcolata.y=filtered_state_means2[3][2]
    #print(str(position.x)+" "+str(position.y)+"posvera")

    #print(str(pos_calcolata.x)+" "+str(pos_calcolata.y)+"poscalco")

    distanza= GetDistanza(pos_calcolata,position)
    # draw estimates
    '''''
    pl.figure()
    lines_true = pl.plot(measurements, 'bo')
    lines_filt = pl.plot(filtered_state_means2, 'r--')
    pl.legend((lines_true[0], lines_filt[0]), ('true', 'filtered'))
    pl.show()'''
    return distanza
