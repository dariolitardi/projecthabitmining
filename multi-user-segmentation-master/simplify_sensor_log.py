import os

import unicodecsv as csv

from utils.constants import DATA_FOLDER, LOG_ENTRY_DELIMITER, SENSOR_ID_POS

SYMBOLS = [   'A',
    'B',
    'C',
    'D',
    'E',
    'F',
    'G',
    'H',
    'I',
    'J',
    'K',
    'L',
    'M',
    'N',
    'O',
    'P',
    'Q',
    'R',
    'S',
    'T',
    'U',
    'V',
    'W',
    'X',
    'Y',
    'a',
    'b',
    'c'
,
    'd'
,
    'e'
,
    'f'
,
    'g',

   'h',
    'i',
    'l',
    'm',
    'n',
    'o',
    'p',
    'q',
    'r',
    's',
    't',
    'u',
    'v',
    'z',
    'y',
    'x',
    'k',
    'ù',
    'é',
    'è',
    'ò',
    'à',
    '$',
    '%',
    '!',
    '?',
    'ì',
    '&'
,
    '|'
,
    '^'
,
    '§'
,
    'ç','']


def simplify_sensor_log(sensor_log, readable=True):
    """
    Translate the given sensor log in a symbols sequence, such that sequence classification techniques can be applied.
    Notice that, for the sake of readability, we allows only for a maximum number of distinct symbols equals to the size
    of English alphabet (that is enough according to the scope of this project). Then, in that case, a mapping between
    sensor ids and letters is automatically computed.

    :type sensor_log: file
    :param sensor_log: the tab-separated file containing the sensor log.
    :param readable: whether the mapping between sensor ids and letters has to be computed or not.
    """
    file_basename = os.path.splitext(sensor_log.name)[0]
    dest = file_basename + '_simplified.txt'
    dest_dict = file_basename + '_simplified_dict.txt'
    src_reader = csv.reader(sensor_log, delimiter=LOG_ENTRY_DELIMITER)
    sensor_id_dict = {}

    with open(dest, 'w') as simplified_log:
        entry = next(src_reader, None)
        while entry is not None:
            sensor_id = entry[SENSOR_ID_POS]
            print(sensor_id)

            if readable:
                try:
                    translation = sensor_id_dict[sensor_id]
                except KeyError:
                    translation = SYMBOLS[len(sensor_id_dict)]
                    sensor_id_dict[sensor_id] = translation
            else:
                translation = sensor_id

            simplified_log.write(translation + '\n')
            entry = next(src_reader, None)

    with open(dest_dict, 'w') as simplified_log_dict:
        for k, v in sensor_id_dict.items():
            simplified_log_dict.write('%s \t\t %s\n' % (v, k))


if __name__ == '__main__':
    SRC = os.path.join(DATA_FOLDER, 'DatasetPaths.txt')
    with open(SRC, 'rb') as log:
        simplify_sensor_log(log)

    SRC_LOG = os.path.join(DATA_FOLDER, 'PathLog1.txt')
    with open(SRC_LOG, 'rb') as log:
        simplify_sensor_log(log)