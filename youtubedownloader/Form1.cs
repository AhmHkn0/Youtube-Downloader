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

        bool videobulundu;

        private void Form1_Load(object sender, EventArgs e)
        {
            videobulundu = false;
            konum();
            radioButton1.Checked = true;
            video_adi.Text = "";
            video_cozunurluk.Text = "";

            panel2.Visible = false;

            progressBar2.Visible = false;
        }

        private void buton_videokontrol_Click_1(object sender, EventArgs e)
        {
            bilgilerisifirla();
            listBox1.Items.Clear();
            string link = textBox1.Text;
            if (radioButton1.Checked == true)
                sesindir(link);
            if (radioButton2.Checked == true)
                videocek(link);
        }



        private void buton_indir_Click(object sender, EventArgs e)
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
                MessageBox.Show("Lütfen indirmek istediðiniz çözünürlüðü listeden seçin.");
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
                videobulundu = true;
                panel1.Visible = true;
                panel2.Visible = true;

            }
            catch
            {
                MessageBox.Show("Yazdýðýnýz link hatalý ya da video indirilemiyor.", "Hata");
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
                MessageBox.Show("Ýndirme tamamlandý.", "Video indirildi");
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
                    File.WriteAllBytes(myvideoskonum() + video.FullName, video.GetBytes());
                });

                progressBar2.Value = 80;

                Mp4ToMp3(myvideoskonum() + "\\" + video.FullName ,indirmekonumu() + "\\" + video.FullName);

                progressBar2.Value = 100;
                MessageBox.Show("Ýndirme tamamlandý.", "Video indirildi");
                Thread.Sleep(100);
                progressBar2.Value = 0;
                progressBar2.Visible = false;
                
            }
            catch (Exception ex)
            {
                 MessageBox.Show("Yazdýðýnýz link hatalý ya da video indirilemiyor.", "Hata");
            }

            

        }

        

        private void Mp4ToMp3(String from, String to)
        {
            var inputFile = new MediaToolkit.Model.MediaFile { Filename = from };
            var outputFile = new MediaToolkit.Model.MediaFile { Filename = $"{to}.mp3" };

            using (var engine = new MediaToolkit.Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);

                File.Delete(Path.Combine(from));
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

        private String myvideoskonum()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\";
        }

        private String indirmekonumu()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\YoutubeDownloader\\";
        }
        private void buton_indirilenklasorac_Click(object sender, EventArgs e)
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
            {
                radioButton2.Checked = true;
                if (videobulundu == true)
                    panel2.Visible = true;
            }
                
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
                buton_videokontrol.PerformClick();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems != null) 
            {
                label2.Text = "";
                string item = listBox1.SelectedItem.ToString();
                if (item != "144" && item != "240" && item != "360" && item != "480" && item != "720" && item != "1080")
                {
                    label2.Text = "Videoyu seçtiðiniz çözünürlükte\nindirirseniz ses olmayacaktýr.";
                    label2.ForeColor = Color.Red;
                }
            }
            
        }

        
    }
}