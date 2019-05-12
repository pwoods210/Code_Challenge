using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/*****************************************************
 * Patrick Woods - 5/12/2019
 * DealerOn - Code Challenge - Problem 1: Mars Rover
 *****************************************************/

namespace Rover_Sim
{
    public class Simulator
    {
        // Main handles input via input_processor(), uses RegEx to pull data, sends commands to Map, prints the results
        static void Main(string[] args)
        {
            // initial variable declarations
            List<string> pre_input;
            string[] input;
            List<string> output = new List<string>();
            Mapper map = null;
            int counter = 0;

            // read from file args[0]
            if (args.Length == 1) pre_input = Rover_Sim.Simulator.input_processor(args[0]);

            // read from command line
            else pre_input = Rover_Sim.Simulator.input_processor("");

            input = pre_input.ToArray();

            while (counter < input.Length)
            {
                // Initialize map with data from first line. y_prep is a gimmick variable that allows for 1-3 digit map sizes.
                // y_prep checks the amount of digits in the x variable, starts the regex match at where the y should start.
                if (counter == 0)
                {
                    Regex regex = new Regex(@"\d+");
                    Match m = regex.Match(input[counter], 0);
                    int x = Int32.Parse(m.Value);
                    int y_prep = x / 10;

                    if (y_prep < 1) m = regex.Match(input[counter], 2);
                    else if (y_prep < 10) m = regex.Match(input[counter], 3);
                    else m = regex.Match(input[counter], 4);

                    int y = Int32.Parse(m.Value);
                    map = new Mapper(x, y);
                }

                // Spawn the rover from data of the odd numbered lines. Direction is stored as an int, needs to be converted.
                else if (counter % 2 == 1)
                {
                    Regex regex = new Regex(@"\d+");
                    Match m = regex.Match(input[counter], 0);
                    int x = (Int32.Parse(m.Value));
                    int y_prep = x / 10;

                    if (y_prep < 1) m = regex.Match(input[counter], 2);
                    else if (y_prep < 10) m = regex.Match(input[counter], 3);
                    else m = regex.Match(input[counter], 4);

                    int y = (Int32.Parse(m.Value));
                    m = Regex.Match(input[counter], @"[NSEW]");
                    char d = m.Value[0];

                    switch (d)
                    {
                        case 'N': map.spawn(x, y, 0);
                            break;
                        case 'E': map.spawn(x, y, 1);
                            break;
                        case 'S': map.spawn(x, y, 2);
                            break;
                        case 'W': map.spawn(x, y, 3);
                            break;
                    }
                }

                // Feed instructions to the rover from data of the even numbered lines. Displays warning if rover attempts to move
                // beyond the bounds set on line 1. Adds final rover position to output, which will be written into Results.txt.
                // Prints the final rover location to console.
                else
                {
                    Regex regex = new Regex(@"[LMR]");
                    int x = 0;
                  
                    while(x < input[counter].Length)
                    {
                        Match m = regex.Match(input[counter], x);
                        map.command(m.Value[0]);
                        x++;
                    }

                    output.Add(map.print());
                    System.Console.WriteLine(map.print());

                    if (map.triggered()) System.Console.WriteLine("WARNING: Rover attempted to break the premises.");
                }
                
                counter++;
            }

            System.IO.File.WriteAllLines(@"Results.txt", output.ToArray());
            System.Console.WriteLine("Successful simulation run.");
        }

        // Reads input of filename or command line, adds data to an string list to be extracted in main.
        // Both input types result in a string list to be processed.
        public static List<string> input_processor(string filename)
        {
            List<string> input = new List<string>();
            string line = "";

            if (string.Compare(filename, "") != 0)
            {
                if (File.Exists(filename))
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(filename);

                    while ((line = file.ReadLine()) != null) input.Add(line);

                    return input;
                }

                else
                {
                    Console.WriteLine("File not found\n");
                    System.Environment.Exit(0);
                    return input;
                }
            }

            else
            {
                Console.WriteLine("Alternate usage - supply file name as the only command line argument.\n" +
                   "Please input plataeu bounds first, followed by all of the commands.\n" + 
                   "Accepts up to 3 digits, but # of x digits must equal # of y digits. Ex: 4 5; 123 789; etc.)\n " +
                   "Type 'exit' to end input.\n");

                for (; ;)
                {
                    line = Console.ReadLine();

                    if (string.Compare(line, "exit") != 0) input.Add(line);

                    else break;
                }

                return input;
            }
        }
    }

    // This class handles maintaining the rover position, including spawning/moving/rotating. Prints location in X Y D format.
    // Features a move_sim() function that reports the new value after a move() call without moving the rover.
    public class Rover
    {
        int x, y, d; 

        public Rover(int x_pos, int y_pos, int dir)
        {
            x = x_pos;
            y = y_pos;
            d = dir;
        }

        public void set_dir(int dir)
        {
            d = dir;
        }

        public void set_cord(int x_pos, int y_pos)
        {
            x = x_pos;
            y = y_pos;
        }

        // The if statement turns negative valid directions into their positive valid counterpart.
        public int get_dir()
        {
            if ((d % 4) < 0) return (d % 4) + 4;

            else return d % 4;
        }

        public void rotate(char dir)
        {
            if (dir == 'L') d--;

            else d++;
        }

        public void move()
        {
            if (d % 4 == 0) y++;

            else if (d % 4 == 1 || d % 4 == -3) x++;

            else if (d % 4 == 2 || d % 4 == -2) y--;

            else x--; 
        }

        public string print()
        {
            char temp = 'x'; //dummy value, avoid compile error for line 184
            
            switch ((d%4))
            {
                case 0: temp = 'N';
                    break;
                case 1:
                case -3:
                    temp = 'E';
                    break;
                case 2:
                case -2:
                    temp = 'S';
                    break;
                case 3:
                case -1:
                    temp = 'W';
                    break;
            }

            string output = x.ToString() + " " + y.ToString() + " " + temp.ToString();
            return output;
        }

        // for Mapper to check valid moves
        public int move_sim()
        {
            switch((d%4))
            {
                case 0: return (y + 1);
                case 1:
                case -3:
                    return (x + 1);
                case 2:
                case -2:
                    return (y - 1);
                case 3:
                case -1:
                    return (x - 1);
            }

            return -1; //safe check
        }
    }

    // The Mapper class monitors the bounds of the plateu, as well as feed commands to the rover.
    // It will display a warning message if the rover attempts to leave the plateu.
    public class Mapper
    {
        int x_m, y_m;
        bool warning;
        Rover rover;

        public Mapper(int x_max, int y_max)
        {
            x_m = x_max;
            y_m = y_max;
            rover = null;
            warning = false;
        }

        public void command(char dir)
        {
            if (rover == null)
            {
                Console.WriteLine("Rover isnt spawned.");
            }

            else if (dir == 'M' && safe()) rover.move();

            else if (dir == 'M' && !safe()) warning = true;

            else rover.rotate(dir);
        }

        public void spawn(int x_pos, int y_pos, int dir)
        {
            rover = null; 
            rover = new Rover(x_pos, y_pos, dir);
            warning = false;
        }

        // Checks to see if the next move() command will cause the rover to leave the plateu.
        public bool safe()
        {
            if (((rover.move_sim()) > x_m && rover.get_dir() == 1) ||
                ((rover.move_sim()) > y_m && rover.get_dir() == 0) ||
                (rover.move_sim()) < 0) return false;

            return true;
        }

        public string print()
        {
            return rover.print();
        }

        public bool triggered()
        {
            return warning;
        }
    }
}

// Hacked together testing class that I used to watch individual variables and methods. 
// Also used to iron out input processing and regex errors.
// To use, I commented out "Main" in class Simulator, uncommented class Rover_Tester, and ran with no command line args.
// Truthfully, the output will seem chaotic and useless as this was a tool to aid my building process with the lack of unit tests.

/*
public class Rover_Tester
{ 
    static void Main(string[] args)
    {
        Rover_Tester test = new Rover_Tester();
        test.inputTest();
        test.roverBasic();
        test.mapBasic();
    }
    
    public void inputTest()
    {
        Rover_Sim.Simulator sim = new Rover_Sim.Simulator();
        int counter = 0;
        List<string> input = new List<string>();
        input = Rover_Sim.Simulator.input_processor("Rover_Tester.txt");
        string[] newinput = input.ToArray();

        while (counter < newinput.Length)
        {
            Console.WriteLine(newinput[counter]);
            
            if (counter == 0)
            {
                Regex regex = new Regex(@"\d+");
                Match m = regex.Match(input[counter], 0);
                int x = Int32.Parse(m.Value);
                int xx = x / 10;
                if (xx < 1) m = regex.Match(input[counter], 2);
                else if (xx < 10) m = regex.Match(input[counter], 3);
                else m = regex.Match(input[counter], 4);

                int y = Int32.Parse(m.Value);
                Console.WriteLine(x + " " + y);
                map = new Mapper(x, y);
            }
            
            else if (counter % 2 == 1)
            {
                Regex regex = new Regex(@"\d+");
                Match m = regex.Match(input[counter], 0);
                Console.WriteLine(m.Value);
                int x = (Int32.Parse(m.Value));
                m = regex.Match(input[counter], 1);
                Console.WriteLine(m.Value);
                int y = (Int32.Parse(m.Value));
                m = Regex.Match(input[counter], @"[NSEW]");
                Regex regex = new Regex(@"(\d+)\s+(\d+)\s+([NWES])");
                char d = m.Value[0];
                Console.Write(x +" "+ y + " " + d);
                Console.Write(" ");
                Console.WriteLine(d);
                    
                switch (d)
                {
                    case 'N':
                        map.spawn(x, y, 0);
                        break;
                    case 'E':
                        map.spawn(x, y, 1);
                        break;
                    case 'S':
                        map.spawn(x, y, 2);
                        break;
                    case 'W':
                        map.spawn(x, y, 3);
                        break;
                }
            }
            
            else
            {
                Regex regex = new Regex(@"[LMR]");
                
                Match m = Regex.Match(input[counter], @"[LMR]");
                int x = 0;
                
                while (x < input[counter].Length)
                {

                    map.command(m.Value[0]);
                    Match m = regex.Match(input[counter], x);
                    Console.WriteLine(m.Value[0]);
                    x++;
                }

                output.Add(map.print());
                System.Console.WriteLine(map.print());
                if (map.triggered()) System.Console.WriteLine("WARNING: Rover attempted to break the premises.\n");
            }
            counter++;
        }
    }
    
    public void roverBasic()
    {
        Rover_Sim.Rover rover = new Rover_Sim.Rover(1, 0, 0);
        rover.move();
        rover.move();
        rover.rotate('R');
        rover.rotate('R');
        rover.rotate('R');
        rover.rotate('R');
        rover.rotate('R');
        rover.move();
        Console.WriteLine(rover.print());
    }

    public void mapBasic()
    {
        Rover_Sim.Mapper map = new Rover_Sim.Mapper(0, 0);
        map.command('M');
        map.spawn(0, 0, 0);
        map.command('R');
        Console.WriteLine(map.print());
        Console.WriteLine(map.triggered());
        map.spawn(1, 1, 1);
        Console.WriteLine(map.print());
        Console.WriteLine(map.triggered());
        Console.WriteLine(map.safe());
        map = new Rover_Sim.Mapper(5, 5);
        map.spawn(0, 0, 0);
        map.command('M');
        Console.WriteLine(map.print());
        Console.WriteLine(map.triggered());
        Console.WriteLine(map.safe());
        map.spawn(1, 1, 1);
        map.command('M');
        Console.WriteLine(map.print());
    }
}
*/