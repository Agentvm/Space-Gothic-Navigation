#!/usr/bin/env python3
# die Krone von Lytar

# TODO: Ship position entlängs des paths verschiebbar machen (rückgängig? )
# TODO: comment the code where necessary

#imports
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D, proj3d
import numpy as np

# classes

from planet import Planet
from system import System
from galaxy import Galaxy
from ship import Ship

# start of program
if __name__ == "__main__":


    # make a new galaxy and a ship within
    milky_way = Galaxy ()
    milky_way.read_systems_from_file ("systems.txt")

    #add some random Systems
    for i in range (15):
        milky_way.systems.append (System (generate_random=True )  )
    milky_way.plot ()


    # add a ship to the galaxy
    starship_enterprise = Ship (milky_way, position=(0,0,0) )
    starship_enterprise.plot_ship ()

    # set a goal
    #starship_enterprise.set_goal ((100, 0, 0 )  )
    # Eta Corvi,-54.33,-7.23,-36.06
    starship_enterprise.set_goal (milky_way.get_system ("Eta Corvi").position )
    for i in range (3):
        starship_enterprise.journey_step ()
    plt.show ()

    ## LAST ERROR
    #vector_dist (milky_way.get_system ("Eta Corvi").position, (-48.13148251294754, -5.1765121164439005, -34.61348507243649))
    # ---> 6.68811161397
    #In IF
    #65.6075102408



    #
