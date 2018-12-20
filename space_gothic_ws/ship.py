from planet import Planet
from system import System
from galaxy import Galaxy

import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D, proj3d
import pylab
import numpy as np

def get_distance_from_line (start, end, point ):
    start = np.array (start )
    end = np.array (end )
    point = np.array (point )

    return np.linalg.norm (np.cross (point - start, point - end )  ) / np.linalg.norm (end - start )

def vector_dist (one, two ):
    return np.linalg.norm (np.array (one) - np.array (two) )

class Ship:
    name = ""
    position = (0.01, 0.0, 0.0)
    plot_position_reference = None
    goal = (0.01, 0.0, 0.0)
    path = []
    plot_path_reference = []
    galaxy = None
    direct_route_threshold = 15.0 # lightyears
    max_jump_distance = 20.0 # lightyears
    ax = None
    label = None


    def __init__ (self, galaxy, position = (0.01, 0.0, 0.0), name = "Starship Enterprise" ):
        self.name = name
        self.galaxy = galaxy
        self.position = position
        self.ax = galaxy.ax
        self.fig = galaxy.fig
        self.plot_ship ()

        # whenever motion_notify_event/ button_release_event takes place, execute function
        self.fig.canvas.mpl_connect('motion_notify_event', self.update_plot_label)
        self.fig.canvas.mpl_connect('button_release_event', self.update_plot_label)

    def find_jump_locations (self, current_pos ):
        # find locations along the way
        locations = []
        ship_goal_distance = vector_dist (current_pos, self.goal )

        for sys in self.galaxy.systems:
            system_goal_distance = vector_dist (sys.position, self.goal )

            # if location is near the line from ship directly to goal
            if get_distance_from_line (self.position, self.goal, sys.position ) \
            <= self.direct_route_threshold \
            and system_goal_distance < ship_goal_distance: # and brings ship towards goal
                locations.append (sys.position )

        # if goal has not been set via system or planet, it might not be an available location yet
        if self.goal not in locations:
            locations.append (self.goal )

        return locations


    def plan_path (self ):
        # DEBUG
        found = False
        for sys in self.galaxy.systems:
            if self.goal == sys.position:
                found = True
                print ("\n\n/Planning a Path to " +str(sys.name ) )
        if not found:
            print ("\n\/Planning a Path to " +str(self.goal ) )
            print ("The desired location is not on the starmap " )


        self.path = []
        iterations = 0
        current_pos = self.position
        while iterations < 3000:

            # find the point farthest from ship, but still in range
            current_best_distance_route_point_to_goal = vector_dist (current_pos, self.goal )
            current_best_route_point = self.position

            # find possible jump locations
            locations = self.find_jump_locations (current_pos )

            # choose a location to jump to
            for route_point in locations:
                system_goal_distance = vector_dist (route_point, self.goal )
                ship_goal_distance = vector_dist (current_pos, self.goal )

                distance_to_route_point = vector_dist (route_point, current_pos )
                distance_route_point_to_goal = vector_dist (route_point, self.goal)

                if distance_to_route_point < self.max_jump_distance \
                and distance_route_point_to_goal < current_best_distance_route_point_to_goal:

                    current_best_route_point = route_point
                    current_best_distance_route_point_to_goal = distance_route_point_to_goal


            if ship_goal_distance >= distance_route_point_to_goal \
            and current_best_route_point not in self.path:
                self.path.append (current_best_route_point )
                current_pos = current_best_route_point

            if len (self.path ) > 0 and self.path[-1] == self.goal:
                self.path = [self.position] + self.path
                print ("goal reached. Path:")
                for p in self.path:
                    print (p)
                self.plot_path ()
                return True

            iterations += 1

        print ("out of loop, iterations: " +str(iterations) )

        if iterations >= 3000 or len (self.path ) == 0:
            print ("pathplanning stopped, unknown error occured")
            for p in self.path:
                print (p)
            self.plot_path ()
            path = []
            return False


    def set_goal (self, goal ):
        if isinstance(goal, Galaxy ) or isinstance(goal, System ) or isinstance(goal, Planet ):
            # Do not fly directly into the star, park 9460,73 km aside from it.
            self.goal = (goal.position[0] + 0.000000001, goal.position[1], goal.position[2] )
        elif goal[0] != None and goal[1] != None and goal[2] != None and goal != None:
            self.goal = goal

        self.plan_path ()


    def journey_step (self, skip_to_end=False ):
        ''' make the next step of the journey. Print how long the step took,
        how much fuel was consumen and where you are now '''
        # determine current and next System along the path
        current_pos = self.path[self.path.index (self.position )]
        if self.path.index (self.position ) + 1 == len (self.path ):
            print ("You already arrived.")
            return
        next_pos = self.path[self.path.index (self.position ) + 1]


        # move the ship one step along the path
        self.position = next_pos
        if skip_to_end:
            self.position = self.path[-1]
        self.plot_ship ()


    def scan_surroundings (self ):
        pass


    def plot_path (self ):
        if len (self.path ) > 0:
            #first path plot
            x1 = [self.position[0], self.path[0][0] ]
            y1 = [self.position[1], self.path[0][1] ]
            z1 = [self.position[2], self.path[0][2] ]
            self.ax.plot (x1, y1, z1, c='r' )

            #sencond path plot to second-last path plot
            for i in range (len(self.path)-1):
                x = [self.path[i][0], self.path[i+1][0] ]
                y = [self.path[i][1], self.path[i+1][1] ]
                z = [self.path[i][2], self.path[i+1][2] ]
                self.ax.plot (x, y, z, c='r' )


    def update_plot_label (self ):
        x2, y2, _ = proj3d.proj_transform(self.position[0], self.position[1], self.position[2], self.ax.get_proj() )
        self.label.xy = x2, y2
        self.label.update_positions(self.fig.canvas.get_renderer () )


    def plot_ship (self ):
        self.ax.scatter(self.position[0], self.position[1], self.position[2], c='blue', marker='^' )
        x2, y2, _ = proj3d.proj_transform(  self.position[0], self.position[1], self.position[2], \
                                            self.ax.get_proj() )
        if self.label is not None:
            self.update_plot_label  %%%%%% make a class for plottting !!!
        else:
            self.label = (pylab.annotate(
                self.name,
                xy = (x2, y2), xytext = (0, 65 ),
                textcoords = 'offset points', ha = 'right', va = 'bottom',
                #bbox = dict(boxstyle = 'round,pad=0.5', fc = 'black', alpha = 0.1),
                arrowprops = dict(arrowstyle = '->', connectionstyle = 'arc3,rad=0') )  )

        plt.draw ()











#
