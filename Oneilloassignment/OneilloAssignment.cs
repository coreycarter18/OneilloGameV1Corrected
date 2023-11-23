using Oneilloassignment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Speech.Synthesis; // enables speak
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



// extra space to make code easier to read



namespace Oneilloassignment
{
    public partial class Oneillogame : Form
    {
        private const int boardSize = 8; // sets board cells
        private const int cellSize = 50; // cell size
        private int[,] board = new int[boardSize, boardSize]; // creates 2D array for size of the board (8x8)
        private int currentPlayer = 1; // start with player 2 (white)
        private int blackCount = 0; // initialise black counter count
        private int whiteCount = 0;// initialise white counter count
        private SpeechSynthesizer synthesizer;
        





        public Oneillogame()
        {
            InitializeComponent();
            FormComponents();
            InitialiseBoard();
            SetBoard();
            synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;  // Sets volume
            synthesizer.Rate = 0;     // Sets speed 
        }





        private void FormComponents() 
        {
            int width = (boardSize * cellSize) + 20; 
            int height = boardSize * cellSize + cellSize * 4; 

            this.Text = "ONeillo Game V2"; 
            this.BackColor = Color.Pink; // sets pink background
            this.Size = new Size(width, height); // create a new size for the form, height and width as parameters

 




        }
        private void InitialiseBoard() // set initial board counter placements
        {
            board[3, 3] = board[4, 4] = 1; // set beginning black pieces
            board[3, 4] = board[4, 3] = 2; // set beginning white pieces

            UpdateInterface(); // update the board to display the starting counters
        }



        private void SetBoard() // create button controls representing black, white, and empty counters
        {
            for (int row = 0; row < boardSize; row++) // iterate over each row on the board
            {
                for (int col = 0; col < boardSize; col++) // iterate over each column on the board
                {
                    Button button = new Button(); // create new button control for every cell on the board
                    button.Size = new Size(cellSize, cellSize); // set all same size
                    button.Location = new Point(col * cellSize, row * cellSize + 30); // location of board
                    button.Click += new EventHandler(CellClicked); // set an event for when the cell is clicked by the user, ie - player moves
                    button.Name = "btn_" + row + "_" + col; // specific cells position on  board

                    if (board[row, col] == 1) // check if the current cell is occupied by a black counter
                    {
                        button.BackColor = Color.Black; // if yes =  set the colour of the button to black representing their counter
                        button.Enabled = false; // disable, no longer empty 
                    }
                    else if (board[row, col] == 2) // check if current cell is white counter
                    {
                        button.BackColor = Color.White; // if yes = set the colour of button to white to represent thei rcounter
                        button.Enabled = false; 
                    }
                    else
                    {
                        button.BackColor = Color.Pink; // sets unused cells to pink
                    }

                    Controls.Add(button); 
                }
            }
        }





        private void CellClicked(object sender, EventArgs e) // event handler for clicked cell
        {
            Button button = (Button)sender; // create instance of clicked button
            int row = button.Location.Y / cellSize; // calculate row of button
            int col = button.Location.X / cellSize; // calculate column of button

            if (ValidMove(row, col)) // check valid move
            {
                MakeMove(row, col); // if yes = make move
                SetBoard(); // the board

                if (GameOver()) // if the game is over...
                {
                    GameOverMessage(); // display message
                }
                else
                {
                    SwitchPlayer(); // if it isn't over... switch to other player
                }
            }
        }





        private void UpdateInterface() // updates score 
        {
            // Updates blackCount + whiteCount variables
            blackCount = 0;
            whiteCount = 0;
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (board[row, col] == 1)
                    {
                        blackCount++;
                    }
                    else if (board[row, col] == 2)
                    {
                        whiteCount++;
                    }
                }
            }


            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    Button button = (Button)Controls.Find("btn_" + row + "_" + col, true).FirstOrDefault();

                    if (button != null)
                    {
                        if (board[row, col] == 1)
                        {
                            button.BackColor = Color.Black;
                            button.Enabled = false;
                        }
                        else if (board[row, col] == 2)
                        {
                            button.BackColor = Color.White;
                            button.Enabled = false;
                        }
                        else
                        {
                            button.BackColor = Color.Pink; // set empty counters to pink to match form background 
                            button.Enabled = ValidMove(row, col); // enable buttons for valid moves
                        }
                    }
                }
            }

            playerCountWhite.Text = whiteCount.ToString();
            playerCountBlack.Text = blackCount.ToString();
        }






        private bool ValidMove(int row, int col) // checks for valid move 
        {
            if (board[row, col] != 0)
            {
                return false; // return false if invalid
            }

            int[] directionRow = { -1, -1, -1, 0, 1, 1, 1, 0 }; // checks for the 8 directions a move can make
            int[] directionCol = { -1, 0, 1, 1, 1, 0, -1, -1 };
            bool isValidMove = false; // move is false, unless it is proven true

            for (int i = 0; i < 8; i++)
            {
                int r = row + directionRow[i];
                int c = col + directionCol[i];
                bool foundOpponent = false;

                if (r < 0 || r >= boardSize || c < 0 || c >= boardSize || board[r, c] != OtherPlayer())
                {
                    continue;
                }

                while (true)
                {
                    r += directionRow[i];
                    c += directionCol[i];

                    if (r < 0 || r >= boardSize || c < 0 || c >= boardSize)
                    {
                        break;
                    }

                    if (board[r, c] == 0)
                    {
                        break;
                    }

                    if (board[r, c] == currentPlayer)
                    {
                        isValidMove = true;
                        break;
                    }
                }
            }

            return isValidMove;
        }






        private void MakeMove(int row, int col)
        {
            int[] directionRow = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] directionCol = { -1, 0, 1, 1, 1, 0, -1, -1 };

            board[row, col] = currentPlayer;

            int flippedCounters = 0; // keeps count

            for (int i = 0; i < 8; i++)
            {
                int r = row + directionRow[i];
                int c = col + directionCol[i];
                bool foundOpponent = false;

                if (r < 0 || r >= boardSize || c < 0 || c >= boardSize || board[r, c] != OtherPlayer())
                {
                    continue;
                }

                while (true)
                {
                    r += directionRow[i];
                    c += directionCol[i];

                    if (r < 0 || r >= boardSize || c < 0 || c >= boardSize)
                    {
                        break;
                    }

                    if (board[r, c] == 0)
                    {
                        break;
                    }

                    if (board[r, c] == currentPlayer)
                    {
                        while (r != row || c != col)
                        {
                            r -= directionRow[i];
                            c -= directionCol[i];
                            board[r, c] = currentPlayer;
                            flippedCounters++; // adds up after moves

                        }
                        break;
                    }
                }
            }







            UpdateInterface();  // to update counters
            if (synthesizer != null && synthesizer.State == SynthesizerState.Ready && speakToolStripMenuItem.Checked) 
            {
                string player = currentPlayer == 1 ? "Black" : "White";
                string move = $"Player {player} made a move at row {row} column {col}";

                synthesizer.SpeakAsync(move);
            }
            UpdateCounterValues(flippedCounters);
            UpdateCounterControls(); // Updates counter value
        }






        private void UpdateCounterValues(int flippedCounters)
        {
            if (currentPlayer == 1)
            {
                blackCount += flippedCounters + 1; // increases black score by new value
                whiteCount -= flippedCounters; // decreases whites score
            }
            else
            {
                whiteCount += flippedCounters + 1; // increases white score by new value
                blackCount -= flippedCounters; // decreases blacks score
            }
        }







        private void UpdateCounterControls()
        {

        } // needed for private void






        private bool GameOver()  // game over
        {
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (ValidMove(row, col))
                    {
                        return false;
                    }
                }
            }

            return true;
        }







        private void GameOverMessage()   // access black + white counter amounts
        {
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (board[row, col] == 1)
                    {
                        blackCount++;
                    }
                    else if (board[row, col] == 2)
                    {
                        whiteCount++;
                    }
                }
            }


            if (blackCount > whiteCount)
            {
                MessageBox.Show("black wins!"); // black is player 1, they will win if they have higher count than white 

            }
            else if (whiteCount > blackCount)
            {
                MessageBox.Show("white wins!"); // disaplys if white count is greater than black

            }
            else
            {
                MessageBox.Show("Draw!"); // draw if both counters are equal at end of the game
            }
        }







        private void SwitchPlayer()
        {
            currentPlayer = OtherPlayer();
            
            picturePlayer1.Visible = (currentPlayer == 1);
            picturePlayer2.Visible = (currentPlayer == 2);
        }




        private int OtherPlayer()
        {
            return currentPlayer == 1 ? 2 : 1;
        }

        private void InformationPanel_Enter(object sender, EventArgs e)  // to display information panel
        {

        }



        private void Oneillogame_Load(object sender, EventArgs e)
        {

        }



        private void pictureBox3_Click(object sender, EventArgs e)    
        {

        }
                                                                            // black and white counter display in information panel

        private void textBoxPlayer2_TextChanged(object sender, EventArgs e)
        {

        }




        private void informationPanelToolStripMenuItem_Click(object sender, EventArgs e)  // remove information panel from view when clicked
        {
            if (informationPanelToolStripMenuItem.Checked)
            {
                InformationPanel.Visible = true;
            }
            else
            {
                InformationPanel.Visible = false;
            }
        }







        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)  // start new game
        {

        }



        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) // about page information
        {
            using (Aboutpage aboutpage = new Aboutpage())
            {
                aboutpage.ShowDialog();
            }
        }





        private void speakToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }






       

        private void playerCountBlack_Click(object sender, EventArgs e) // number of player tokens on board to display
        {

        }





        private void playerCountWhite_Click(object sender, EventArgs e)  // number of player tokens on board to display
        {

        }







        private void speakToolStripMenuItem_Click_1(object sender, EventArgs e) //allows to enable Speak when clicked
        {
            if (speakToolStripMenuItem.Checked)  
            {
                speakToolStripMenuItem.Checked = false;
            }
            else
            {
                speakToolStripMenuItem.Checked = true;
            }
        }






        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e) // will show my About page when clicked 
        {
            using (Aboutpage aboutpage = new Aboutpage())
            {
                aboutpage.ShowDialog();  
            }

        }









        private void pictureBox3_Click_1(object sender, EventArgs e)    // Displays 'Your Turn' indicator to whos player turn it is
        {

        }

        private void picturePlayer2_Click(object sender, EventArgs e)
        {

        }
    }
}

