using System;
using System.Collections.Generic;
using System.Linq;

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
        public int Num { get; private set; }
        public string DisplayVal { get; private set; }
        public char Symbol { get; private set; }
        public string StringRep { get; private set; }

        public bool hidden; // if the card is hidden we know not to show its value

        public Card(int num, string suit, bool hideCard = true)
        {
            Num = num;

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

        public Deck()
        {
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
            GillScreen();
            PlayGame();
        }
        void Init(bool displayHeader = true)
        {
            if (displayHeader)
                Utils.BuildScreen("Card Shark");
            else
                Console.Clear();
        }

        void GillScreen(bool error = false)
        {
            Init();

            if(error)
            {
                Console.WriteLine("Invalid entry");
            }

            Console.Write("Enter amount of gil to risk: ");

            if(!double.TryParse(Console.ReadLine(), out gilToRisk)) {
                GillScreen(true);
            }
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
                Console.WriteLine("Gil you are risking: " + gilToRisk.ToString("F"));
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
            Console.WriteLine();

            if(correctCount < 4)
            {
                Console.WriteLine("YOU BUST!");
            }
            else if(correctCount <= 5)
            {
                Console.WriteLine("YOU BROKE EVEN");
            }
            else if(correctCount < 9)
            {
                Console.WriteLine("YOU DOUBLED YOUR GIL!");
            }
            else if(correctCount == 9)
            {
                Console.WriteLine("YOU TRIPLED YOUR GIL");
            }


            Console.WriteLine();
            Console.WriteLine();

            string[] exitOptions =
            {
                "Play Again",
                "Exit"
            };
            menu = new Menu(exitOptions);

            switch(menu.GetInput())
            {
                case 1:
                    Render();
                    break;
                case 2:
                    new MainScreen().Render();
                    break;
            }

        }


    }


    class MainScreen
    {
        string[] menuItems =
        {
            "Card Shark",
            "Shut the Box",
            "Exit",
        };

        public void Render()
        {
            Utils.BuildScreen("Games");
                
            Menu menu = new Menu(menuItems);

            switch(menu.GetInput())
            {
                case 1:
                    new CardShark().Render();
                    break;
                case 2:
                    break;
                case 3:
                    Utils.Exit();
                    break;
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // needed to make card symbols appear
            Console.SetWindowSize(130, 35); // make sure we have room to show cards. TODO: adjust this

            MainScreen mainScreen = new MainScreen();
            mainScreen.Render();

            Console.ReadKey();
        }

    }
}
