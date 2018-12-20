class Planet:

    name = ""
    position = (0.0, 0.0, 0.0)
    description = ""

    def __init__ (self, pos=(0.0, 0.0, 0.0), system="", generate_random=False ):
        ''' expects a name, a 3-tuple position and a home system for construction
            these can be determined randomly by setting random=True '''
        #if generate_random:
        pass
