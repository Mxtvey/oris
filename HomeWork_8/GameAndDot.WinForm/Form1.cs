using GameAndDot.Shared.Enums;
using GameAndDot.Shared.Models;
using System.Net.Sockets;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GameAndDot.WinForm
{
    public partial class Form1 : Form
    {
        private int dotSize = 15;
        private readonly StreamReader? _reader;
        private readonly StreamWriter? _writer;
        private readonly TcpClient _client;
        private readonly Dictionary<string, Color> _playerColors = new();
        private readonly Dictionary<string, Point> _playerPositions = new();

        const string host = "127.0.0.1";
        const int port = 8888;

        public Form1()
        {
            InitializeComponent();

            _client = new TcpClient();

            gameField.Paint += gameField_Paint;
            gameField.Resize += (s, e) => gameField.Invalidate();

            try
            {
                _client.Connect(host, port); 

                _reader = new StreamReader(_client.GetStream());
                _writer = new StreamWriter(_client.GetStream());

              
                listBox1.DrawMode = DrawMode.OwnerDrawFixed;
                listBox1.DrawItem += ListBox1_DrawItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            textBox1.Visible = false;
            button1.Visible = false;

            label2.Visible = true;
            label4.Visible = true;
            usernameLbl.Visible = true;
            colorLbl.Visible = true;
            listBox1.Visible = true;
            gameField.Visible = true;

            string userName = textBox1.Text;
            usernameLbl.Text = userName;

        
            Task.Run(() => ReceiveMessageAsync());

            var message = new EventMessage()
            {
                Type = EventType.PlayerConnected,
                Username = userName
            };

            string json = JsonSerializer.Serialize(message);

         
            await SendMessageAsync(json);
        }

       
        async Task ReceiveMessageAsync()
        {
            while (true)
            {
                try
                {
                 
                    string? jsonRequest = await _reader.ReadLineAsync();
                    var messageRequest = JsonSerializer.Deserialize<EventMessage>(jsonRequest);

                    switch (messageRequest.Type)
                    {
                        case EventType.PlayerConnected:

                            Invoke(() =>
                            {
                                listBox1.Items.Clear();
                                _playerColors.Clear();

                                foreach (var name in messageRequest.Players)
                                {
                                    Color color = ColorTranslator.FromHtml(name.ColorHex);
                                    _playerColors[name.Username] = color;
                                    _playerPositions[name.Username] = new Point(name.DotX, name.DotY);
                                    listBox1.Items.Add(name.Username);
                                }

                                var somePlayer = messageRequest.Players.FirstOrDefault(p => p.Username == usernameLbl.Text);
                                if (somePlayer != null)
                                {
                                    colorLbl.Text = somePlayer.ColorHex;
                                    colorLbl.ForeColor = ColorTranslator.FromHtml(somePlayer.ColorHex);
                                }
                                gameField.Invalidate();
                            });

                            break;
                    }
                }
                catch
                {
                    break;
                }
            }
        }

   
        async Task SendMessageAsync(string message)
        {
        
            await _writer.WriteLineAsync(message);
            await _writer.FlushAsync();
        }

        private void ListBox1_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string username = listBox1.Items[e.Index].ToString();

            e.DrawBackground();

         
            Color color = _playerColors.ContainsKey(username)
                ? _playerColors[username]
                : Color.Black;

          
            using (Brush brush = new SolidBrush(color))
            {
                e.Graphics.DrawString(username, e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void gameField_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var kvp in _playerPositions)
            {
                string name = kvp.Key;
                Point pos = kvp.Value;

                if (!_playerColors.TryGetValue(name, out Color color)) continue;

                using SolidBrush brush = new SolidBrush(color);
                e.Graphics.FillEllipse(brush, pos.X, pos.Y, dotSize, dotSize);
            }
        }
    }
}
