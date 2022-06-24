namespace youtubedownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       
        List<VideoLibrary.YouTubeVideo> videos = new List<VideoLibrary.YouTubeVideo>();

        VideoLibrary.YouTubeVideo video;

        private void Form1_Load(object sender, EventArgs e)
        {
            konum();
            radioButton1.Checked = true;
            video_adi.Text = "";
            video_cozunurluk.Text = "";

            panel2.Visible = false;

            progressBar2.Visible = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            bilgilerisifirla();
            listBox1.Items.Clear();
            string link = textBox1.Text;
            if (radioButton1.Checked == true)
                sesindir(link);
            if (radioButton2.Checked == true)
                videocek(link);

        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                foreach (VideoLibrary.YouTubeVideo video in videos)
                {
                    string res = video.Resolution.ToString();
                    if (listBox1.SelectedItem.ToString() == res)
                    {
                        videoindir(video);
                        bilgileriyazdir(video.FullName.ToString(), video.Resolution.ToString()+"P");
                        break;
                    }
                }
                }
            else
            {
                MessageBox.Show("L�tfen indirmek istedi�iniz ��z�n�rl��� listeden se�in.");
            }
            
            
            
        }

        

        private async void videocek(String link)
        {
            progressBar2.Visible = false;

            try
            {
                
                var youTube = VideoLibrary.YouTube.Default;
                foreach (VideoLibrary.YouTubeVideo video in youTube.GetAllVideos(link))
                {
                    videos.Add(video);
                    string res = video.Resolution.ToString();
                    if (!listBox1.Items.Contains(res) && res != "-1")
                    {
                        listBox1.Items.Add(res);
                    }

                    if (video_adi.Text == "")
                    {
                        bilgileriyazdir(video.FullName.ToString(), "-");
                    }
                }

                listBox1.Sorted = true;

                panel1.Visible = true;
                panel2.Visible = true;

            }
            catch
            {
                MessageBox.Show("Yazd���n�z link hatal� ya da video indirilemiyor.", "Hata");
            }
            
        }

        private async void videoindir(VideoLibrary.Video video)
        {
            progressBar1.Value = 30;
            if (listBox1.SelectedItem != null)
            {
                await Task.Run(() =>
                {
                    File.WriteAllBytes(indirmekonumu() + video.FullName, video.GetBytes());
                });

                progressBar1.Value = 100;
                MessageBox.Show("�ndirme tamamland�.", "Video indirildi");
                Thread.Sleep(100);
                progressBar1.Value = 0;
            }

            
        }


        private async void sesindir(String link)
        {
            
            try
            {

                var youTube = VideoLibrary.YouTube.Default;
                video = youTube.GetVideo(link);
                panel1.Visible = true;
                progressBar2.Visible = true;
                progressBar2.Value = 20;
                bilgileriyazdir(video.FullName.ToString(), "128Kbps");
                await Task.Run(() =>
                {
                    File.WriteAllBytes(indirmekonumu() + video.FullName, video.GetBytes());
                });

                progressBar2.Value = 80;

                Mp4ToMp3(indirmekonumu() + "\\" + video.FullName);

                progressBar2.Value = 100;
                MessageBox.Show("�ndirme tamamland�.", "Video indirildi");
                Thread.Sleep(100);
                progressBar2.Value = 0;
                progressBar2.Visible = false;
                
            }
            catch
            {
                MessageBox.Show("Yazd���n�z link hatal� ya da video indirilemiyor.", "Hata");
            }

            

        }

        

        private void Mp4ToMp3(String path)
        {
            var inputFile = new MediaToolkit.Model.MediaFile { Filename = path };
            var outputFile = new MediaToolkit.Model.MediaFile { Filename = $"{path}.mp3" };

            using (var engine = new MediaToolkit.Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);

                File.Delete(Path.Combine(path));
            }
        }

        private void bilgileriyazdir(String baslik, String cozunurluk)
        {
            video_adi.Text = baslik;
            video_cozunurluk.Text = cozunurluk;
        }

        private void bilgilerisifirla()
        {
            video_adi.Text = "";
            video_cozunurluk.Text = "";
        }
        private void konum()
        {
            
            String path = konumcek();
            System.IO.Directory.CreateDirectory(path);
        }

        private String konumcek()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\YoutubeDownloader";
        }

        private String indirmekonumu()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\YoutubeDownloader\\";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            indirmekonumuac();
        }
        private void indirmekonumuac()
        {
            System.Diagnostics.Process.Start("explorer.exe", indirmekonumu());
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false)
                radioButton2.Checked = true;
            if (radioButton1.Checked == true)
            {
                radioButton2.Checked = false;
                if (panel2.Visible == true)
                    panel2.Visible = false;
            }
                
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == false)
                radioButton1.Checked = true;
            if (radioButton2.Checked == true)
                radioButton1.Checked = false;
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button3.PerformClick();
            }
        }


    }
}