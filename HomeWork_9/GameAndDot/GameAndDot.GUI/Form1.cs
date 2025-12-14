using GameAndDot.Shared;
using GameAndDot.Shared.Enums;
using GameAndDot.Shared.Models;
using System.Net.Sockets;
using System.Text.Json;
using ProtocolFramework;
using XProtocol;
using XProtocol.Serializator;

namespace GameAndDot.GUI
{   
    
    public partial class Form1 : Form
    {   
        private static int _handshakeMagic;

        private int _myPlayerId;
        private Bitmap _canvas;
        private Graphics _graphics;
        private readonly Dictionary<int, Brush> PlayerBrushById = new();
        private readonly Dictionary<int, string> PlayerNameById = new();

        XClient client = new XClient();

        private string _username = "";

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            gameField.Enabled = false;
            client.OnPacketRecieve += Client_OnPacketRecieve;
            XPacketTypeManager.RegisterType(XPacketType.Handshake, 1, 0);
            XPacketTypeManager.RegisterType(XPacketType.PlayerConnected, 1, 1);
            XPacketTypeManager.RegisterType(XPacketType.Welcome, 1, 2);
            XPacketTypeManager.RegisterType(XPacketType.PlayerJoined, 1, 3);
            XPacketTypeManager.RegisterType(XPacketType.PointPlaced, 1, 4);
            try
            {
                client.Connect("127.0.0.1", 4910);
                var rand = new Random();
                _handshakeMagic = rand.Next();

                client.QueuePacketSend(
                    XPacketConverter.Serialize(
                        XPacketType.Handshake,
                        new XPacketHandshake { MagicHandshakeNumber = _handshakeMagic }
                    ).ToPacket()
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
             private void Client_OnPacketRecieve(byte[] raw)
        {
            var packet = XPacket.Parse(raw);
            if (packet == null) return;

            var type = XPacketTypeManager.GetTypeFromPacket(packet);
            Console.WriteLine($"RECV {type}");

            if (type == XPacketType.Welcome)
                Console.WriteLine($"WELCOME id={packet.GetValue<int>(1)}");

            if (type == XPacketType.PlayerJoined)
                Console.WriteLine($"JOIN id={packet.GetValue<int>(1)}");

            if (type == XPacketType.PointPlaced)
                Console.WriteLine($"POINT id={packet.GetValue<int>(1)}");

            switch (type)
            {
                case XPacketType.Welcome:
                    {
                        _myPlayerId = packet.GetValue<int>(1);

                        BeginInvoke(new Action(() =>
                        {
                            gameField.Enabled = true;
                        }));
                        break;
                    }
                case XPacketType.PlayerJoined:
                    {
                        int id = packet.GetValue<int>(1);
                        string username = packet.GetString(2);
                        string colorHex = packet.GetString(3);

                        PlayerNameById[id] = username;
                        PlayerBrushById[id] = new SolidBrush(ColorTranslator.FromHtml(colorHex));

                        BeginInvoke(new Action(UpdatePlayersList));
                        break;
                    }

                case XPacketType.PointPlaced:
                    {
                        int id = packet.GetValue<int>(1);
                        int x = packet.GetValue<int>(2);
                        int y = packet.GetValue<int>(3);

                        if (PlayerBrushById.TryGetValue(id, out var brush))
                            BeginInvoke(new Action(() => DrawDot(x, y, brush)));

                        break;
                    }
            }
        }
           

 
        


        private void Form1_Load(object sender, EventArgs e)
        {
            _canvas = new Bitmap(gameField.Width, gameField.Height);
            _graphics = Graphics.FromImage(_canvas);
            gameField.BackgroundImage = _canvas;
        }
     

     
    

        // ======= Вход =======
        private async void button1_Click(object sender, EventArgs e)
        {
            _username = textBox1.Text;

            label1.Visible = false;
            textBox1.Visible = false;
            button1.Visible = false;

            usernameLbl.Text = _username;

            usernameLbl.Visible = true;
            label2.Visible = true;
            label4.Visible = true;
            colorLbl.Visible = true;
            listBox1.Visible = true;
            gameField.Visible = true;






            var p = XPacket.Create(XPacketType.PlayerConnected);
            p.SetString(1, _username);
            client.QueuePacketSend(p.ToPacket());


        }

  
        private void gameField_MouseClick(object sender, MouseEventArgs e)
        {
            var p = XPacket.Create(XPacketType.PointPlaced);
            p.SetValue(1, _myPlayerId);
            p.SetValue(2, e.X);
            p.SetValue(3, e.Y);

            client.QueuePacketSend(p.ToPacket());

            // локально можно нарисовать
            if (PlayerBrushById.TryGetValue(_myPlayerId, out var brush))
                DrawDot(e.X, e.Y, brush);

        }

        // ======= Рисуем точку =======
        private void DrawDot(int x, int y, Brush brush)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => DrawDot(x, y, brush)));
                return;
            }

            _graphics.FillEllipse(brush, x - 5, y - 5, 10, 10);
            gameField.Invalidate();
        }







        private void UpdatePlayersList()
        {
            listBox1.Items.Clear();
            foreach (var name in PlayerNameById.Values)
                listBox1.Items.Add(name);
        }



    }
}
