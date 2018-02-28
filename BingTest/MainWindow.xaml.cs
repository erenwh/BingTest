using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Configuration;
using Microsoft.CognitiveServices.SpeechRecognition;
using System.IO;


namespace BingTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		
		AutoResetEvent _FinalResponceEvent;                                         //Global var for handle thread
		MicrophoneRecognitionClient _microphoneRecognitionClient;					//microphone client
		public MainWindow()
		{
			InitializeComponent();
			SpeakBtn.Content = "Start Recording";
			StopBtn.Content = "Stop";
			StopBtn.IsEnabled = false;
			_FinalResponceEvent = new AutoResetEvent(false);
			Responsetxt.Background = Brushes.White;
			Responsetxt.Foreground = Brushes.Black;
		}
		private void ConvertSpeechToText()
		{
			var recognitionMode = SpeechRecognitionMode.LongDictation;
			string language = "en-us";
			string subscriptionKey = ConfigurationManager.AppSettings["MicrosoftSpeechApiKey"].ToString();

			_microphoneRecognitionClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
				recognitionMode,
				language,
				subscriptionKey
				);
			_microphoneRecognitionClient.OnPartialResponseReceived += ResponseReceived;
			_microphoneRecognitionClient.StartMicAndRecognition();
		}

		private void ResponseReceived(object sender, PartialSpeechResponseEventArgs e)
		{
			string result = e.PartialResult;
			string path = @"C:\Users\erenw\Desktop\test.txt";
			Dispatcher.Invoke(() =>
			{
				Responsetxt.Text = (e.PartialResult);
				Responsetxt.Text += ("\n");
				
				//File.AppendAllLines(path, new[] { result });
				//File.WriteAllText(@"C:\Users\erenw\Desktop\test.txt", result);
			});
			File.AppendAllLines(path, new[] { result });


		}

		private void SpeakBtn_Click(object sender, RoutedEventArgs e)
		{
			SpeakBtn.Content = "Listen...";
			SpeakBtn.IsEnabled = false;
			StopBtn.IsEnabled = true;
			Responsetxt.Background = Brushes.Green;
			Responsetxt.Foreground = Brushes.White;
			ConvertSpeechToText();
		}

		private void StopBtn_Click(object sender, RoutedEventArgs e)
		{
			StopBtn.IsEnabled = false;
			Dispatcher.Invoke((Action)(() =>
			{
				_FinalResponceEvent.Set();
				_microphoneRecognitionClient.EndMicAndRecognition();
				_microphoneRecognitionClient.Dispose();
				_microphoneRecognitionClient = null;
				SpeakBtn.Content = "Start Recording";
				SpeakBtn.IsEnabled = true;
				Responsetxt.Background = Brushes.White;
				Responsetxt.Foreground = Brushes.Black;
			}));
		}
	}
}
