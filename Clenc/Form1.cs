using Clenc.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clenc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string TimeKoniec = "00:00:01";
        public string TimeStart = "00:00:00";
        public int sekundywideo = 1;
        public string textsekundy = "1";
        public string poczatek = "0";
        public string miejsceZapisu = "Brak";
        public string plikzrodlowy = "Brak";
        public BigInteger bitratewideo = 1000;
        public string mapowanieaudio = "-map 0:a:0";
        public string bitratewideouser = "1000k";
        public int sekundykoniec = 10;
        public string priority = "start /B /W /low ";
        public BigInteger oczekiwanawielkoscpliku = 65536;
        public int megabajty = 8;
        public BigInteger bajty = 8388605;
        public int enctimer;
        public string fpschange;
        public string usersettingstext;
        public string audiofade;
        public int audiofadeout;
        public int TotalSeconds = 20;
        readonly Process process = new Process();
        readonly ProcessStartInfo startInfo = new ProcessStartInfo();
        readonly Process process2 = new Process();
        readonly ProcessStartInfo startInfo2 = new ProcessStartInfo();
        public int encodertimesec;
        public string mapowanieaudio2k;
        public string kodekwideo = "libvpx-vp9";
        public int laginframesnum = 25;
        public int zapasdladzwieku;
        public string mpvdirexist;
        public string addmin;
        public string pauza;
        public string fileName;
        public int bitmapn;
        public string videodimensions;
        public BigInteger wielkoscaudioobliczony;
        public BigInteger przewidywanybitrateaudio;
        public int przewidywanybitrateaudioKb;
        public int RawVideo;
        public int kanalyaudio = 2;
        public string voipoptimizationcmd;
        public BigInteger wielkoscplikucurrent;
        public FileInfo CurrentFileSize;
        public string nvaccel = "-hwaccel dxva2";
        public string arnrplus = " -arnr_max_frames 15 -arnr_strength 6 -arnr_type 3";
        public int altrefenabled = 1;
        public int zbytdlugiczas = 0;
        public int FsErrCount = 0;
        public int dopuszczalnybitrateaudio = 128;
        public string removemetacmd = " -map_metadata -1";
        public int tiles = 0;

        private void ustawpoczatek_Click(object sender, EventArgs e)
        {
            TimeStart = wmp1.Ctlcontrols.currentPositionString;
            if (TimeStart != "")
            {
                if (TimeStart.Length > 5)
                {
                    poczatektextmask.Text = TimeStart;
                }
                else
                {
                    poczatektextmask.Text = "00:" + TimeStart;
                }
                WyznaczanieCzasu();
            }
            else
            {
                TimeStart = "00:00:00";
                poczatektextmask.Text = TimeStart;
            }
        }

        private void wyznaczkoniec_Click(object sender, EventArgs e)
        {
            TimeKoniec = wmp1.Ctlcontrols.currentPositionString;
            if (TimeKoniec != "")
            {
                if (TimeKoniec.Length > 5)
                {
                    koniectextmask.Text = TimeKoniec;
                }
                else
                {
                    koniectextmask.Text = "00:" + TimeKoniec;
                }
                WyznaczanieCzasu();
            }
            else
            {
                timerdatadownload.Enabled = true;
            }
        }

        private void WyznaczanieCzasu()
        {
            maskerror.Visible = false;
            poczatek = poczatektextmask.Text;
            string koniec = koniectextmask.Text;
            if (TimeSpan.TryParse(poczatek, out _) && TimeSpan.TryParse(koniec, out _))
            {
                double startseconds = TimeSpan.Parse(poczatek).TotalSeconds;
                int sekundypoczatek = (int)startseconds;
                double stopseconds = TimeSpan.Parse(koniec).TotalSeconds;
                sekundykoniec = (int)stopseconds;
                sekundywideo = sekundykoniec - sekundypoczatek;
                textsekundy = sekundywideo.ToString();
                dlugosctext.Text = textsekundy + "s";
                bitratelabelinf.Text = "Przewidywany bitrate: " + wielkosciplikubox.Text + "MB / " + sekundywideo + "s = " + bitratewideo + "kb/s";
            }
            else
            {
                maskerror.Visible = true;
                return;
            }
            if (sekundywideo <= 0)
            {
                dlugosctext.Text = "Długość filmu nie może być ujemna";
            }
        }

        private void PokazOknoZapisu()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "Nagranie WEBM|*.webm|Nagranie MKV|*.mkv|Tylko dźwięk OGG|*.ogg",
                Title = "Wybierz miejsce zapisu nagrania",
                FilterIndex = 1,
                FileName = Path.GetFileNameWithoutExtension(fileName)
            };
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                miejsceZapisu = saveFileDialog1.FileName;
                saveplacetext.Text = miejsceZapisu;
                tabControl1.SelectTab("tabPage4");
                ObliczanieBitrateDiscord();
                ObliczanieAF();
                WyznaczanieCzasu();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PokazOknoZapisu();
        }

        public void ObliczanieBitrateDiscord()
        {
            if (sekundywideo > 0)
            {
                bitratewideo = (oczekiwanawielkoscpliku / sekundywideo) - audiozapassuwak.Value;
                bitratelabelinf.Text = "Przewidywany bitrate: " + wielkosciplikubox.Text + "MB / " + sekundywideo + "s = " + bitratewideo + "kb/s";
                GenerowanieCMDVideo();
                GenerowanieCMDAudio();
            }
        }

        public void GenerowanieCMDVideo()
        {
            if (usersettings.Checked == true)
            {
                usersettingstext = usercommandchange.Text + " ";
            }
            string cmdtext = "ffmpeg -y -hide_banner " + nvaccel + " -ss " + poczatek + advancedstarttime.Text + " -i " + '"' + plikzrodlowy + '"' + " -c:v " + kodekwideo + " -cpu-used " + encspdtrack.Value + arnrplus + removemetacmd + " -row-mt 1 -g 990 -pix_fmt yuv420p -tile-rows " + tiles + " -tile-columns " + tiles + " -threads " + Environment.ProcessorCount + " -lag-in-frames " + laginframesnum + " -auto-alt-ref " + altrefenabled + " " + fpschange + " -an -map 0:v:0 " + usersettingstext + "-b:v " + bitratewideo + "k -t " + sekundywideo + advancedendtime.Text + " " + videodimensions + " -f webm -passlogfile CLENCguilog -pass ";
            if (!zdalnyRenderToolStripMenuItem.Checked)
            {
                liniakomend.Text = cmdtext + "1 -";
                liniakomend2.Text = cmdtext + "2 .\\tempdir\\videop2.webm";
            }
            else
            {
                liniakomend.Text = cmdtext + "1 videop1.mp4";
                liniakomend2.Text = cmdtext + "2 videop2.mp4";
            }
        }

        public void GenerowanieCMDAudio()
        {
            liniakomendaudio.Text = "ffmpeg -y -ss " + poczatek + " -i " + '"' + plikzrodlowy + '"' + " " + mapowanieaudio + " -vn " + voipoptimizationcmd + audiofade + "-t " + sekundywideo + removemetacmd + " -c:a libopus -frame_duration 120 -ac " + kanalyaudio + " -b:a ";
        }

        public void ObliczanieAF()
        {
            int afoutint = sekundywideo - 1;
            audiofade = "-af afade=t=in:d=1,afade=t=out:d=1:st=" + afoutint + ".000 ";
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (plikzrodlowy != "Brak")
            {
                if (miejsceZapisu != "Brak")
                {
                    if (!File.Exists("ffmpeg.exe"))
                    {
                        MessageBox.Show("Wgraj bibliotekę FFMPEG zanim rozpoczniesz.", "Brak wymaganych bibliotek", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        RenderowanieNagrania();
                    }
                }
                else
                {
                    tabControl1.SelectTab("tabPage3");
                }
            }
            else
            {
                tabControl1.SelectTab("tabPage1");
            }
        }

        public void RenderowanieNagrania()
        {
            Kolejka.Text = "💾 " + saveplacetext.Text + " | ⏱" + sekundywideo + "s | 📏" + wielkosciplikubox.Text + "MB";
            encodertimesec = 0;
            tabControl1.SelectTab("renderTab");
            splitContainer1.Visible = false;
            WylaczForms();
            if (wyłaczOptymalizacjęWMPToolStripMenuItem.Checked == true)
            {
                wmp1.close();
            }
            killprocess.Visible = true;
            fserrorinf.Visible = false;
            encprogressbar.Style = ProgressBarStyle.Marquee;
            status.Text = "Przygotowywanie...";
            Process processX = new Process();
            ProcessStartInfo startInfoX = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C del /S /Q .\\tempdir & del /Q CLENCguilog-0.log & cd . & mkdir .\\tempdir"
            };
            processX.StartInfo = startInfoX;
            processX.Start();
            processX.WaitForExit();
            bitmapn++;
            if (zdalnyRenderToolStripMenuItem.Checked == true)
            {
                ZdalnyRenderE1();
                return;
            }

            Process processimg = new Process();
            ProcessStartInfo startInfoImg = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C ffmpeg -y " + nvaccel + " -ss " + poczatek + " -i " + '"' + plikzrodlowy + '"' + " -map 0:v:0 -filter_complex smartblur=lr=5,eq=brightness=-0.3 -q:v 1 -frames:v 1 .\\tempdir\\backgroundimg" + bitmapn + ".jpg"
            };
            processimg.StartInfo = startInfoImg;
            processimg.Start();

            progresscircle.Image = Resources.aniload;
            renderTab.Text = "🔄 Renderowanie [Etap 1/3]";

            if (!AudioOnlyCheck.Checked)
            {
                if (niePokazujOknaWierszaPoleceńToolStripMenuItem.Checked == true)
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }
                else
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                }
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C " + priority + liniakomend.Text + pauza;
                process.StartInfo = startInfo;
                process.Start();
                processexited1.Enabled = true;
            }
            else
            {
                GenerowanieDzwieku();
                return;
            }
        }

        public void ZdalnyRenderE1()
        {
            status.Text = "Trwa szyfrowanie nagrania źródłowego...";
            Process processimg = new Process();
            ProcessStartInfo startInfoImg = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                FileName = "cmd.exe",
                Arguments = "/C ffmpeg -y -hide_banner -i " + '"' + plikzrodlowy + '"' + " -c copy -f mp4 -strict -2 -to " + koniectextmask.Text + " -movflags +faststart -encryption_scheme cenc-aes-ctr -encryption_key 4d696368616c4a657354686542657374 -encryption_kid 30303030303030303030303030303031 " + '"' + miejsceZapisu + "-video.mp4" + '"'
            };
            processimg.StartInfo = startInfoImg;
            processimg.Start();
            processimg.WaitForExit();

            nvaccel = "-decryption_key 4d696368616c4a657354686542657374";

            Process processaudio = new Process();
            ProcessStartInfo startInfoaudio = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                FileName = "cmd.exe",
                Arguments = "/C ffmpeg -y -hide_banner -ss " + poczatek + " -i " + '"' + plikzrodlowy + '"' + " -vn -strict -2 -c copy -t " + sekundywideo + " -f mp4 " + '"' + miejsceZapisu + "-audioonly.m4a" + '"'
            };
            processaudio.StartInfo = startInfoaudio;
            processaudio.Start();
            processaudio.WaitForExit();

            plikzrodlowy = "oryginal.mp4";
            usersettings.Checked = true;
            usercommandchange.Text = usercommandchange.Text + " -movflags +faststart -encryption_scheme cenc-aes-ctr -encryption_key 4d696368616c4a657374426573743037 -encryption_kid 30303030303030303030303030303031";
            GenerowanieCMDVideo();
            string[] lines =
                {
                    liniakomend.Text, liniakomend2.Text, liniakomendaudio.Text
                };

            File.WriteAllLines("Komendy.txt", lines);
            MessageBox.Show("W folderze aplikacji znajdują się wszystkie potrzebne pliki. Przenieś oryginal.mp4 na zewnętrzny komputer i wklej komendy znajdujące się w Komendy.txt","Gotowe do wykonania etapu 1", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Exit();
        }

        public void GenerowanieDzwieku()
        {
            if (AudioOnlyCheck.Checked != true)
            {
                FileInfo Video = new FileInfo(".\\tempdir\\videop2.webm");
                RawVideo = (int)Video.Length;
            }
            else
            {
                RawVideo = 0;
            }

            wielkoscaudioobliczony = bajty - RawVideo;
            przewidywanybitrateaudio = BigInteger.Divide(wielkoscaudioobliczony, sekundywideo);
            przewidywanybitrateaudioKb = (int)BigInteger.Divide(przewidywanybitrateaudio, 128);

            if (przewidywanybitrateaudioKb > 160)
                przewidywanybitrateaudioKb = 160;

            if (precisiontrack.Value == 2)
                przewidywanybitrateaudioKb += 18;
            
            if (precisiontrack.Value == 1)
                przewidywanybitrateaudioKb += 9;

            if (przewidywanybitrateaudioKb < 6)
            {
                przewidywanybitrateaudioKb = 6;
                fserrorinf.Visible = true;
            }

            TworzenieAudio();
        }

        public void TworzenieAudio()
        {
                status.Text = "[Etap 3/3] Enkodowanie Audio " + przewidywanybitrateaudioKb + "kb/s";
                soundbitrateinf.Text = przewidywanybitrateaudioKb + "kb/s";
                Process soundprocess = new Process();
                ProcessStartInfo startInfoS = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C " + liniakomendaudio.Text + przewidywanybitrateaudioKb + "k .\\tempdir\\opus" + przewidywanybitrateaudioKb + "k.opus"
                };
                soundprocess.StartInfo = startInfoS;
                soundprocess.Start();
                soundprocess.WaitForExit();

                if (AudioOnlyCheck.Checked != true)
                {
                    if (!File.Exists(".\\tempdir\\videop2.webm") || !File.Exists(".\\tempdir\\opus" + przewidywanybitrateaudioKb + "k.opus"))
                    {
                        MessageBox.Show("Wygląda na to, że wystąpił błąd z enkodowaniem. Powtórz proces wraz z włączoną opcją " + '"' + "Pauza na końcu enkodowania(diagnost.)" + '"' + " z menu kontekstowego przycisku " + '"' + "Rozpocznij Enkodowanie" + '"', "Błąd enkodowania", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        zabijprocess();
                        return;
                    }
                    else
                    {
                        SprawdzanieWielkosciPliku();
                    }
                }
                else
                {
                    SprawdzanieWielkosciPliku();
                }
        }

        public void SprawdzanieWielkosciPliku()
        {
            FileInfo Opus1F = new FileInfo(".\\tempdir\\opus" + przewidywanybitrateaudioKb + "k.opus");
            int Opus1size = (int)Opus1F.Length;
            int fullsize = RawVideo + Opus1size;
                soundbitrateinf.Text = przewidywanybitrateaudioKb + "kb/s (" + Opus1size / 1024 + "KB)";
            if (fullsize < bajty)
            {
                if (optimizebits.Checked == true && audiozapassuwak.Value != 0 && przewidywanybitrateaudioKb > 144)
                {
                        audiozapassuwak.Value = 0;
                        optimizebits.Checked = false;
                        zapasdzwiektext.Text = "0kb/s";
                        var notification = new NotifyIcon()
                        {
                            Visible = true,
                            Icon = SystemIcons.Warning,
                            BalloonTipText = "Wyrenderowane nagranie zajmuje mniej miejsca niż jest ustawione. Renderowanie zostanie powtórzone.",
                            BalloonTipTitle = "Wymuszona optymalizacja wielkości"
                        };
                        if (wyświetlPowiadomieniePoZakończeniuProcesuToolStripMenuItem.Checked == true)
                        {
                            notification.ShowBalloonTip(7000);
                        }
                        GenerowanieCMDVideo();
                        GenerowanieCMDAudio();
                        RenderowanieNagrania();
                        return;
                } 

                if (AudioOnlyCheck.Checked != true)
                    {
                        Process soundprocess2 = new Process();
                        ProcessStartInfo soundstartInfo2 = new ProcessStartInfo
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            FileName = "cmd.exe",
                            Arguments = "/C ffmpeg -y -i .\\tempdir\\videop2.webm -i .\\tempdir\\opus" + przewidywanybitrateaudioKb + "k.opus " + liniakomendmeta.Text + " -c copy " + mapowanieaudio2k + '"' + miejsceZapisu + '"'
                        };
                        soundprocess2.StartInfo = soundstartInfo2;
                        soundprocess2.Start();
                        soundprocess2.WaitForExit();
                    }
                    else
                    {
                        Process soundprocess2 = new Process();
                        ProcessStartInfo soundstartInfo2 = new ProcessStartInfo
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            FileName = "cmd.exe",
                            Arguments = "/C ffmpeg -y -i .\\tempdir\\opus" + przewidywanybitrateaudioKb + "k.opus -i .\\tempdir\\opus" + przewidywanybitrateaudioKb + "k.opus " + liniakomendmeta.Text + " -c copy " + mapowanieaudio2k + '"' + miejsceZapisu + '"'
                        };
                        soundprocess2.StartInfo = soundstartInfo2;
                        soundprocess2.Start();
                        soundprocess2.WaitForExit();
                    }
                    EncZakonczone();
                }
                else
                {
                    if (precisiontrack.Value == 0)
                    {
                        przewidywanybitrateaudioKb -= 3;
                    }
                    if (precisiontrack.Value == 1)
                    {
                        przewidywanybitrateaudioKb -= 2;
                    }
                    if (precisiontrack.Value == 2)
                    {
                        przewidywanybitrateaudioKb -= 1;
                    }

                if (przewidywanybitrateaudioKb < 6)
                {
                    var result = MessageBox.Show("Nagranie nie mieści się w oczekiwanej wielkości. Czy chcesz powtórzyć proces z automatycznie zmienionymi ustawieniami?", "Błąd renderu", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (result == DialogResult.Yes)
                    {
                        zapasdzwiektext.Text = "160kb/s";
                        audiozapassuwak.Value = 160;
                        GenerowanieCMDVideo();
                        GenerowanieCMDAudio();
                        RenderowanieNagrania();
                        return;
                    }
                    else
                    {
                        encprogressbar.Value = 0;
                        procentstatus.Text = "Błąd";
                        status.Text = "Renderowanie przerwane";
                        tabControl1.SelectTab("tabPage4");
                        AktywujForms();
                        return;
                    }
                }
                TworzenieAudio();
            }
        }

        public void EncZakonczone()
        {
            renderTab.Text = "🔄 Renderowanie";
            var notification = new NotifyIcon()
            {
                Visible = true,
                Icon = SystemIcons.Information,
                BalloonTipText = "Clenc zakończył proces enkodowania nagrania " + '"' + miejsceZapisu + '"',
                BalloonTipTitle = "Enkodowanie Zakończone"
            };
            if (wyświetlPowiadomieniePoZakończeniuProcesuToolStripMenuItem.Checked == true)
            {
                notification.ShowBalloonTip(7000);
            }
            AktywujForms();
                encprogressbar.Style = ProgressBarStyle.Blocks;
                renderTab.BackgroundImage = null;
                encprogressbar.Value = 100;
                enctimer1.Enabled = true;
                soundbitrateinf.Enabled = true;
                filesizekb.Enabled = true;
                progresscircle.Image = Resources.check;
                status.Text = "Zakończono Enkodowanie w " + encodertimesec + "s";
                playencoded.Visible = true;
                if (nieCzyśćFolderuTymczasowegoPoZakończeniuToolStripMenuItem.Checked == false)
                {
                    Process processX2 = new Process();
                    ProcessStartInfo startInfoX2 = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/C del /S /Q .\\tempdir & del /Q CLENCguilog-0.log"
                    };
                    processX2.StartInfo = startInfoX2;
                    processX2.Start();
                }
                splitContainer1.Visible = true;
        }

        private void radiosound1_CheckedChanged(object sender, EventArgs e)
        {
            mapowanieaudio = "-map 0:a:0";
            audiofadecheck.Enabled = true;
            GenerowanieCMDAudio();
        }

        private void radiosound2_CheckedChanged(object sender, EventArgs e)
        {
            mapowanieaudio = "-map 0:a:1";
            audiofadecheck.Enabled = true;
            GenerowanieCMDAudio();
        }

        private void radiosoundB_CheckedChanged(object sender, EventArgs e)
        {
            if (radiosoundB.Checked == true)
            {
                mapowanieaudio = "-map 0:a:0 -map 0:a:1";
                mapowanieaudio2k = "-map 0:v:0 -map 1:a:0 -map 1:a:1 ";
                audiofadecheck.Enabled = true;
                audiozapassuwak.Value = 0;
            }
            else
            {
                mapowanieaudio2k = "";
                audiofadecheck.Enabled = true;
                audiozapassuwak.Value = 2;
            }
            GenerowanieCMDAudio();
        }

        private void radiosoundJ_CheckedChanged(object sender, EventArgs e)
        {
            mapowanieaudio = "-filter_complex [0:a:0][0:a:1]amix[aout] -map [aout]";
            audiofadecheck.Checked = false;
            audiofade = "";
            audiofadecheck.Enabled = false;
            GenerowanieCMDAudio();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ObliczanieBitrateDiscord();
            liniakomendaudio.Text = liniakomendaudio.Text;
        }

        private void bitratelabel_TextChanged(object sender, EventArgs e)
        {
            GenerowanieCMDVideo();
            GenerowanieCMDAudio();
        }

        private void timerdatadownload_Tick(object sender, EventArgs e)
        {
            addmin = "";
            wmp1.Ctlcontrols.pause();
            double TimeDlugoscFull = wmp1.currentMedia.duration;
            sekundykoniec = (int)TimeDlugoscFull;
            int koniecsec = (int)TimeDlugoscFull % 60 + 1;
            int koniecmin = (int)TimeDlugoscFull / 60;
            if (koniecmin >= 180)
            {
                koniecmin -= 180;
            }

            if (koniecmin >= 120)
            {
                koniecmin -= 120;
            }

            if (koniecmin >= 60)
            {
                koniecmin -= 60;
            }

            if (koniecmin < 10)
            {
                addmin = "0";
            }

            int koniectim = (int)TimeDlugoscFull / 3600;
            TimeKoniec = "0" + koniectim + ":" + addmin + koniecmin + ":" + koniecsec;
            koniectextmask.Text = TimeKoniec;
            poczatek = TimeStart;
            double startseconds = TimeSpan.Parse(poczatek).TotalSeconds;
            int sekundypoczatek = (int)startseconds;
            sekundywideo = (sekundykoniec - sekundypoczatek) + 1;
            textsekundy = sekundywideo.ToString();
            dlugosctext.Text = textsekundy + "s";
            bitratelabelinf.Text = "Przewidywany bitrate: " + wielkosciplikubox.Text + "MB / " + sekundywideo + "s = " + bitratewideo + "kb/s";
            if (koniectextmask.Text != "00:00:01" && koniectextmask.Text != "00:00:1")
            {
                wmp1.Visible = true;
                timerdatadownload.Enabled = false;
            }

            zbytdlugiczas += 1;
            if (zbytdlugiczas >= 40)
            {
                pobinfo.Text = "Wczytywanie trwa dłużej niż zwykle...";
            }
            if (zbytdlugiczas >= 80)
            {
                pobinfo.Text = "Prawdopodobnie kodek nagrania nie jest obsługiwany.\nAby rozwiązać problem spróbuj: Doinstalować kodeki do Windows Media Player\n lub otwórz nagranie klikając PPM na przycisk [Wybierz nagranie] na dole pierwszej strony.";
            }
        }

        private void defaultplayer_Click(object sender, EventArgs e)
        {
            Process process10 = new Process();
            ProcessStartInfo startInfo10 = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C " + mpvdirexist + '"' + plikzrodlowy + '"'
            };
            process10.StartInfo = startInfo10;
            process10.Start();
        }

        private void lpmodecheck_CheckedChanged(object sender, EventArgs e)
        {
            if (lpmodecheck.Checked == false)
            {
                priority = "";
            }
            else
            {
                priority = "start /B /low ";
            }
        }

        private void obliczanieB()
        {
            if (BigInteger.TryParse(wielkosciplikubox.Text, out _))
            {
                maskerror2.Visible = false;
                BigInteger Oczekiwanawielkoscplikubox = BigInteger.Parse(wielkosciplikubox.Text);
                oczekiwanawielkoscpliku = Oczekiwanawielkoscplikubox * 8192;
                bajty = oczekiwanawielkoscpliku * 128 - 3;
                ObliczanieBitrateDiscord();
            }
            else
            {
                maskerror2.Visible = true;
                return;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            obliczanieB();
            WyznaczanieCzasu();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (fpscheck.Checked == true)
            {
                panelfps.Visible = true;
            }
            else
            {
                panelfps.Visible = false;
                fps60.Checked = false;
                fps50.Checked = false;
                fps30.Checked = false;
                fps29.Checked = false;
                fpsPAL.Checked = false;
                fpschange = "";
                GenerowanieCMDVideo();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
                FsErrCount++;
                if (FsErrCount >= 3)
                {
                filesizekb.Text = "N/A";
                enctimer1.Enabled = false;
                return;
                }

                FileInfo ExitFileSize = new FileInfo(miejsceZapisu);
                int wielkoscplikukb = (int)ExitFileSize.Length;
                int exitfs = wielkoscplikukb / 1024;
                filesizekb.Text = exitfs + "KB";
                enctimer1.Enabled = false;
        }

        private void playencoded_Click(object sender, EventArgs e)
        {
            Process process10 = new Process();
            ProcessStartInfo startInfo10 = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C " + mpvdirexist + '"' + miejsceZapisu + '"'
            };
            process10.StartInfo = startInfo10;
            process10.Start();
        }

        private void fps30_CheckedChanged(object sender, EventArgs e)
        {
            if (fps30.Checked == true)
            {
                fpschange = " -r 30";
            }
            GenerowanieCMDVideo();
        }

        private void usersettings_CheckedChanged(object sender, EventArgs e)
        {
            if (usersettings.Checked == true)
            {
                usercommandchange.Visible = true;
            }
            else
            {
                usercommandchange.Visible = false;
                usercommandchange.Text = "";
            }
        }

        private void audiofadecheck_CheckedChanged(object sender, EventArgs e)
        {
            if (audiofadecheck.Checked == false)
            {
                audiofade = "";
            }
            else
            {
                ObliczanieAF();
            }
            GenerowanieCMDAudio();
        }

        private void dropmetadata_CheckedChanged(object sender, EventArgs e)
        {
            if (dropmetadata.Checked == true)
            {
                liniakomendmeta.Text = "-map_metadata -1";
            }
            else
            {
                liniakomendmeta.Text = "-metadata copyright=" + '"' + "Sho#9398" + '"';
            }
        }

        private void wielkosciplikubox_TextChanged(object sender, EventArgs e)
        {
            if (wielkosciplikubox.Text != "")
            {
                if (wielkosciplikubox.Text != "0")
                {
                    obliczanieB();
                    WyznaczanieCzasu();
                    ObliczanieAF();
                }
                else
                {
                    oczekiwanienadanewielkoscMB.Enabled = true;
                }
            }
        }

        private void oczekiwanienadanewielkoscMB_Tick(object sender, EventArgs e)
        {
            wielkosciplikubox.Text = "1";
            obliczanieB();
            WyznaczanieCzasu();
            oczekiwanienadanewielkoscMB.Enabled = false;
        }

        private void koniectext_TextChanged(object sender, EventArgs e)
        {
            WyznaczanieCzasu();
            ObliczanieBitrateDiscord();
        }

        private void processexited1_Tick(object sender, EventArgs e)
        {
            if (process.HasExited == true)
            {
                if (File.Exists(".\\tempdir\\backgroundimg" + bitmapn + ".jpg"))
                {
                    renderTab.BackgroundImage = new Bitmap(".\\tempdir\\backgroundimg" + bitmapn + ".jpg");
                }
                encprogressbar.Style = ProgressBarStyle.Blocks;
                renderTab.Text = "🔄 Renderowanie [Etap 2/3]";
                procentstatus.Visible = true;
                if (niePokazujOknaWierszaPoleceńToolStripMenuItem.Checked == true)
                {
                    startInfo2.WindowStyle = ProcessWindowStyle.Hidden;
                }
                else
                {
                    startInfo2.WindowStyle = ProcessWindowStyle.Minimized;
                }
                startInfo2.FileName = "cmd.exe";
                startInfo2.Arguments = "/C " + priority + liniakomend2.Text + pauza;
                process2.StartInfo = startInfo2;
                process2.Start();
                processexited2.Enabled = true;
                processexited1.Enabled = false;
            }
            else
            {
                encodertimesec++;
                CurrentFileSize = new FileInfo(".\\tempdir\\videop2.webm");
                status.Text = "[Etap 1/3] Enkodowanie nagrania Pass 1 - ⏱" + encodertimesec + "s";
                featureslogo.Text = "Renderowanie: 0%";
            }
        }

        private void processexited2_Tick(object sender, EventArgs e)
        {
                if (process2.HasExited == true)
                {
                    processexited2.Enabled = false;
                    encprogressbar.Value = 100;
                renderTab.Text = "🔄 Renderowanie [Etap 3/3]";
                procentstatus.Text = "100%";
                    if (sounddisabled.Checked == false)
                    {
                        GenerowanieDzwieku();
                    }
                    else
                    {
                        if (!File.Exists(".\\tempdir\\videop2.webm"))
                        {
                            MessageBox.Show("Wygląda na to, że wystąpił błąd z enkodowaniem. Powtórz proces wraz z włączoną opcją " + '"' + "Pauza na końcu enkodowania(diagnost.)" + '"' + " z menu kontekstowego przycisku " + '"' + "Rozpocznij Enkdoowanie" + '"', "Błąd enkodowania", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            zabijprocess();
                            return;
                        }
                        Process processAN = new Process();
                    ProcessStartInfo startInfoAN = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/C ffmpeg -y -i .\\tempdir\\videop2.webm " + liniakomendmeta.Text + " -c copy " + '"' + miejsceZapisu + '"'
                };
                        processAN.StartInfo = startInfoAN;
                        processAN.Start();
                        processAN.WaitForExit();
                        soundbitrateinf.Text = "Dźwięk wyłączony";
                        EncZakonczone();
                    }
                }
                else
                {
                encodertimesec++;
                CurrentFileSize = new FileInfo(".\\tempdir\\videop2.webm");
                status.Text = "[Etap 2/3] Enkodowanie nagrania Pass 2 - ⏱" + encodertimesec.ToString() + "s";
                wielkoscplikucurrent = 10 + 100 * CurrentFileSize.Length / bajty;
                    if (wielkoscplikucurrent < 0)
                    {
                        return;
                    }

                    if (wielkoscplikucurrent < 100)
                    {
                        encprogressbar.Value = (int)wielkoscplikucurrent;
                        procentstatus.Text = wielkoscplikucurrent + "%";
                        featureslogo.Text = "Renderowanie: " + wielkoscplikucurrent + "%";
                }
            }
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            if (plikzrodlowy == "Brak")
            {
                PokazOknoOtwierania();
            }
        }

        private void PokazOknoOtwierania()
        {
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Nagrania (*.mp4; *.mkv; *.webm, *.mov, *.avi)|*.mp4;*mkv;*.webm;*.mov|Muzyka (*.mp3; *.flac, *.wav, *.m4a, *.aac, *.ogg)|*.ogg;*.mp3;*.flac;*.wav;*.m4a;*.aac|Wszystkie pliki (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.Title = "Wybierz nagranie do enkodowania";
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        string filePath = openFileDialog.FileName;
                        fileName = openFileDialog.SafeFileName;
                        adresnagrania.Text = filePath;
                        fileName = fileName.Replace('.', '-');
                        tabControl1.SelectTab("tabPage2");
                        if (nieOtwarzajNagraniaWAplikacjiToolStripMenuItem.Checked == false)
                        {
                            wmp1.Visible = false;
                            wmp1.URL = filePath;
                            timerdatadownload.Enabled = true;
                        }
                        plikzrodlowy = filePath;
                        ustawpoczatek.Enabled = true;
                        wyznaczkoniec.Enabled = true;
                        obliczanieB();
                        poczatektextmask.Enabled = true;
                        koniectextmask.Enabled = true;
                    }
                }
            }
            if (Directory.Exists("mpvdir"))
            {
                mpvdirexist = "cd mpvdir & mpv ";
                defaultplayer.Text = "Odtwórz w MPV ↗";
            }
        }

        private void znajdznagranie_Click_1(object sender, EventArgs e)
        {
            PokazOknoOtwierania();
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            if (miejsceZapisu == "Brak")
            {
                PokazOknoZapisu();
            }
        }

        private void tabPage4_Enter(object sender, EventArgs e)
        {
                if (maskerror.Visible == false && maskerror2.Visible == false && miejsceZapisu != "Brak" && plikzrodlowy != "Brak" && killprocess.Visible == false)
                {
                        startenc.Enabled = true;
                        status.Text = "Gotowy do enkodowania";
                    }
                    else
                    {
                        startenc.Enabled = false;
                        status.Text = "Przed rozpoczęciem wykonaj podstawowe kroki";
                }

            featureslogo.Text = "";
            if (lpmodecheck.Checked == true)
            {
                featureslogo.Text += "🎮";
            }
            if (fps29.Checked == true || fps30.Checked == true)
            {
                featureslogo.Text += "🎞";
            }
            if (changedimcheck.Checked == true)
            {
                featureslogo.Text += "↔️";
            }
            if (optimizebits.Checked == true)
            {
                featureslogo.Text += "⏲️";
            }

            if (niePokazujOknaWierszaPoleceńToolStripMenuItem.Checked)
            {
                pauzaNaKońcuEnkodowaniadebugToolStripMenuItem.Enabled = false;
            }
        }

        public void button2_Click_2(object sender, EventArgs e)
        {
            if (process.HasExited == true)
            {
                process2.Kill();
            }
            else
            {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("ffmpeg"))
            {
                process.Kill();
            }
            zabijprocess();
        }

        public void zabijprocess()
        {
            processexited1.Enabled = false;
            processexited2.Enabled = false;
            status.Text = "Przerwano proces";
            killprocess.Visible = false;
            tabControl1.Enabled = true;
            Process processXR = new Process();
            ProcessStartInfo startInfoXR = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C del /S /Q .\\clenc & del /Q CLENCguilog-0.log"
            };
            processXR.StartInfo = startInfoXR;
            processXR.Start();
            AktywujForms();
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (sounddisabled.Checked == true)
            {
                AudioOnlyCheck.Enabled = false;
                labelkomendasound.Enabled = false;
                liniakomendaudio.Enabled = false;
                audiofadecheck.Checked = false;
                audiofadecheck.Enabled = false;
                liniakomendaudio.Text = "Dźwięk wyłączony";
                audiozapassuwak.Value = 20;
                PodwojnyPanel.Panel1Collapsed = true;
            }
            else
            {
                AudioOnlyCheck.Enabled = true;
                labelkomendasound.Enabled = true;
                liniakomendaudio.Enabled = true;
                PodwojnyPanel.Panel1Collapsed = false;
            }
        }

        private void av1check_CheckedChanged(object sender, EventArgs e)
        {
            if (av1check.Checked == true)
            {
                kodekwideo = "libaom-av1";
                encspdtrack.Value = 2;
                ARNRcheck.Checked = false;
            }
            else
            {
                kodekwideo = "libvpx-vp9";
                encspdtrack.Value = 0;
            }
        }

        private void audiozapassuwak_ValueChanged(object sender, EventArgs e)
        {
            zapasdladzwieku = audiozapassuwak.Value;
            zapasdzwiektext.Text = audiozapassuwak.Value + "kb/s";

            ObliczanieBitrateDiscord();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (encspdtrack.Value >= 1)
            {
                audiozapassuwak.Value = 64;
                precisiontrack.Value = 1;
                ARNRcheck.Checked = false;
                optimizebits.Checked = false;
                optimizebits.Enabled = false;
            }
            else
            {
                audiozapassuwak.Value = 48;
                precisiontrack.Value = 2;
                ARNRcheck.Checked = true;
                optimizebits.Enabled = true;
            }

            if (encspdtrack.Value >= 3)
            {
                audiozapassuwak.Value = 96;
                precisiontrack.Value = 0;
            }

            if (encspdtrack.Value == 4)
            {
                altrefenabled = 0;
                laginframesnum = 1;
                tiles = 1;
            }

            if (encspdtrack.Value < 4)
            {
                altrefenabled = 1;
                laginframesnum = 25;
                tiles = 0;
            }
            GenerowanieCMDVideo();
            GenerowanieCMDAudio();
        }

        private void poczatektextmask_TextChanged(object sender, EventArgs e)
        {
                    WyznaczanieCzasu();
                    ObliczanieBitrateDiscord();
                    ObliczanieAF();
        }

        private void poczatektextmask_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            maskerror.Visible = true;
        }

        private void koniectextmask_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            maskerror.Visible = true;
        }

        private void koniectextmask_TextChanged(object sender, EventArgs e)
        {
                    WyznaczanieCzasu();
                    ObliczanieBitrateDiscord();
                    ObliczanieAF();
        }

        public void AktywujForms()
        {
            procentstatus.Visible = false;
            AudioOnlyCheck.Enabled = true;
            encspdtrack.Enabled = true;
            wielkosciplikubox.Enabled = true;
            radiosound1.Enabled = true;
            radiosound2.Enabled = true;
            radiosoundB.Enabled = true;
            radiosoundJ.Enabled = true;
            sounddisabled.Enabled = true;
            startenc.Enabled = true;
            killprocess.Visible = false;
        }

        public void WylaczForms()
        {
            playencoded.Visible = false;
            startenc.Enabled = false;
            AudioOnlyCheck.Enabled = false;
            encspdtrack.Enabled = false;
            wielkosciplikubox.Enabled = false;
            radiosound1.Enabled = false;
            radiosound2.Enabled = false;
            radiosoundB.Enabled = false;
            radiosoundJ.Enabled = false;
            sounddisabled.Enabled = false;
            startenc.Enabled = false;
        }

        private void pauzaNaKońcuEnkodowaniadebugToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (pauzaNaKońcuEnkodowaniadebugToolStripMenuItem.Checked == true)
            {
                pauza = " && pause";
                niePokazujOknaWierszaPoleceńToolStripMenuItem.Checked = false;
            }
            else
            {
                pauza = "";
            }
        }

        private void fullctrl_CheckedChanged_1(object sender, EventArgs e)
        {
            if (fullctrl.Checked == true)
            {
                liniakomend.ReadOnly = false;
                liniakomend2.ReadOnly = false;
                liniakomendaudio.ReadOnly = false;
                liniakomendmeta.ReadOnly = false;
            }
            else
            {
                liniakomend.ReadOnly = true;
                liniakomend2.ReadOnly = true;
                liniakomendaudio.ReadOnly = true;
                liniakomendmeta.ReadOnly = true;
            }
        }

        private void tabPage7_Click(object sender, EventArgs e)
        {
                Process processX = new Process();
                ProcessStartInfo startInfoX = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C del /S /Q .\\tempdir & cd . & mkdir .\\tempdir"
                };
                processX.StartInfo = startInfoX;
                processX.Start();
                processX.WaitForExit();
        }

        public void updatedimensions()
        {
            videodimensions = "-vf " + '"' + "crop=" + resolutionszerokoscbox.Text + ":" + resolutionwysokoscbox.Text + ":" + punktszerokoscibox.Text + ":" + punktwysokoscibox.Text + '"';
            GenerowanieCMDVideo();
        }

        public void renderimgfunction()
        {
            renderinfo.Text = "🕛";
            renderinfo.Visible = true;
            changedimcheck.Checked = true;
            updatedimensions();
            bitmapn++;
            Process processimg = new Process();
            string argumentycmd = "/C ffmpeg -y " + nvaccel + " -hide_banner -ss " + recduration.Text + " -i " + '"' + plikzrodlowy + '"' + " -map 0:v:0 -q:v 8 -frames:v 1 " + videodimensions + " .\\tempdir\\previewimg" + bitmapn + ".jpg";
            if (wyświetlIWstrzymajOknoWierszaPoleceńToolStripMenuItem.Checked == false)
            {
                ProcessStartInfo startInfoImg = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = argumentycmd
                };
                processimg.StartInfo = startInfoImg;
                processimg.Start();
                processimg.WaitForExit();
            }
            else
            {
                ProcessStartInfo startInfoImg = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Minimized,
                    FileName = "cmd.exe",
                    Arguments = argumentycmd + " & pause"
                };
                processimg.StartInfo = startInfoImg;
                processimg.Start();
                processimg.WaitForExit();
            }

            if (File.Exists(".\\tempdir\\previewimg" + bitmapn + ".jpg"))
            {
                dimensionsimg.Image = new Bitmap(".\\tempdir\\previewimg" + bitmapn + ".jpg");
            }
            else
            {
                renderinfo.Text = "Wystąpił błąd podczas renderowania. Spróbuj ponownie z innymi ustawieniami.";
                renderinfo.Visible = true;
                changedimcheck.Checked = false;
                return;
            }
            renderinfo.Visible = false;
        }

        private void button2_Click_3(object sender, EventArgs e)
        {
            renderimgfunction();
        }

        private void changedimcheck_CheckedChanged(object sender, EventArgs e)
        {
            if (changedimcheck.Checked == false)
            {
                videodimensions = "";
                usersettings.Enabled = true;
                GenerowanieCMDVideo();
            }
            else
            {
                updatedimensions();
                usersettings.Enabled = false;
            }
        }

        private void resolutionszerokoscbox_TextChanged(object sender, EventArgs e)
        {
            resolutioninfo.Text = resolutionwysokoscbox.Text + "p";
            if (autopreviewrendering.Checked == true)
            {
                renderimgfunction();
            }
        }

        private void AudioOnlyCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (AudioOnlyCheck.Checked == true)
            {
                panel3.Enabled = false;
                audiozapassuwak.Value = 0;
            }
            else
            {
                panel3.Enabled = true;
                audiozapassuwak.Value = 48;
            }
        }

        private void voipoptimization_CheckedChanged(object sender, EventArgs e)
        {
            if (voipoptimization.Checked == true)
            {
                voipoptimizationcmd = "-application voip ";
            }
            else
            {
                voipoptimizationcmd = "";
            }
            GenerowanieCMDAudio();
        }

        private void nvidiaaccelcheck_CheckedChanged(object sender, EventArgs e)
        {
            if (nvidiaaccelcheck.Checked == true)
            {
                nvaccel = "";
                hwaccelmodel.Visible = false;
            }
            else
            {
                hwaccelmodel.Visible = true;
                if (hwaccelmodel.Text == "NVDEC")
                {
                    nvaccel = "-hwaccel nvdec";
                }
                else
                {
                    nvaccel = "-hwaccel dxva2";
                }
            }
            GenerowanieCMDVideo();
        }

        private void fps29_CheckedChanged(object sender, EventArgs e)
        {
            if (fps29.Checked == true)
            {
                fpschange = " -r ntsc";
            }
            GenerowanieCMDVideo();
        }

        private void ARNRcheck_CheckedChanged(object sender, EventArgs e)
        {
            if (ARNRcheck.Checked == false)
            {
                arnrplus = "";
            }
            else
            {
                arnrplus = " -arnr_max_frames 15 -arnr_strength 6 -arnr_type 3";
            }
            GenerowanieCMDVideo();
        }

        private void killprocess_Click(object sender, EventArgs e)
        {
            processexited1.Enabled = false;
            processexited2.Enabled = false;
            foreach (var process in Process.GetProcessesByName("ffmpeg"))
            {
                process.Kill();
            }
            encprogressbar.Value = 0;
            procentstatus.Text = "Błąd";
            status.Text = "Renderowanie przerwane";
            renderTab.Text = "🔄 Renderowanie";
            tabControl1.SelectTab("tabPage4");
            AktywujForms();
        }

        private void niePokazujOknaWierszaPoleceńToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (niePokazujOknaWierszaPoleceńToolStripMenuItem.Checked)
            {
                pauzaNaKońcuEnkodowaniadebugToolStripMenuItem.Enabled = false;
            }
            else
            {
                pauzaNaKońcuEnkodowaniadebugToolStripMenuItem.Enabled = true;
            }
        }

        private void hwaccelmodel_Click(object sender, EventArgs e)
        {
            if (nvaccel == "-hwaccel nvdec")
            {
                nvaccel = "-hwaccel dxva2";
                hwaccelmodel.Text = "DXVA2";
            }
            else
            {
                nvaccel = "-hwaccel nvdec";
                hwaccelmodel.Text = "NVDEC";
            }
            GenerowanieCMDVideo();
        }

        private void fps60_CheckedChanged(object sender, EventArgs e)
        {
            if (fps60.Checked == true)
            {
                fpschange = " -r 60";
            }
            GenerowanieCMDVideo();
        }

        private void fps50_CheckedChanged(object sender, EventArgs e)
        {
            if (fps50.Checked == true)
            {
                fpschange = " -r 50";
            }
            GenerowanieCMDVideo();
        }

        private void fps48_CheckedChanged(object sender, EventArgs e)
        {
            if (fps48.Checked == true)
            {
                fpschange = " -r 48";
            }
            GenerowanieCMDVideo();
        }

        private void fpsPAL_CheckedChanged(object sender, EventArgs e)
        {
            if (fpsPAL.Checked == true)
            {
                fpschange = " -r pal";
            }
            GenerowanieCMDVideo();
        }

        private void removemeta_CheckedChanged(object sender, EventArgs e)
        {
            if (removemeta.Checked)
            {
                removemetacmd = " -map_metadata -1";
            }
            else
            {
                removemetacmd = "";
            }
            GenerowanieCMDVideo();
            GenerowanieCMDAudio();
        }

        private void advancedstarttime_TextChanged(object sender, EventArgs e)
        {
            GenerowanieCMDVideo();
        }

        private void advancedendtime_TextChanged(object sender, EventArgs e)
        {
            GenerowanieCMDVideo();
        }
    }
}