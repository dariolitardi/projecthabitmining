import pickle
import gzip
from sklearn.externals import joblib

if __name__ == '__main__':
    # save data to a file

    clf = joblib.load( "C:\\Users\\Dario\\Desktop\\multi-user-segmentation-master\\trained_classifiers\\1554204086.pkl",'r')

    print('\n')



