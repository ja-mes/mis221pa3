using System;
using System.Collections.Generic;
using System.Linq;

// TODO: fix bug where game gets stuck when user runs out of gil

namespace Arrays1
{
    static class Utils
    {

        /*
         * Divider creates a line accross the screen with the specified char
         */
        public static void Divider(char c, int length)
        {
            string outputStr = "";
            for (int i = 0; i < length; i++)
                outputStr += c;

            Console.WriteLine(outputStr);
        }

        /*
         * The BuildScreen method first clears the terminal, and then builds a header with a optional subheader
         */
        public static void BuildScreen(string title, string subHeader = null)
        {
            Console.Clear(); // clear the screen
            Divider('-', 50);
            Console.Write($"\n{title.ToUpper()}");

            // display sub header if provided
            if (subHeader != null)
                Console.WriteLine($" / {subHeader}");
            else
                Console.WriteLine();

            Console.WriteLine();
            Divider('-', 50);
        }

        /*
         * The Exit method prints to the screen and terminates the program
         */
        public static void Exit() // TODO: allow custom exit messages
        {
            System.Environment.Exit(0);
        }

    }
    class Card
    {
        public string Suit { get; private set; }
        private int _num;
        public int Num { 
            get
            {
                if (blackJack && FaceCard)
                    return 10;
                return _num;
            }
            set
            {
                _num = value;
            }
        }
        public string DisplayVal { get; private set; }
        public char Symbol { get; private set; }
        public string StringRep { get; private set; }

        public bool blackJack = false;

        public bool hidden; // if the card is hidden we know not to show its value
        public bool FaceCard 
        {
            get 
            {
                if(DisplayVal == "J" || DisplayVal == "Q" || DisplayVal == "K" || DisplayVal == "A") 
                    return true;
                return false;
            }
        }

        public Card(int num, string suit, bool hideCard = true)
        {
            _num = num;

            if(suit == "clubs")
                Symbol = '♣';
            else if(suit == "diamonds")
                Symbol = '♦';
            else if(suit == "hearts")
                Symbol = '♥';
            else if(suit == "spades")
                Symbol = '♠';
            else
                throw new Exception("Invalid suit");

            Suit = suit;
            hidden = hideCard;

            string[] vals = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};
            DisplayVal = vals[Num - 1];

            StringRep = $"{DisplayVal} of {Suit}";
        }
    }

    class Deck
    {
        public Card[] Cards { get; private set; }
        public bool BlackJack { get; private set; }

        public Deck(bool isBlackJack = false)
        {
            BlackJack = isBlackJack;
            Cards = new Card[52];
            Build();
        }

        /* Build creates a deck of 52 cards */
        public void Build()
        {
            string[] suits = { "clubs", "diamonds", "hearts", "spades" };
            int[] number = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

            int k = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    Cards[k] = new Card(number[j], suits[i]);
                    if (BlackJack)
                        Cards[k].blackJack = true;
                    k++;
                }
            }

        }

        public void Shuffle()
        {
            Random random = new Random();
            int n = Cards.Length;

            for (int i = 0; i < (n - 1); i++)
            {
                int r = i + random.Next(n - i);
                Card c = Cards[r];
                Cards[r] = Cards[i];
                Cards[i] = c;
            }

        }
        
        /* SelectCards selects the specified number of cards from the deck*/ 
        public Card[] SelectCards(int numCards)
        {
            Card[] selectedCards = new Card[numCards];
            Array.Copy(Cards, 0, selectedCards, 0, numCards);

            return selectedCards;

        }


        /* Prints the array of cards on 2 lines instead of */
        public void PrintRowSplit(Card[] cards)
        {
            Card[] array1 = cards.Take(cards.Length / 2).ToArray();
            Card[] array2 = cards.Skip(cards.Length / 2).ToArray();

            PrintRow(array1);
            PrintRow(array2);
        }

        /* Print row accepts an array of cards and prints them horrizontally across the screen */
        public void PrintRow(Card[] cards)
        {
            int count = cards.Length;

            // this is the skelton for each card
            string[] lines =
            {
                "┌─────────┐",
                "│{}       │",
                "│         │",
                "│    {}   │",
                "│         │",
                "│         │",
                "│       {}│",
                "└─────────┘",
            };

            string[] hiddenLines =
                {
                "┌─────────┐",
                "│░░░░░░░░░│",
                "│░░░░░░░░░│",
                "│░░░░░░░░░│",
                "│░░░░░░░░░│",
                "│░░░░░░░░░│",
                "│░░░░░░░░░│",
                "└─────────┘",
            };

            const string SPACE = "  ";

            for (int i = 0; i < 8; i++) // each card is 8 lines high
            {
                for (int j = 0; j < count; j++)
                {
                    if (cards[j].hidden)
                    {
                        Console.Write(hiddenLines[i] + SPACE);
                        continue;
                    }

                    if (i == 1)
                    {
                        if(cards[j].DisplayVal.Length >= 2)
                            Console.Write("│{0}       │" + SPACE, cards[j].DisplayVal);
                        else
                            Console.Write("│{0}        │" + SPACE, cards[j].DisplayVal);
                    }
                    else if (i == 3)
                    {
                        Console.Write("│    {0}    │" + SPACE, cards[j].Symbol);
                    }
                    else if (i == 6)
                    {
                        if(cards[j].DisplayVal.Length >= 2)
                            Console.Write("│       {0}│" + SPACE, cards[j].DisplayVal);
                        else 
                            Console.Write("│        {0}│" + SPACE, cards[j].DisplayVal);
                    }
                    else
                    {
                        Console.Write(lines[i] + SPACE);
                    }
                }

                Console.WriteLine();
            }

        }
    }

    class Menu
    {
        private bool error;
        private string[] menuItems;

        public Menu(string[] items)
        {
            menuItems = items;
        }

        /* 
         * GetInput writes the menu to the screen, handles error checking and returns the option the user selects. 
         * This method will call itself recursively until the user selections an option on the menu.
         * TODO: look into breaking this into sub-methods
         */
        public int GetInput()
        {
            Console.WriteLine();

            for (int i = 0; i < menuItems.Length; i++)
            {
                string output;
                output = $"({Convert.ToString(i + 1)}) {menuItems[i]}";

                Console.WriteLine(output);
            }
            Utils.Divider('_', 50);

            if (error)
            {
                Console.WriteLine($"Invalid Input. Select a value between {1} and {menuItems.Length}");
            }

            Console.Write("Please select an option: ");
            int selection;
            bool valid = int.TryParse(Console.ReadLine(), out selection);

            // Ensure that input is a valid menu item
            if (!valid || selection < 1 || selection > menuItems.Length)
            {
                error = true;
                return GetInput();
            }

            return selection;
        }
    }

    class CardShark
    {

        double gilToRisk = 0;

        public void Render()
        {
            string header = "Card Shark";
           string description = "Welcome to Card Shark!\n" +
                              "○ ...\n" +
                              "○ ...\n" +
                              "○ ...\n";

            gilToRisk = Controller.GilScreen(header, description);
            PlayGame();
        }

        void PlayGame()
        {
            Menu menu;

            Deck deck = new Deck();
            deck.Shuffle();

            // select 10 random cards for card shark
            Card[] cards = deck.SelectCards(10);

            int correctCount = 0;

            int i; // want to know the stoping point outside loop
            for(i = 0; i < 9; i++)
            {
                Console.Clear();

                cards[i].hidden = false;

                deck.PrintRowSplit(cards);

                Console.WriteLine();
                Console.WriteLine("You are risking " + gilToRisk.ToString("F") + " gil");
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("Your guess: ");
                string[] menuOptions = {
                    $"Next card will be LOWER than {cards[i].StringRep}",
                    $"Next card will be HIGHER than {cards[i].StringRep}",
                };
                menu = new Menu(menuOptions);

                int selection = menu.GetInput();


                if(cards[i].Num > cards[i + 1].Num) // the next card is less than the current card. user should have selected 1
                {
                    if(selection == 1)
                    {
                        correctCount++;
                    }
                    else
                    {
                        break;
                    }
                }
                else if(cards[i].Num == cards[i + 1].Num)
                {
                    correctCount++;
                }
                else
                {
                    if(selection == 2)
                    {
                        correctCount++;
                    }
                    else
                    {
                        break;
                    }
                }

            }

            Console.Clear();


            if (i < 9)
                cards[i + 1].hidden = false;
            else
                cards[i].hidden = false;

            deck.PrintRowSplit(cards);

            Console.WriteLine();

            if(correctCount < 4)
            {
                Console.WriteLine("YOU BUST!");
                Console.WriteLine($"You lost {gilToRisk.ToString("F")} gil");
                Controller.playerGil -= gilToRisk;
            }
            else if(correctCount <= 5)
            {
                Console.WriteLine("YOU BROKE EVEN");
                Console.WriteLine($"You won 0 gil");
                // don't need to touch the gil here
            }
            else if(correctCount < 9)
            {
                Console.WriteLine("YOU DOUBLED YOUR GIL!");
                Console.WriteLine($"You won {gilToRisk.ToString("F")} gil");
                Controller.playerGil += (gilToRisk);
            }
            else if(correctCount == 9)
            {
                Console.WriteLine("YOU TRIPLED YOUR GIL");
                Controller.playerGil -= gilToRisk;
                gilToRisk *= 3;
                Controller.playerGil += gilToRisk;
                Console.WriteLine($"You won {gilToRisk.ToString("F")} gil");
            }


            Console.WriteLine();
            Console.WriteLine();

            string[] exitOptions = new string[2];

            if(Controller.playerGil > 0)
            {
                exitOptions[0] = "Play Again";
                exitOptions[1] = "Exit";
            }
            else
            {
                exitOptions[0] = "Exit";
            }

            menu = new Menu(exitOptions);

            switch(menu.GetInput())
            {
                case 1:
                    Render();
                    break;
                case 2:
                    Controller.Render();
                    break;
            }

        }


    }

    class Tile
    {
        public int Num { get; private set; }
        public bool turned;

        public Tile(int num, bool isTurned = false)
        {
            Num = num;
            turned = isTurned;
        }
    }

    class TitleGenerator
    {
        public Tile[] tiles;

        public void Generate(int num)
        {
            tiles = new Tile[num];

            for(int i = 1; i <= num; i++)
            {
                tiles[i - 1] = new Tile(i);
            }
        }

        public void PrintRow()
        {
            int count = tiles.Length;

            // prototype for tile
            string[] lines1 =
            {
                "┌─────┐",
                "│  1  │",
                "└─────┘",
                "       ",
                "       ",
            };

            string[] lines2 =
            {
                "       ",
                "       ",
                "┌─────┐",
                "│  1  │",
                "└─────┘",
            };


            string[] linesToUse = lines1;

            const string SPACE = " ";

            for(int i = 0; i < 5; i++) // each tile is 5 lines high
            {
                for(int j = 0; j < count; j++)
                {
                    if (tiles[j].turned)
                        linesToUse = lines2;
                    else
                        linesToUse = lines1;

                    if((linesToUse == lines1 && i == 1) || (linesToUse == lines2 && i == 3))
                    {
                        if(tiles[j].Num >= 10)
                            Console.Write("│ {0}  │" + SPACE, tiles[j].Num);
                        else
                            Console.Write("│  {0}  │" + SPACE, tiles[j].Num);
                    }
                    else
                    {
                        Console.Write(linesToUse[i] + SPACE);
                    }
                }

                Console.WriteLine();
            }

        }
    }

    class Dice
    {
        public int Num { get; private set; }
        public string[] StringRep { get; private set; }

        public Dice()
        {
            Roll();
        }

        public void Roll()
        {
            Num = new Random().Next(1, 7); // returns a num between 1 and 6
            SetStringRep();
        }
        void SetStringRep()
        {
            string[] lines1 =
            {
               "+-------+" ,
               "|       |" ,
               "|   o   |" ,
               "|       |" ,
               "+-------+"
            };

            string[] lines2 =
            {
                "+-------+" ,
                "| o     |" ,
                "|       |" ,
                "|     o |" ,
                "+-------+"
            };

            string[] lines3 =
            {
                "+-------+" ,
                "| o     |" ,
                "|   o   |" ,
                "|     o |" ,
                "+-------+"
            };

            string[] lines4 =
            {
                  "+-------+" ,
                  "| o   o |" ,
                  "|       |" ,
                  "| o   o |" ,
                  "+-------+"
            };

            string[] lines5 =
            {
                 "+-------+" ,
                 "| o   o |" ,
                 "|   o   |" ,
                 "| o   o |" ,
                 "+-------+"
            };

            string[] lines6 =
            {
                  "+-------+" ,
                  "| o   o |" ,
                  "| o   o |" ,
                  "| o   o |" ,
                  "+-------+"
            };

            if(Num == 1)
            {
                StringRep = lines1;
            }
            else if(Num == 2)
            {
                StringRep = lines2;
            }
            else if(Num == 3)
            {
                StringRep = lines3;
            }
            else if(Num == 4)
            {
                StringRep = lines4;
            }
            else if(Num == 5)
            {
                StringRep = lines5;
            }
            else if(Num == 6)
            {
                StringRep = lines6;
            }
        }
    }

    class TwoDice
    {
        public Dice dice1;
        public Dice dice2;
        public TwoDice()
        {
            dice1 = new Dice();
            dice2 = new Dice();
        }

        public void Roll()
        {
            dice1.Roll();
            dice2.Roll();
        }

        public void Print()
        {
            string[] line1 = dice1.StringRep;
            string[] line2 = dice2.StringRep;

            const string SPACE = "  ";
            for(int i = 0; i < 5; i++) // each dice 5 high
            {
                Console.Write(line1[i] + SPACE);
                Console.Write(line2[i] + SPACE);
                Console.WriteLine();
            }
        }
    }

    class ShutTheBox
    {
        double gilToRisk;

        public void Render()
        {
            string header = "Shut the Box";
            string description = "Welcome to Shut the Box!\n" +
                              "○ ...\n" +
                              "○ ...\n" +
                              "○ ...\n";
            gilToRisk = Controller.GilScreen(header, description);
            PlayGame();
        }

        void PlayGame()
        {
            Menu menu;
            TitleGenerator tileG = new TitleGenerator();
            tileG.Generate(12);

            TwoDice twoDice = new TwoDice();
            bool gameGoing = true;

            while(gameGoing)
            {
                twoDice.Roll();

                int dice1 = twoDice.dice1.Num;
                int dice2 = twoDice.dice2.Num;
                int diceTotal = dice1 + dice2;


                List<string> menuOptions = new List<string>();

                List<string> choices = new List<string>();

                gameGoing = false;
                if(tileG.tiles[diceTotal - 1].turned == false)
                {
                    menuOptions.Add($"Turn tile {diceTotal}");
                    choices.Add("total");
                    gameGoing = true;
                }
                if(tileG.tiles[dice1 - 1].turned == false && 
                    tileG.tiles[dice2 - 1].turned == false && 
                    tileG.tiles[dice2 - 1].Num != tileG.tiles[dice1 -1].Num)
                {
                    menuOptions.Add($"Turn tile {dice1} and {dice2}");
                    choices.Add("both");
                    gameGoing = true;
                }
                if(tileG.tiles[dice1 -1].turned == false)
                {
                    menuOptions.Add($"Turn tile {dice1}");
                    choices.Add("first");
                    gameGoing = true;
                }
                if(tileG.tiles[dice2 -1].turned == false && tileG.tiles[dice2 - 1].Num != tileG.tiles[dice1 -1].Num)
                {
                    menuOptions.Add($"Turn tile {dice2}");
                    choices.Add("second");
                    gameGoing = true;
                }

                if (gameGoing == false)
                    break;

                Console.Clear();
                tileG.PrintRow();
                Utils.Divider('-', 100);
                Console.WriteLine();
                twoDice.Print();

                string[] arr = menuOptions.ToArray();
                menu = new Menu(arr);

                Console.WriteLine();

                // TODO: refactor this. 
                switch(menu.GetInput())
                {
                    case 1:
                        if(choices[0] == "total")
                            tileG.tiles[diceTotal - 1].turned = true;
                        else if(choices[0] == "both")
                        {
                            tileG.tiles[dice1 - 1].turned = true;
                            tileG.tiles[dice2 - 1].turned = true;
                        }
                        else if(choices[0] == "first")
                            tileG.tiles[dice1 - 1].turned = true;
                        else if(choices[0] == "second") 
                            tileG.tiles[dice2 - 1].turned = true;
                        break;
                    case 2:
                        if(choices[1] == "total")
                            tileG.tiles[diceTotal - 1].turned = true;
                        else if(choices[1] == "both")
                        {
                            tileG.tiles[dice1 - 1].turned = true;
                            tileG.tiles[dice2 - 1].turned = true;
                        }
                        else if(choices[1] == "first")
                            tileG.tiles[dice1 - 1].turned = true;
                        else if(choices[1] == "second") 
                            tileG.tiles[dice2 - 1].turned = true;
                        break;
                    case 3:
                        if(choices[2] == "total")
                            tileG.tiles[diceTotal - 1].turned = true;
                        else if(choices[2] == "both")
                        {
                            tileG.tiles[dice1 - 1].turned = true;
                            tileG.tiles[dice2 - 1].turned = true;
                        }
                        else if(choices[2] == "first")
                            tileG.tiles[dice1 - 1].turned = true;
                        else if(choices[2] == "second") 
                            tileG.tiles[dice2 - 1].turned = true;
                        break;
                    case 4:
                        if(choices[3] == "total")
                            tileG.tiles[diceTotal - 1].turned = true;
                        else if(choices[3] == "both")
                        {
                            tileG.tiles[dice1 - 1].turned = true;
                            tileG.tiles[dice2 - 1].turned = true;
                        }
                        else if(choices[3] == "first")
                            tileG.tiles[dice1 - 1].turned = true;
                        else if(choices[3] == "second") 
                            tileG.tiles[dice2 - 1].turned = true;
                        break;

                }
            }

            Console.Clear();

            tileG.PrintRow();
            Utils.Divider('-', 100);
            Console.WriteLine();
            twoDice.Print();

            Console.WriteLine();
            Console.WriteLine("GAME OVER!");

            int tileCount = 0;
            foreach (Tile t in tileG.tiles)
            {
                if(t.turned == false)
                {
                    tileCount++;
                }
            }

            Console.WriteLine($"There are {tileCount} tiles remaining");
            Console.WriteLine();

            if(tileCount >=3 && tileCount <= 6)
            {
                Console.WriteLine("Gil won: 0");
                // no need to touch the gil
            }
            else if(tileCount >= 0 && tileCount <= 2)
            {
                Console.WriteLine($"Gil won: {gilToRisk}");
                Controller.playerGil += gilToRisk;
            }
            else if(tileCount >= 7)
            {
                Console.WriteLine($"Gil lost: {gilToRisk}");
                Controller.playerGil -= gilToRisk;
            }

            Console.WriteLine();

            string[] exitOptions = new string[2];

            if (Controller.playerGil > 0)
            {
                exitOptions[0] = "Play Again";
                exitOptions[1] = "Exit";
            }
            else
            {
                exitOptions[0] = "Exit";
            }

            menu = new Menu(exitOptions);

            switch(menu.GetInput())
            {
                case 1:
                    Render();
                    break;
                case 2:
                    Controller.Render();
                    break;
            }
        }
    }

    class BlackJack
    {
        double bet = 0;

        public void Render()
        {
            GilScreen();
            PlayGame();
        }

        void GilScreen()
        {
            Utils.BuildScreen("Black Jack");

            Console.WriteLine("Welcome to Black Jack!");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");

            Console.ReadKey();

        }

        void GetBet(bool error = false)
        {
            Console.Clear();

            Utils.BuildScreen("Black Jack");

            if(error)
            {
                Console.WriteLine("Invalid entry");
            }
            Console.WriteLine();
            Console.WriteLine($"You currently have {Controller.playerGil.ToString("F")} gil");
            Console.WriteLine();

            Console.Write("Gil to bet: ");

            if(!double.TryParse(Console.ReadLine(), out bet) || bet <= 0 || bet > Controller.playerGil)
            {
                GetBet(true);
            }

        }

        void BuildScreen(Deck deck, int playerTotal, List<Card> dealerCards, List<Card> playerCards)
        {

            Console.Clear();
            // print stuff for dealer
            Console.WriteLine();
            Console.WriteLine("DEALER");
            deck.PrintRow(dealerCards.ToArray());
            Console.WriteLine();
            Console.WriteLine();


            // print stuff for player
            Console.WriteLine($"Current Total: {playerTotal}");
            deck.PrintRow(playerCards.ToArray());
            Console.WriteLine();
        }

        void PlayGame()
        {
            GetBet();

            bool playerBust = false;
            int playerTotal = 0;

            Deck deck = new Deck(true);
            Menu menu;


            // get the cards to play with
            deck.Shuffle();
            Card[] playCards = deck.SelectCards(4);

            List<Card> dealerCards = playCards.Take(playCards.Length / 2).ToList();
            // we want one of the dealer cards to be visible
            dealerCards[0].hidden = false;

            List<Card> playerCards = playCards.Skip(playCards.Length / 2).ToList() ;
            // we want both the player cards to be visible
            playerCards[0].hidden = false; 
            playerCards[1].hidden = false;

            playerTotal = 0;
            foreach (Card c in playerCards)
            {
                playerTotal += c.Num;
            }

            while(!playerBust)
            {
                BuildScreen(deck, playerTotal, dealerCards, playerCards);

                string[] menuOptions =
                {
                    "Hit",
                    "Stand"
                };
                menu = new Menu(menuOptions);
                int input = menu.GetInput();

                if(input == 1)
                {
                    // inefficient
                    deck.Shuffle();
                    Card newC = deck.SelectCards(1)[0];
                    newC.hidden = false;
                    playerCards.Add(newC);
                       
                    playerTotal = 0;
                    foreach (Card c in playerCards)
                    {
                        playerTotal += c.Num;
                    }

                    if(playerTotal > 21)
                    {
                        playerBust = true;
                        BuildScreen(deck, playerTotal, dealerCards, playerCards);
                        Console.WriteLine("YOU BUST!");
                    }

                }
                else if(input == 2)
                {
                    break;
                }

            }

        }
    }

    static class Controller
    {
        static public double playerGil = 50; // player starts with 50 gil
        public const double GIL_GOAL = 300;

        static string[] menuItems =
        {
            "Card Shark",
            "Shut the Box",
            "Black Jack",
            "Exit",
        };

        static public void Render()
        {
            Utils.BuildScreen("Games");
            Menu menu;

            Console.WriteLine();

            if(playerGil <= 0)
            {
                string[] exitMenu = { 
                    "Reset Games and Play Again",
                    "Exit",
                };
                Console.WriteLine($"You have {playerGil} gil! You can no longer play.");

                menu = new Menu(exitMenu);

                switch(menu.GetInput())
                {
                    case 1:
                        Reset();
                        Render();
                        break;
                    case 2:
                        Utils.Exit();
                        break;
                }

            }

            Console.WriteLine($"You currently have {playerGil.ToString("F")} gil");
            if (playerGil < GIL_GOAL)
                Console.WriteLine($"You need {GIL_GOAL - playerGil} more gil to reach your goal of {GIL_GOAL} gil");
            else
                Console.WriteLine($"You reached your goal of {GIL_GOAL}!");


           menu = new Menu(menuItems);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Choose a game: ");

            switch (menu.GetInput())
            {
                case 1:
                    new CardShark().Render();
                    break;
                case 2:
                    new ShutTheBox().Render();
                    break;
                case 3:
                    new BlackJack().Render();
                    break;
                case 4:
                    Utils.Exit();
                    break;
            }
        }
        static public double GilScreen(string header, string description, bool error = false, bool toSmall = false, bool toMuch = false)
        {
            double gilToRisk;

            Utils.BuildScreen(header);

            Console.WriteLine();
            Console.WriteLine(description);
            Console.WriteLine();
            Utils.Divider('_', 50);

            Console.WriteLine();
            Console.WriteLine($"You currently have {Controller.playerGil} gil");
            Console.WriteLine();


            if (error)
            {
                Console.WriteLine("Invalid entry");
            }
            else if(toSmall)
            {
                Console.WriteLine("You cannot risk 0 gil!");
            }
            else if(toMuch)
            {
                Console.WriteLine("You cannot risk more gil than you have!");
            }

            Console.Write("Enter amount of gil to risk: ");

            if(!double.TryParse(Console.ReadLine(), out gilToRisk)) {
                GilScreen(header, description, true);
            }

            if(gilToRisk <= 0)
            {
                GilScreen(header, description, false, true);
            }

            if(gilToRisk > Controller.playerGil)
            {
                GilScreen(header, description, false, false, true);
            }

            if(Controller.playerGil <= 0)
                Controller.Render();

            return gilToRisk;
        }

        private static void Reset()
        {
            playerGil = 50;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // needed to make card symbols appear
            Console.SetWindowSize(130, 35); // make sure we have room to stuff. TODO: adjust this

            Controller.Render();

            Console.ReadKey();
        }

    }
}
