using Demo1.Model;
using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecurePost : ContentPage
    {
        public ObservableCollection<Post> Posts { get; set; }


        Random _rand = new Random();
        FaceServiceClient faceServiceClient = new FaceServiceClient("948312e2e5954e7c829948f8e09670ee", "https://centralindia.api.cognitive.microsoft.com/face/v1.0");
        string FolderPath = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
        string PhotoPath;

        public SecurePost()
        {
            Posts = new ObservableCollection<Post>()
            {
                new Post {Date = DateTime.Now.AddDays(_rand.Next(-3,-1)).ToShortDateString(), Time = DateTime.Now.AddHours(_rand.Next(-3,-1)).ToShortTimeString(),Content="This is the first post. You have to do scannig for every post you make."},
                new Post {Date =DateTime.Now.AddDays(_rand.Next(-6,-1)).ToShortDateString(),Time = DateTime.Now.AddHours(_rand.Next(-6,-1)).ToShortTimeString(),Content="This is the second post. These posts are for demonstration purpose only."},
                new Post {Date = DateTime.Now.AddDays(_rand.Next(-12,-1)).ToShortDateString(), Time =DateTime.Now.AddHours(_rand.Next(-12,-1)).ToShortTimeString(),Content="This is the third post. After every post, last post gets deleted and will be replaced with your new post to demonstrate this feature."},
                new Post {Date = DateTime.Now.ToShortDateString(), Time = DateTime.Now.ToShortTimeString(),Content="This is the fourth post. This post will be replaced with your new post if verification is successfull."},
            };
            InitializeComponent();
            BindingContext = this;



            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                Grid _grid = new Grid();
                _grid.Padding = 12;
                _grid.RowSpacing = 0;
                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });


                Label labelDate = new Label();
                labelDate.TextColor = Color.Black;
                labelDate.HorizontalOptions = LayoutOptions.StartAndExpand;
                labelDate.SetBinding(Label.TextProperty, "Date");
                Grid.SetRow(labelDate, 0);
                Grid.SetColumn(labelDate, 0);
                _grid.Children.Add(labelDate);

                Label labelTime = new Label();
                labelTime.TextColor = Color.Black;
                labelTime.HorizontalOptions = LayoutOptions.EndAndExpand;
                labelTime.SetBinding(Label.TextProperty, "Time");
                Grid.SetRow(labelTime, 0);
                Grid.SetColumn(labelTime, 1);
                _grid.Children.Add(labelTime);

                Label labelContent = new Label();
                labelContent.TextColor = Color.Black;
                labelContent.HorizontalOptions = LayoutOptions.Start;
                labelContent.SetBinding(Label.TextProperty, "Content");
                Grid.SetRow(labelContent, 1);
                Grid.SetColumnSpan(labelContent, 2);
                _grid.Children.Add(labelContent);
                _grid.BackgroundColor = Color.Gray;
                return _grid;

            });
            clvPosts.ItemTemplate = dataTemplate;
            //      clvPosts.ItemsSource = Posts;
        }
        async Task TakePhotoAsync()
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                btnNewPost.IsEnabled = false;
            });
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo);
                Console.WriteLine($"CapturePhotoAsync COMPLETED:{PhotoPath} ");
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                PhotoPath = null;
                btnNewPost.IsEnabled = true;
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;
            await RecognitionFace("home", PhotoPath);
            //    await MakeAnalysisRequest(PhotoPath);
            Device.BeginInvokeOnMainThread(() =>
            {
                btnNewPost.IsEnabled = true;

            });
        }

        public async Task RecognitionFace(String PersonGroupId, string imgPath)
        {
            //   _img.Source = imgPath;
            using (Stream s = File.OpenRead(imgPath))
            {
                try
                {
                    var faces = await faceServiceClient.DetectAsync(s);
                    var faceIds = faces.Select(face => face.FaceId).ToArray();
                    var results = await faceServiceClient.IdentifyAsync(PersonGroupId, faceIds);
                    foreach (var identifyResults in results)
                    {
                        Console.Write($"Result of face: {identifyResults.FaceId}\n");
                        if (identifyResults.Candidates.Length == 0)
                        {

                            Console.WriteLine("No one identified");
                            await DisplayAlert("Msg", "Verificarion failed", "Ok");
                        }
                        else
                        {
                            //Get top 1 among all candidates returned
                            var candidateId = identifyResults.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(PersonGroupId, candidateId);
                            Console.WriteLine($"Identified as: {person.Name}");
                            lblMsg.Text = $"Identified as: {person.Name}";
                            if(person.Name == "hrishikesh")
                            {
                                Posts[Posts.Count - 1] = new Post { Content = editorPost.Text, Time = DateTime.Now.ToShortTimeString(), Date = DateTime.Now.ToShortDateString() };
                                await DisplayAlert(title: "Verificarion done", message: "New Post added successfully", cancel: "Ok");
                            }
                            else
                            {
                                await DisplayAlert("Verificarion Failed", "Try again", "Ok");
                            }
                            //           clvPosts.ItemsSource = null;
                            //            clvPosts.ItemsSource = Posts;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert($"Verificarion Failed-{ex.Message}", "Try again", "Ok");
                }
            }
        }


        private async void btnNewPost_Clicked(object sender, EventArgs e)
        {
            await TakePhotoAsync();
            editorPost.Text = "";
        }


    }
}