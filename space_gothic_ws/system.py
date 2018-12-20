from planet import Planet

import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D, proj3d
import numpy as np

class System:

    name = ""
    position = (0.0, 0.0, 0.0)
    planets = []

    def __init__ (self, name="", pos=(0.0, 0.0, 0.0), generate_random=False ):

        self.name = name
        self.position = pos

        if generate_random:
            # x = np.random.uniform (-150, 150 )
            # y = np.random.uniform (-150, 150 )
            # z = np.random.uniform (-10, 10 )

            # x = np.random.uniform (100, 0 )
            # y = np.random.uniform (-10, 10 )
            # z = np.random.uniform (-10, 10 )

            x = np.random.uniform (-54.33, 0 )
            y = np.random.uniform (-7.23, 0 )
            z = np.random.uniform (-36.06, 0 )
            self.position = (x, y, z )

            self.name = str ("[" + str (round(x)) + " " + str (round(y)) + " " + str (round(z)) + "]" )
