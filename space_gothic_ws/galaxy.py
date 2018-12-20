from system import System

import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D, proj3d
import pylab

class Galaxy:

    systems = []
    fig = None
    ax = None
    labels = []
    plot_update_sampler = 0
    position = 0.0


    def __init__ (self ):
        self.fig = plt.figure()
        self.ax = self.fig.add_subplot(111, projection='3d')

        # whenever motion_notify_event/ button_release_event takes place, execute function
        self.fig.canvas.mpl_connect('motion_notify_event', self.update_plot_labels)
        self.fig.canvas.mpl_connect('button_release_event', self.update_plot_labels)


    def get_system (self, name ):
        for sys in self.systems:
            if sys.name == name:
                return sys
        print ("ERROR: requested system <"+str(name )+"> could not be found")
        return None


    def read_systems_from_file (self, filename ):
        ''' add Systems from text file with format:
            >planet1,x,y,z;planet2,x,y,z;<
            Example:
            >Eta Corvi,-54.33,-7.23,-36.06;Eta Leporis,1.0,30.0,-12.0;< '''

        file = open (filename, "r" )
        content = file.read ()
        elements = content.split (";" )

        for e in elements:
            e = e.strip("\n")
            data = e.split ("," )

            if len (data ) == 4:
                s = System (data[0], (float(data[1]), float(data[2]), float(data[3]) )  )
                self.systems.append (s )


    def label_plot (self ):
        for s in self.systems:
            x2, y2, _ = proj3d.proj_transform(s.position[0], s.position[1], s.position[2], self.ax.get_proj() )
            self.labels.append (pylab.annotate(
                s.name,
                xy = (x2, y2), xytext = (0, 65 ),
                textcoords = 'offset points', ha = 'right', va = 'bottom',
                #bbox = dict(boxstyle = 'round,pad=0.5', fc = 'black', alpha = 0.1),
                arrowprops = dict(arrowstyle = '->', connectionstyle = 'arc3,rad=0') )  )


    def update_plot_labels (self, e ):

        for label, s in zip (self.labels, self.systems ):
            x2, y2, _ = proj3d.proj_transform(s.position[0], s.position[1], s.position[2], self.ax.get_proj() )
            label.xy = x2, y2
            label.update_positions(self.fig.canvas.renderer)
        self.fig.canvas.draw ()


    def get_plot_limits (self ):

        min_lim = 999999999
        max_lim = -999999999

        for sys in self.systems:
            min_val = min (sys.position )
            max_val = max (sys.position )

            if min_val < min_lim:
                min_lim = min_val
            if max_val > max_lim:
                max_lim = max_val

        return min_lim, max_lim


    def plot (self ):

        # Hide grid and axes ticks
        self.ax.grid(False)
        self.ax.set_xticks([])
        self.ax.set_yticks([])
        self.ax.set_zticks([])

        # set axes range
        lim = self.get_plot_limits ()
        self.ax.set_xlim3d(lim[0], lim[1] )
        self.ax.set_ylim3d(lim[0], lim[1] )
        self.ax.set_zlim3d(lim[0], lim[1] )

        for s in self.systems:
            c = 'r'
            if s.name == "Sol":
                c = 'yellow'
            pos = (s.position[0], s.position[1], s.position[2] )
            self.ax.scatter(pos[0], pos[1], pos[2], c=c, marker='o'  )
        plt.draw ()

        self.label_plot ()
        plt.draw ()
