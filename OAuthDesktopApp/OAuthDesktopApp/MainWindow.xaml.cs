// Copyright 2016 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using OAuthDesktopApp.GoogleApi;

namespace OAuthApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public class ViewModel : INotifyPropertyChanged
		{
			private string name;
			private Uri imageUrl;
			private ImageSource imageSource;
			private string outputText;

			public string Name { get { return this.name; } set { this.name = value; OnNotifyPropertyChanged(nameof(Name)); } }

			public Uri ImageUrl { get { return this.imageUrl; } set { this.imageUrl = value; OnNotifyPropertyChanged(nameof(ImageUrl)); } }

			public ImageSource ImageSource { get { return this.imageSource; } set { this.imageSource = value; OnNotifyPropertyChanged(nameof(ImageSource)); } }

			public string OutputText { get { return this.outputText; } set { this.outputText = value; OnNotifyPropertyChanged(nameof(OutputText)); } }

			public event PropertyChangedEventHandler PropertyChanged;

			protected void OnNotifyPropertyChanged(string propertyName)
			{
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private ViewModel viewModel;

		public MainWindow()
		{
			InitializeComponent();

			this.viewModel = new ViewModel();

			//this.label.Content = string.Empty;
			//this.label.DataContext = this;
			this.DataContext = this.viewModel;

			this.viewModel.Name = "user name goes here";
			this.textBoxOutput.DataContext = this.viewModel;
			this.label.DataContext = this.viewModel;

			//this.UserName = "user name goes here";
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				this.viewModel.Name = "getting...";
				var googleAuth = new GoogleOAuth2(this.Output);
				await googleAuth.PerformAuthenticationAsync(() => this.Activate());

				Output("*** Done auth. Making an API call now. ***");
				Output(string.Empty);

				var googleApis = new ApiCalls(this.Output);
				var responseJson = await googleApis.UserinfoCallAsync(googleAuth);

				this.UpdateContactInfo(responseJson);
			}
			catch (Exception ex)
			{
				Output(ex.Message);
			}
		}

		private void UpdateContactInfo(string responseJson)
		{
			/*
			{
				"sub": "111654053324868073923",
				"name": "Paul Vickery",
				"given_name": "Paul",
				"family_name": "Vickery",
				"picture": "https://lh3.googleusercontent.com/a-/AOh14Gg59HW4841ssWsiDbLPHGQIkhv6t9vt-xmBhb2cNg\u003ds96-c",
				"locale": "en-GB"
			} */

			JToken json = JToken.Parse(responseJson);

			var name = json.Value<string>("name");
			//var givenName = json.Value<string>("given_name");
			//var familyName = json.Value<string>("family_name");
			var pictureUrl = json.Value<string>("picture");

			//this.label.Content = name;
			this.viewModel.Name = name;

			// https://timheuer.com/blog/making-circular-images-in-xaml-easily/
			// https://stackoverflow.com/questions/23138878/create-circular-image-xaml
			var userImage = new BitmapImage();
			userImage.BeginInit();
			userImage.DecodePixelHeight = (int)this.contactImageFrame.Height;
			userImage.DecodePixelWidth = (int)this.contactImageFrame.Width;
			userImage.UriSource = new Uri(pictureUrl);
			userImage.EndInit();
			//this.contactImage.ImageSource = userImage;
			this.viewModel.ImageSource = userImage;
		}

		/// <summary>
		/// Appends the given string to the on-screen log, and the debug console.
		/// </summary>
		/// <param name="output">string to be appended</param>
		private void Output(string output)
		{
			//textBoxOutput.Text = textBoxOutput.Text + output + Environment.NewLine;
			this.viewModel.OutputText += output + Environment.NewLine;
			Console.WriteLine(output);
		}
	}
}
