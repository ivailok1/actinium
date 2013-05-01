using System;
using Wintellect.PowerCollections;

namespace LabirynthGame
{
	// tuka polzvame edna biblioteka - PowerCollections - moze da ya namerite v gugal - ima sortiran re4nik, mnogo udobno za klasaciata
    class Labirynth
    {
        private const int size = 7;
        private const int px = 3;
        private const int py = 3;
        private const int MinimumPercentageOfBlockedCells = 30;
        private const int MaximumPercentageOfBlockedCells = 50;

        const char BlockedCell = 'X';
        const char FreeCell = '-';
        const char PlayerSign = '*';

        private int playerPositionX;
        private int playerPositionY;

        private char[,] matrix;
        private OrderedMultiDictionary<int, string> scoreBoard;
      
        // + class for point for example


        // Labyrinth
        public Labirynth()
        {
            this.playerPositionX = px;
            this.playerPositionY = py;
            this.matrix = this.GenerateMatrix();
            this.scoreBoard = new OrderedMultiDictionary<int, string>(true);
        }

        private void Move(int dirX, int dirY)
        {

            if(this.IsMoveValid(this.playerPositionX + dirX , this.playerPositionY+dirY)==false)
            {
                return;
            }

            if (this.matrix[playerPositionY + dirY, playerPositionX + dirX] == BlockedCell)
            {
                Console.WriteLine("Invalid Move!");
                Console.WriteLine("**Press a key to continue**");
                Console.ReadKey();
                return;
            }
            else
            {
                this.matrix[this.playerPositionY, this.playerPositionX] = FreeCell;
                this.matrix[this.playerPositionY + dirY, this.playerPositionX + dirX] = PlayerSign;
                this.playerPositionY += dirY;
                this.playerPositionX += dirX;
                return;
            }
        }

        private bool IsMoveValid(int x, int y)
        {
            if (x < 0 || x > size - 1 || y < 0 || y > size - 1)
            {
                return false;
            }

            return true;
        }

        private void PrintLabirynth()
        {
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    Console.Write("{0,2}", this.matrix[row, col]);
                }
                Console.WriteLine();
            }
        }

        private char[,] GenerateMatrix()
        { 
            char[,] generatedMatrix = new char[size, size];
            Random rand = new Random();
            int percentageOfBlockedCells = rand.Next(MinimumPercentageOfBlockedCells, MaximumPercentageOfBlockedCells);

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    int num = rand.Next(0, 100);
                    if (num < percentageOfBlockedCells)
                    {
                        generatedMatrix[row, col] = BlockedCell;
                    }
                    else
                    {
                        generatedMatrix[row, col] = FreeCell;
                    }

                }
            }
            generatedMatrix[playerPositionY, playerPositionX] = PlayerSign;

            this.MakeAtLeastOneExitReachable(generatedMatrix);
            Console.WriteLine("Welcome to “Labirinth” game. Please try to escape. Use 'top' to view the top");
            Console.WriteLine("scoreboard, 'restart' to start a new game and 'exit' to quit the game.");
            return generatedMatrix;
        }

        private void MakeAtLeastOneExitReachable(char[,] generatedMatrix)
        {
            Random rand = new Random();
            int pathX = px;
            int pathY = py;
            int[] dirX = { 0, 0, 1, -1 };
            int[] dirY = { 1, -1, 0, 0 };
            int numberOfDirections = 4;
            int maximumTimesToChangeAfter = 2;

            while (this.IsGameOver(pathX , pathY) == false)
            {
                int num = rand.Next(0, numberOfDirections);
                int times = rand.Next(0, maximumTimesToChangeAfter);

                for (int d = 0; d < times; d++)
                {
                    if (pathX + dirX[num] >= 0 && pathX + dirX[num] < size && pathY + dirY[num] >= 0 &&
                        pathY + dirY[num] < size)
                    {


                        pathX += dirX[num];



                        pathY += dirY[num];
                        if (generatedMatrix[pathY, pathX] == PlayerSign)
                        {
                            continue;
                        }
                        generatedMatrix[pathY, pathX] = FreeCell;
                    }
                }
            }
        }
   

        // Score
        private int GetWorstScore()
        {


            int worstScore = 0;
            foreach (var score in this.scoreBoard.Keys)
            {
                worstScore = score;
            }

            return worstScore;
        }

        private void PrintScore()
        {
            int counter = 1;

            if (this.scoreBoard.Count == 0)
            {
                Console.WriteLine("The scoreboard is empty.");



            }
            else
            {
                foreach (var score in this.scoreBoard)
                {
                    var foundScore = this.scoreBoard[score.Key];

                    foreach (var equalScore in foundScore)
                    {
                        Console.WriteLine("{0}. {1} --> {2}", counter, equalScore, score.Key);

                    }
                    counter++;
                }
            }


            Console.WriteLine();
        }

        private void UpdateScoreBoard(int currentNumberOfMoves)
        {
            string userName = string.Empty;

            if (this.scoreBoard.Count < 5)
            {
                while (userName == string.Empty)
                {
                    Console.WriteLine("**Please put down your name:**");
                    userName = Console.ReadLine();
                }
                this.scoreBoard.Add(currentNumberOfMoves, userName);
            }
            else
            {
                int worstScore = this.GetWorstScore();
                if (currentNumberOfMoves <= worstScore)
                {
                    if (this.scoreBoard.ContainsKey(currentNumberOfMoves) == false)
                    {
                        this.scoreBoard.Remove(worstScore);
                    }
                    while (userName == string.Empty)
                    {
                        Console.WriteLine("**Please put down your name:**");
                        userName = Console.ReadLine();
                    }
                    this.scoreBoard.Add(currentNumberOfMoves, userName);
                }
            }
        }


        // Engine

        public void PlayGame()
        {
            string command = string.Empty;
            int movesCounter = 0;
            while (command.Equals("EXIT")==false)
            {
                PrintLabirynth();
                string currentLine = string.Empty;

                if (this.IsGameOver(this.playerPositionX, this.playerPositionY))
                {
                    Console.WriteLine("Congratulations! You've exited the labirynth in {0} moves.", movesCounter);

                    this.UpdateScoreBoard(movesCounter);
                    this.PrintScore();
                    movesCounter = 0;
                    currentLine = "RESTART";
                }
                else
                {
                    Console.Write("Enter your move (L=left, R-right, U=up, D=down):");
                    currentLine = Console.ReadLine();
                }
                if (currentLine == string.Empty)
                {
                    continue;
                }

                command = currentLine.ToUpper();
                this.ExecuteCommand(command, ref movesCounter);
            }       
     


        }

        private bool IsGameOver(int playerPositionX, int playerPositionY)
        {
            if ((playerPositionX > 0 && playerPositionX < size - 1) &&
                (playerPositionY > 0 && playerPositionY < size - 1))
            {
                return false;
            }

            return true;
        }

        private void ExecuteCommand(string command, ref int movesCounter)
        {
            switch (command.ToUpper())
            {
                case "L":
                    {
                        movesCounter++;
                        Move(-1, 0);
                        break;
                    }
                case "R":
                    {
                        movesCounter++;
                        Move(1, 0);
                        break;
                    }
                case "U":
                    {
                        movesCounter++;
                        Move(0, -1);
                        break;
                    }
                case "D":
                    {
                        movesCounter++;
                        Move(0, 1);
                        break;
                    }
                case "RESTART":
                    {
                        this.playerPositionX = px;
                        this.playerPositionY = py;
                        this.matrix = this.GenerateMatrix();

                        break;
                    }
                case "TOP":
                    {
                        this.PrintScore();
                        break;
                    }
                case "EXIT":
                    {
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Invalid input!");
                        Console.WriteLine("**Press a key to continue**");
                        Console.ReadKey();
                        break;
                    }
            }
        }
    }
}
