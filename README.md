# WFC-Layered-Model-Godot  
Implementation of Wave Function Collapse algorithm in Godot for 2D sprites with hybrid model and layered aproach to 3rd Dimmension (f. e. objects on map as separate layer)  Alpha stages: only one Layer is supported for now.

# WaveFunctionCollapse  

WaveFunctionCollapse algorithm is texture synthesis algyrythm working on MetaHeuristics and adapted into tileMap generation algorithm.  

To learn more head to: [Original Repository](https://github.com/mxgmn/WaveFunctionCollapse/tree/master)  
  
## Algorithm  
1. Repeat the following steps:  
    1. Observation:  
        1. Find a wave element with the minimal nonzero entropy. If there is no such elements (if all elements have zero or undefined entropy) then break the cycle (1) and go to step (2).  
        2. Collapse this element into a definite state according to its coefficients and the distribution of NxN patterns in the input.  
    2. Propagation: propagate information gained on the previous observation step.  
2. By now all the wave elements are either in a completely observed state (all the coefficients except one being zero) or in the contradictory state (all the coefficients being zero). At last it return the output.  

# Used libraries in the project
(Godot)[https://github.com/godotengine] - game engine this is plugin for.  
(Refit)[https://github.com/reactiveui/refit] - allows connection to the api automatically.  
(Python.Net)[https://github.com/pythonnet/pythonnet] - Eneables to integrate Python Code into C#.  
(Keras)[https://github.com/keras-team/keras] - Keras eneables to make Deep Learing model for sides and corners predictions. 
Other essential c# .Net and Python packages.
