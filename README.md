# Code_Challenge
Repo to submit Code Challenge to DealorOn

## Introduction

Welcome! First, I want to thank you all at DealerOn for taking the chance to consider me as a candidate. It's an exciting time 
and I hope for the opportunity to meet you all someday.

I ultimately decided to take on Problem 1 - Mars Rover of the Code Challenge to complete for my submission. I settled on this 
task as I felt that task 2 was mostly a parsing job, and that task 3 was an implementation of vector graph algorithms. 
Task 1 was more favorable to me because there seemed to be more opportunity to customize various implementation details, and
it appealed to my love for space. I take pleasure in learning how systems function behind the scenes, so the chance to develop my own method was intriguing.

## Implementation

Throughout this process, I was forced to make many decisions that affected the final outcome of my submission. For starters, 
despite having limited experience in C#, I chose to use it as my language for development. I'm always open to learning new 
languages, and I wanted to develop in an environment most similar to what you use. This would prove to add headaches, as I
began coding with Sublime in a standard folder instead of within a VisualStudio project. You can imagine my frustration as I 
wrestled with getting Unit Tests to work. I decided that the time commitment to get the Unit Test working
was becoming expensive, and eventually settled with making a light tester class within the same file to confirm individual
variable/method behavior. Another consequence of using C# was my ignorance of their RegEx class. I struggled with getting the 
RegEx method I wanted to use (RegEx.Matches) to work properly, and so I had to settle with manipulating RegEx.Match.

### Design Decisions

Initially, I assumed that I would create a map-like structure (say, 2-D array) to store the location of the rover.
But after some thought, I decided that it was overkill, and that the rover could be represented with a lot less information.
I made a "Rover" class that maintained 3 ints to represent the rover. I noticed that directions for rotations were done
by adjusting the current direction, as opposed to speciifying a new direction. So, I decided to make the direction an int 
so that rotations could be performed with simple increments/decrements instead of long chains of "if" "else" statements.
I then made a "Mapper" class to monitor the plateu bounds, as well as call the Rover commands. Only one mapper is needed 
per run as we specify the bounds once at the beginning. However, the map is able to spawn multiple rovers per file read.
Lastly, I made a "Simulator" class that features the Main entry point, and an input processor. Main handles starting the program, 
retrieving desired user input (via input processor and RegEx), executing the commands for a particular rover (via Mapper),
and displaying the results. Input can be supplied via the command line (if no command line arguments are specified) or an input 
file (specify filename as only command line argument), and all output will posted to Console and written into "Results.txt".
Excluding comments, the code base comes out to roughly 275 lines of code.

### Assumptions

I made several assumptions that lead to a larger code base than originally planned. I decided to account for rover moves that
would cause the rover to leave the bounds of the plateu. Instead of allowing the move, it sets a trigger that prompts a warning 
during the final location print. It does NOT prevent a rover from being spawned outside the bounds of the plateu, however, it will
not be allowed to move unless its returning back to the plateu. This required me to add a move_sim() method to Rover, as well as a safe()/triggered() methods in Mapper. Another assumption I made was that bounds of the plateu (the x and y) would be the same amount of digits, and that all input is formatted correctly. Obviously, this is hard to guarantee from a user, but this allowed me to easily accept plateu sizes ranging from 0-999. 

### Class Documents

Simulator:
  Main: 
  public static List<string> input_processor(string filename)
    - Takes a string that specifies the file to read from, if the string is blank then read from Console.
    - Returns a string list that holds each line of input
  
 Rover:
  public Rover(int x, int y, int dir)
    - Constructor for Rover. Sets the 3 ints of Rover to the parameters.
  public  
