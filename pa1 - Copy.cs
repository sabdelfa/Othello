#nullable enable
using System;
using static System.Console;

namespace Bme121
{
    record Player( string Colour, string Symbol, string Name );
    
    // The `record` is a kind of automatic class in the sense that the compiler generates
    // the fields and constructor and some other things just from this one line.
    // There's a rationale for the capital letters on the variable names (later).
    // For now you can think of it as roughly equivalent to a nonstatic `class` with three
    // public fields and a three-parameter constructor which initializes the fields.
    // It would be like the following. The 'readonly' means you can't change the field value
    // after it is set by the constructor method.
    
    //class Player
    //{
        //public readonly string Colour;
        //public readonly string Symbol;
        //public readonly string Name;
        
        //public Player( string Colour, string Symbol, string Name )
        //{
            //this.Colour = Colour;
            //this.Symbol = Symbol;
            //this.Name = Name;
        //}
    //}
	  
    static partial class Program
    {
        // Display common text for the top of the screen.
        static void Welcome( )
        {
			Console.Clear();
			WriteLine( );
			WriteLine( "~~ Welcome to Othello ~~" );
			WriteLine( );
        }
        
        // Collect a player name or default to form the player record.
        static Player NewPlayer( string colour, string symbol, string defaultName )
        {
            Write( "Type the {0} ({1}) player's name [or <Enter> for default name {2}]: ", colour, symbol, colour );
            string name = ReadLine( ); 
            
            if ( name.Length == 0 )
			{
				name = defaultName;
				//WriteLine("Invalid response, your name will be '{0}' by default.", name);
			}
			return new Player( colour, symbol, name );
        }
        
        // Determine which player goes first or default.
        static int GetFirstTurn( Player[ ] players, int defaultFirst )
        {
			Write( "Choose which player will go first (1 or 2): " );
			string response = ReadLine( );
			
			if ( response != "1" &&  response != "2" )
			{
				WriteLine($"Invalid response, {players[defaultFirst].Name} will go first by default");
				return defaultFirst; 
			}

			int firstPlayer = int.Parse( response );

			WriteLine($"{players[ firstPlayer - 1 ].Name} will go first.");
			
			return firstPlayer - 1;
        }
        
        // Get a board size (between 4 and 26 and even) or default, for one direction.
        static int GetBoardSize( string direction, int defaultSize )
        {
			while( true )
			{
				Write( "Enter board {0} (must be even numbers from 4 to 26) [or <Enter> for {1}]: ",
					direction, defaultSize );
					
				string size = ReadLine( );
				if ( size.Length == 0 )
				{
					return defaultSize;
				}
				
				int boardSize = int.Parse( size );
				if ( boardSize >= 4 && boardSize <= 26 && boardSize % 2 == 0)
				{
					return boardSize;
				}
				else
				{
					Write( "Invalid board size, please try again." );
				}
			}
        }
        
        // Get a move from a player.
        static string GetMove( Player player )
        {
			WriteLine( $"{player.Name}'s turn.");
            WriteLine( "\nPick a cell by its row then column names (like'bc') to play there." );
            WriteLine( "Type 'skip' to give up your turn. Type 'quit' to end the game." ); 
            
            Write( "Please enter your move: " );
            string move = ReadLine( );
            
            return move;
        }
        
        // Try to make a move. Return true if it worked.
        static bool TryMove( string[ , ] board, Player player, string move )
        {
			//Check if the user wants to skip turn:
			if ( move == "skip" )
			{
				return true;
			}
			
			//Check if the entered 2 characters:
			if ( move.Length != 2 )
			{
				WriteLine( "Invalid move. The move should be two (2) characters in length." );
				WriteLine( "One character is for the row and one for the column. " );
	
				return false;
			}

			//Checking rows and columns, and board boundaries:
			int row = IndexAtLetter( move.Substring ( 0, 1 ) ); 
			int column = IndexAtLetter( move.Substring ( 1, 1 ) ); 
							
			if ( row == -1 || column == -1 )
			{
				return false;
			}	
																		
            if ( row >= board.GetLength( 0 ) || column >= board.GetLength( 1 ) )
			{
				return false;
			}
			
			if ( board[ row, column ] != " " )
			{
				WriteLine( "This position is occupied." );
				return false;
			}
            
            //Checking the 8 directions:
            bool[ ] passedDirection = new bool[8]; //calling it 8 times	
			passedDirection[0] = TryDirection( board, player, row, -1, column,-1 ); // top left
        	passedDirection[1] = TryDirection( board, player, row, -1, column, 0 ); // top
        	passedDirection[2] = TryDirection( board, player, row, -1, column, 1 ); // top right
        	passedDirection[3] = TryDirection( board, player, row,  0, column, 1 ); // right
        	passedDirection[4] = TryDirection( board, player, row,  1, column, 1 ); // bottom right
        	passedDirection[5] = TryDirection( board, player, row,  1, column, 0 ); // bottom
        	passedDirection[6] = TryDirection( board, player, row,  1, column,-1 ); // bottom left
        	passedDirection[7] = TryDirection( board, player, row,  0, column,-1 ); // left
	
			foreach( bool direction in passedDirection )
			{
				if ( direction == true )
				{
					return true;
				}
			}
			return false;
        }
        
        // Do the flips along a direction specified by the row and column delta for one step.
       static bool TryDirection( string[ , ] board, Player player,
            int moveRow, int deltaRow, int moveCol, int deltaCol )
        {
			//Checking to see if we can move another step:
			int nextRow = moveRow + deltaRow; 
			if( nextRow < 0 || nextRow >= board.GetLength( 0 ) ) return false;
			int nextCol = moveCol + deltaCol;
			if( nextCol < 0 || nextCol >= board.GetLength( 1 ) ) return false;
			if( board[ nextRow, nextCol ] == player.Symbol ) return false;
		   
			bool validMove = true;
			int flipCounter = 1;

			//Loop to see if move is possible:
			while( validMove )
			{
				nextRow += deltaRow;
				nextCol += deltaCol;

				//Check if it is a valid move:
				if( nextRow < 0 || nextRow >= board.GetLength( 0 ) - 1 ) validMove = false;
				else if( nextCol < 0 || nextCol >= board.GetLength( 1 ) - 1 ) validMove = false;

				//Check if the move is in an empty location:
				else if( board[ nextRow, nextCol ] == " " ) validMove = false;
				
				//Flips:
				else if( board[  nextRow, nextCol ] != player.Symbol ) flipCounter++;
				else
				{
					for ( int i = 0; i <= flipCounter ; i++)
					{
						board[ moveRow, moveCol ] = player.Symbol;
						moveRow += deltaRow;
						moveCol += deltaCol;
					}
					return true;
				}
			}
			return false;
		}
	
        // Count the discs to find the score for a player.
        static int GetScore( string[ , ] board, Player player )
        {
			//Scan the rows and columns of the board and
			//Count how many elements are occupied by the player's symbol
			int playerScore = 0;
			for ( int i = 0; i < board.GetLength( 0 ); i++ )
			{
				for ( int j = 0; j < board.GetLength( 1 ); j++)
				{
					if ( board[ i, j ] == player.Symbol )
					{
						playerScore++;
					}
				}
			}
            return playerScore;
        }
        // Display a line of scores for all players.
        static void DisplayScores( string[ , ] board, Player[ ] players )
        {
			int playerScore1 = GetScore( board, players[ 0 ] );
			int playerScore2 = GetScore( board, players[ 1 ] );
			
			WriteLine($"\n{players[0].Name}'s score: {playerScore1} \t{players[1].Name}'s score: {playerScore2}" );
       
        }
        
        // Display winner(s) and categorize their win over the defeated player(s).
        static void DisplayWinners( string[ , ] board, Player[ ] players )
        {
			int[ ] finalScores = new int[ players.Length ];
			
			for( int i = 0; i < players.Length; i++ )
			{
				finalScores[ i ] = GetScore( board, players[ i ] );
			}
			
			int maxScore = finalScores[ 0 ];
			foreach( int finalScore in finalScores )
			{
				if ( finalScore > maxScore )
				{
					maxScore = finalScore;
				}
			}
			
			//Check to see if the game ends in a tie:
			bool tie = true;
			foreach( int finalScore in finalScores )
			{
				if ( finalScore < maxScore )
				{
					tie = false; 
				}
			}
			
			if( tie )
			{
				WriteLine( "It's a tie!" );
			}
			else
			{
				for ( int i = 0; i < players.Length; i++ )
				{
					if ( finalScores[ i ] == maxScore )
					{
						Write( "Winner is: {0}", players[ i ].Name );
					}
				}
			}
        }
        
        static void Main( )
        {
            // Set up the players and game.
            // Note: I used an array of 'Player' objects to hold information about the players.
            // This allowed me to just pass one 'Player' object to methods needing to use
            // the player name, colour, or symbol in 'WriteLine' messages or board operation.
            // The array aspect allowed me to use the index to keep track or whose turn it is.
            // It is also possible to use separate variables or separate arrays instead
            // of an array of objects. It is possible to put the player information in
            // global variables (static field variables of the 'Program' class) so they
            // can be accessed by any of the methods without passing them directly as arguments.
            
            Welcome( );
            
            Player[ ] players = new Player[ ] 
            {
                NewPlayer( colour: "black", symbol: "X", defaultName: "Black" ),
                NewPlayer( colour: "white", symbol: "O", defaultName: "White" ),
            };
            
            int turn = GetFirstTurn( players, defaultFirst: 0 );
           
            int rows = GetBoardSize( direction: "rows",    defaultSize: 8 );
            int cols = GetBoardSize( direction: "columns", defaultSize: 8 );
            
            string[ , ] game = NewBoard( rows, cols );
            
            // Play the game.
            
            bool gameOver = false;
            while( ! gameOver )
            {
                Welcome( );
                DisplayBoard( game ); 
                DisplayScores( game, players );
                
                string move = GetMove( players[ turn ] );
                if( move == "quit" ) gameOver = true;
                else
                {
                    bool madeMove = TryMove( game, players[ turn ], move );
                    if( madeMove ) turn = ( turn + 1 ) % players.Length;
                    else 
                    {
                        Write( " Your choice didn't work!" );
                        Write( " Press <Enter> to try again." );
                        ReadLine( ); 
                    }
                }
            }
            
            // Show fhe final results.
            
            DisplayWinners( game, players );
            WriteLine( );
        }
    }
}
