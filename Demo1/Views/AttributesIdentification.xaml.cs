using Microsoft.ProjectOxford.Face;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AttributesIdentification : ContentPage
    {
        FaceServiceClient faceServiceClient = new FaceServiceClient("948312e2e5954e7c829948f8e09670ee", "https://centralindia.api.cognitive.microsoft.com/face/v1.0");
        string FolderPath = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
        string PhotoPath;

        public AttributesIdentification()
        {
            InitializeComponent();
        }
        void clearLbls()
        {
            lblGender.Text = lblSmile.Text = lblAge.Text = lblAnger.Text = lblGlasses.Text = "";
        }

        async Task TakePhotoAsync()
        {
            clearLbls();
            Device.BeginInvokeOnMainThread(() =>
            {
                btnPickImg.IsEnabled = false;

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
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;
            await MakeAnalysisRequest(PhotoPath);
            Device.BeginInvokeOnMainThread(() =>
            {
                btnPickImg.IsEnabled = true;

            });
        }

        private async void btnPickImg_Clicked(object sender, EventArgs e)
        {
            await TakePhotoAsync();
        }

        public async Task MakeAnalysisRequest(string imageFilePath)
        {
            try{
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "948312e2e5954e7c829948f8e09670ee");

                string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                    "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                    "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

                string uri = "https://centralindia.api.cognitive.microsoft.com/face/v1.0/detect" + "?" + requestParameters;
                HttpResponseMessage response;
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);

                    string contentString = await response.Content.ReadAsStringAsync();

                    List<ResponseModel> faceDetails = JsonConvert.DeserializeObject<List<ResponseModel>>(contentString);
                    if (faceDetails.Count != 0)
                    {
                        lblGender.Text = "Gender : " + faceDetails[0].faceAttributes.gender;
                        lblAge.Text = "Age : " + faceDetails[0].faceAttributes.age;
                        lblSmile.Text = "Smile : " + faceDetails[0].faceAttributes.smile.ToString("0.0");
                        lblAnger.Text = "Anger : " + faceDetails[0].faceAttributes.emotion.anger;
                        lblGlasses.Text = "Glasses : " + faceDetails[0].faceAttributes.glasses;
                    }

                }
            }
            catch (Java.Net.UnknownHostException ex)
            {
                await DisplayAlert("Mgs", "No Internet connected", "Ok");
            }
        }
        public byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
