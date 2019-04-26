using System;
using System.ComponentModel;
using Plugin.AudioRecorder;
using Xamarin.Forms;

namespace AudioDemo
{

    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        AudioRecorderService gravador;
        AudioPlayer reprodutor;

        public MainPage()
        {
            InitializeComponent();

            gravador = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(15),
                AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            };

            reprodutor = new AudioPlayer();
            reprodutor.FinishedPlaying += Finaliza_Reproducao;
        }

        async void Gravar_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!gravador.IsRecording) 
                {
                    gravador.StopRecordingOnSilence = TimeoutSwitch.IsToggled;

                    GravarButton.IsEnabled = false;
                    ReproduzirButton.IsEnabled = false;

                    //Começa gravação
                    var audioRecordTask = await gravador.StartRecording();

                    GravarButton.Text = "Parar Gravação";
                    GravarButton.IsEnabled = true;

                    await audioRecordTask;

                    GravarButton.Text = "Gravar";
                    ReproduzirButton.IsEnabled = true;
                }
                else 
                {
                    GravarButton.IsEnabled = false;

                    //parar a gravação...
                    await gravador.StopRecording();

                    GravarButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                //blow up the app!
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        void Reproduzir_Clicked(object sender, EventArgs e)
        {
            try
            {
                var filePath = gravador.GetAudioFilePath();

                if (filePath != null)
                {
                    ReproduzirButton.IsEnabled = false;
                    GravarButton.IsEnabled = false;

                    reprodutor.Play(filePath);
                }
            }
            catch (Exception ex)
            {
                //blow up the app!
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }


        void Finaliza_Reproducao(object sender, EventArgs e)
        {
            ReproduzirButton.IsEnabled = true;
            GravarButton.IsEnabled = true;
        }
    }
}