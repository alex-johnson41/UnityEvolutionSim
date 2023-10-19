# Unity Evolution Simulator
## Created by Alex Johnson
Inspired by [this](https://www.youtube.com/watch?v=N3tRFayqVtk) video

Initially implemented in python in [this](https://github.com/alex-johnson41/evolutionSim) repo

## Code Architecture:
### Project designed using vanilla c# classes with a unity monobehavior class wrapped around each of the classes that need to be present in the unity game and not just internally
### Vanilla C# classes:
- Sim Controller
  - API for setting up and controlling the simulation as it runs
- World
  - Contains the locations of all the individuals and manages their movement within the world
- Individual
  - Contians a set of all supported input, internal, and output neurons and a neural network
  - Makes decisions based on the inputs it recieves and the connections in it's neural network
- Neural Network
  - Contains a list of synapses, or connections between input, output and internal neurons. 
- Synapse
  - A single connection between an input or internal neuron, and an internal or output neuron. 
  - Has a weight value that is applied to the data it receives as it is passed along to the next neuron
- Neuron
  - Base class for all neuron types
- Input Neuron
  - Recieves various inputs from the world as a double between 0 and 1
  - Synapses send the data to either internal or output neurons
  - Currently supported inputs:
    - X position in the world 
      - Calculated as x coordinate in world / width of world
    - Y position in the world 
      - Calculated as y coordinate in world / height of world
    - Nearest X border
      - 0 if x position < 0.5, 1 if x position > 0.5
    - Nearest Y border
      - 0 if y position < 0.5, 1 if y position > 0.5
    - Random input
      - Random double from 0 to 1
- Output Neuron
  - Recieves a list of weighted data values from input/internal neurons
  - Returns the probability that the individual will take the specified action, or inverse of the action (-1 to 1)
  - Probility is calculated as tanh(sum(input data))
  - Currently supported outputs:
    - Move X
      - Increase x by one if probability returns true and is positive
      - Decrease x by one if probability returns true and is negative
    - Move Y
      - Same as move x but for the y axis
    - Move forward
      - Move the individual whatever direction it last moved
      - Can be any of 8 possible directions (N, S, E, W, NW, NE, SW, SE)
    - Move random
      - Move in one of 8 random directions
- Internal Neuron
  - Recieves a list of weighted data values from input/internal neurons
  - Same calculations as output neurons, except once their data is summed up and calculated it is sent to another internal neuron (potentially itself), and/or an output neuron. 
  - In the case that a synapse sends data from an internal neuron to itself, that data is stored until the next step, where it is then included in the input data.
- Enums
  - Several enums are present and used as a set of possible options that are currently supported
  - All enums can be found in Types.cs
  - Types:
    - Input types
      - All supported input neuron types
    - Output types
      - All supported output neuron types/actions that an individual can make
    - Survival conditions
      - All potential options for the goal state of each of the individuals
    - Move directions
      - All 8 possible directions that an individual can move in
### Unity classes:
- Grid Manager
- Individual Wrapper
- Input Controller
- Sim Controller Creator
- Survival Conditions Dropdown