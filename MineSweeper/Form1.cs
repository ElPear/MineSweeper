using static MineSweeper.Form1;

namespace MineSweeper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            this.Size = new Size(25 + tileSize * arenaSize + 10, 75 + tileSize * 8 + 10);
            int margin = (this.Bounds.Width - (button1.Width + label1.Width + label2.Width + comboBox1.Width)) / 2;
            button1.Left = margin;
            label1.Left = button1.Right;
            label2.Left = label1.Right;
            comboBox1.Left = label2.Right;
            comboBox1.SelectedIndex = 0;
        }
        System.Media.SoundPlayer player = new System.Media.SoundPlayer("Win.wav");

        bool gameOver = false;
        int time = 0;
        int mines;
        int flags;
        int minesPlaced = 0;
        int tileSize = 30;
        int arenaSize = 28;
        Tile[,] grid;
        public partial class Tile : Button
        {
            private int adjacentMines;
            private bool mine;
            private bool covered;
            private bool marked;
            public bool Mine
            {
                get { return mine; }
                set { mine = value; }
            }
            public int AdjacentMines
            {
                get { return adjacentMines; }
                set { adjacentMines = value; }
            }
            public bool Covered
            {
                get { return covered; }
                set { covered = value; }
            }
            public bool Marked
            {
                get { return marked; }
                set { marked = value; }
            }
        }
        private bool GameWon()
        {
            bool isOver = true;
            foreach (Tile item in grid)
            {
                if (!item.Marked && item.Mine)
                    isOver = false;
            }
            foreach (Tile item in grid)
            {
                if (item.Covered && !item.Mine)
                {
                    isOver = false;
                }
            }
            return isOver;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Easy":
                    arenaSize = 8;
                    break;
                case "Medium":
                    arenaSize = 12;
                    break;
                case "Hard":
                    arenaSize = 20;
                    break;
                case "Very Hard":
                    arenaSize = 28;
                    break;
                default:
                    break;
            }
            timer1.Start();
            time = 0;
            gameOver = false;
            mines = (int)Math.Round(arenaSize * arenaSize * 0.20/10)*10;
            flags = mines;
            label1.Text = $"Flags: {flags}";

            this.Size = new Size(25 + tileSize * 28 + 10, 75 + tileSize * arenaSize + 10);
            StartGame();
        }
        private void PlaceMine()
        {
            Random rng = new();

            int x = rng.Next(0, arenaSize);
            int y = rng.Next(0, arenaSize);

            if (grid[x, y].Mine)
            {
                PlaceMine();
            }
            else
            {
                grid[x, y].Mine = true;
                //grid[x, y].BackColor = Color.IndianRed;
                minesPlaced++;
            }

        }
        private void MarkTile(Tile tile)
        {

            if (tile.Marked)
            {
                flags++;
                GetColor(tile);
                tile.Marked = false;
                tile.Text = "";
            }
            else
            {
                if (!tile.Covered)
                {
                    return;
                }
                else
                {
                    if (flags > 0)
                    {
                        flags--;
                        tile.ForeColor = Color.Black;
                        tile.Marked = true;
                        tile.Text = "X";
                    }
                }
            }
            label1.Text = $"Flags: {flags}";
            if (GameWon())
            {
                player.Play();
                MessageBox.Show("yippie");
                timer1.Stop();
            }
        }
        private void CheckTile(object sender, MouseEventArgs e)
        {
            if (gameOver)
            {
                return;
            }

            Tile tile = (Tile)sender;

            int x = int.Parse(tile.Name.Replace("Tile", "").Split("-")[0]);
            int y = int.Parse(tile.Name.Replace("Tile", "").Split("-")[1]);
            if (e.Button == MouseButtons.Left)
            {
                if (tile.Marked)
                {
                    return;
                }
                if (tile.Mine)
                {
                    tile.ForeColor = Color.Black;
                    tile.Text = "O";
                    foreach (Tile item in grid)
                    {
                        if (item.Mine)
                        {
                            item.BackColor = Color.IndianRed;


                        }
                    }
                    timer1.Stop();
                    gameOver = true;
                    return;
                }
                if (tile.Covered)
                {

                    tile.Covered = false;
                    tile.Text = tile.AdjacentMines.ToString();
                    CheckClose(tile);
                }
                //
                int marked = 0;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        try
                        {
                            if (grid[x + i, y + j].Marked && grid[x + i, y + j].Covered)
                                marked++;
                        }
                        catch { }

                    }
                }

                if (tile.AdjacentMines == marked && tile.AdjacentMines != 0)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            try
                            {
                                if (!grid[x + i, y + j].Marked && grid[x + i, y + j].Covered)
                                {
                                    if (grid[x + i, y + j].Mine && !grid[x + i, y + j].Marked)
                                    {
                                        grid[x + i, y + j].ForeColor = Color.Black;

                                        grid[x + i, y + j].Text = "O";

                                        foreach (Tile item in grid)
                                        {
                                            if (item.Mine)
                                            {
                                                item.BackColor = Color.IndianRed;
                                            }
                                        }
                                        timer1.Stop();
                                        gameOver = true;
                                        return;
                                    }
                                    grid[x + i, y + j].Covered = false;
                                    grid[x + i, y + j].Text = grid[x + i, y + j].AdjacentMines.ToString();
                                    if (grid[x + i, y + j].AdjacentMines == 0)
                                    {
                                        CheckClose(grid[x + i, y + j]);
                                    }
                                }




                            }
                            catch { }

                        }
                    }
                }



            }
            else if (e.Button == MouseButtons.Right)
            {
                MarkTile(tile);
            }
        }
        private void CheckClose(Tile tile)
        {
            int x = int.Parse(tile.Name.Replace("Tile", "").Split("-")[0]);
            int y = int.Parse(tile.Name.Replace("Tile", "").Split("-")[1]);

            if (grid[x, y].AdjacentMines == 0)
            {
                grid[x, y].ForeColor = Color.Gray;
                grid[x, y].BackColor = Color.Gray;
            }

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    try
                    {
                        if (!grid[x + i, y + j].Mine && grid[x + i, y + j].Covered && grid[x, y].AdjacentMines == 0)
                        {

                            grid[x + i, y + j].Covered = false;
                            grid[x + i, y + j].Text = grid[x + i, y + j].AdjacentMines.ToString();
                            CheckClose(grid[x + i, y + j]);
                        }
                    }
                    catch { }
                }
            }



        }
        private void StartGame()
        {
            try
            {
                foreach (Tile item in grid)
                {
                    Controls.Remove(item);
                    minesPlaced = 0;
                }
            }
            catch
            {

            }

            int margin = (tileSize * 28 - tileSize * arenaSize) / 2;
            grid = new Tile[arenaSize, arenaSize];
            for (int x = 0; x < arenaSize; x++)
            {
                for (int y = 0; y < arenaSize; y++)
                {
                    grid[x, y] = new Tile()
                    {
                        Name = $"Tile{x}-{y}",
                        Width = tileSize,
                        Height = tileSize,
                        Location = new Point(10 + margin + tileSize * x, 35 + tileSize * y),
                        BackColor = Color.LightGray,
                        Covered = true,
                        Mine = false,

                    };
                    grid[x, y].MouseDown += new MouseEventHandler(this.CheckTile);
                    grid[x, y].Font = new Font(grid[x, y].Font.Name, grid[x, y].Font.Size, FontStyle.Bold);
                    grid[x, y].FlatStyle = FlatStyle.Popup;
                    Controls.Add(grid[x, y]);
                }
            }
            while (minesPlaced != mines)
            {
                PlaceMine();
            }
            foreach (Tile tile in grid)
            {
                int adjacentMines = 0;
                int x = int.Parse(tile.Name.Replace("Tile", "").Split("-")[0]);
                int y = int.Parse(tile.Name.Replace("Tile", "").Split("-")[1]);

                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        try
                        {
                            if (grid[x + i, y + j].Mine)
                                adjacentMines++;
                        }
                        catch { }

                    }
                }
                tile.AdjacentMines = adjacentMines;

                GetColor(tile);
            }
        }
        private static void GetColor(Tile tile)
        {
            switch (tile.AdjacentMines)
            {
                case 0:
                    tile.ForeColor = Color.LightGray;
                    break;
                case 1:
                    tile.ForeColor = Color.Blue;
                    break;
                case 2:
                    tile.ForeColor = Color.Green;
                    break;
                case 3:
                    tile.ForeColor = Color.Red;
                    break;
                case 4:
                    tile.ForeColor = Color.DarkBlue;
                    break;
                case 5:
                    tile.ForeColor = Color.DarkRed;
                    break;
                case 6:
                    tile.ForeColor = Color.DarkCyan;
                    break;
                case 7:
                    tile.ForeColor = Color.Black;
                    break;
                case 8:
                    tile.ForeColor = Color.DarkGray;
                    break;
                default:
                    break;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            label2.Text = $"Time: {time}";
        }
    }
}