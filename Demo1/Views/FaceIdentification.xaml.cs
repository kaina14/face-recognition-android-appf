using Demo1.Model;
using Microsoft.ProjectOxford.Face;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Demo1
{
    public partial class FaceIdentification : ContentPage
    {
        FaceServiceClient faceServiceClient = new FaceServiceClient("948312e2e5954e7c829948f8e09670ee", "https://centralindia.api.cognitive.microsoft.com/face/v1.0");
        string FolderPath = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
        string FilePath;
        string PhotoPath;
        XmlWriterSettings sts;
        XmlWriter _xmlWriter;
        PreAbs preAbs = new PreAbs();
        public bool IsEnd = false;
        public FaceIdentification()
        {
            InitializeComponent();
            FilePath = Path.Combine(FolderPath, "attendance.xml");
            sts = new XmlWriterSettings()
            {
                Indent = true,
            };
        }

   
        async Task TakePhotoAsync(bool isEnd)
        {
            lblMsg.Text = "";
            _img.Source = null;
            IsEnd = false;        //No end attendance
            Device.BeginInvokeOnMainThread(() =>
            {
                btnTakePhoto.IsEnabled = false;
            });
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo,isEnd);
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

        async Task LoadPhotoAsync(FileResult photo,bool isEnd)
        {
            // canceled
            if (photo == null)
            {
                PhotoPath = null;
                Device.BeginInvokeOnMainThread(() => 
                {
                    btnTakePhoto.IsEnabled = true;
                });
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;
            await RecognitionFace("home", PhotoPath,isEnd);
           
             Device.BeginInvokeOnMainThread(() =>
            {
                btnTakePhoto.IsEnabled = true;

            });
        }

        public async Task RecognitionFace(String PersonGroupId, string imgPath,bool isEnd)
        {
            _img.Source = imgPath;
            using (Stream s = File.OpenRead(imgPath))
            {
                var faces = await faceServiceClient.DetectAsync(s);
                var faceIds = faces.Select(face => face.FaceId).ToArray();
                try
                {
                    var results = await faceServiceClient.IdentifyAsync(PersonGroupId, faceIds);
                    foreach (var identifyResults in results)
                    {
                        Console.Write($"Result of face: {identifyResults.FaceId}\n");
                        if (identifyResults.Candidates.Length == 0)
                            Console.WriteLine("No one identified");
                        else
                        {
                            //Get top 1 among all candidates returned
                            var candidateId = identifyResults.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(PersonGroupId, candidateId);
                            Console.WriteLine($"Identified as: {person.Name}");
                            lblMsg.Text = $"Identified as: {person.Name}";
                            
                            if(person.Name == "hrishikesh" && isEnd)      //Admin name here
                            {
                                IsEnd = true;
                                return;
                            }

                            WriteXmlFile(person.Name, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), true);
                           
                            switch(person.Name)  //setting attendance true
                            {
                                case "hrishikesh":              //Admin name
                                    preAbs["hrishikesh"] = true;  //Admin
                                    break;
                                case "ram":  //Admin
                                    preAbs["ram"] = true;  //Admin
                                    break;
                                case "omkar":  //Admin
                                    preAbs["omkar"] = true;  //Admin
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "";
                    _img.Source = null;
                    await DisplayAlert("Mgs", "No Faces Detecctd", "Ok");
                    Console.WriteLine($"Error in recognizing face: {ex.Message}");
                }
            }
        }

        private async void btnTakePhoto_Clicked(object sender, EventArgs e)
        {
            await TakePhotoAsync(false);
        }

        #region commented analyssis
        /*
        public async Task MakeAnalysisRequest(string imageFilePath)
        {
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
        } */
        #endregion

        #region Commented imgArrayGet
        /*
        public byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
         */
        #endregion

        private void WriteXmlFile(string Name, string Date, string InTime, bool Status)
        {
            if (File.Exists(FilePath) && File.ReadAllBytes(FilePath).Length > 0)
            {
                XmlSerializer _xmlSerializer = new XmlSerializer(typeof(List<Attendance>), new XmlRootAttribute("Attendances"));
                XmlReader _xmlReader = XmlReader.Create(FilePath);
                List<Attendance> attendancesList = (List<Attendance>)_xmlSerializer.Deserialize(_xmlReader);

                for (int i = 0; i < attendancesList.Count; i++)
                {
                    if (attendancesList[i].SName == Name && attendancesList[i].Date == Date)
                    {
                        if (attendancesList[i].Status == false)
                        {
                            _xmlReader.Close();
                            return;
                        }

                        if(!IsEnd)
                        {
                            DisplayAlert("Msg", "Attendance is already taken for today.", "Ok");
                        }
                        
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            btnTakePhoto.IsEnabled = true;
                        });
                        _xmlReader.Close();
                        return;
                    }
                }
                _xmlReader.Close();
            }
            else if(!File.Exists(FilePath))
            {
                  _xmlWriter = XmlWriter.Create(FilePath, sts);
            }

            if (File.ReadAllBytes(FilePath).Length == 0)
            {
                _xmlWriter?.Close();
                _xmlWriter = XmlWriter.Create(FilePath, sts);
                _xmlWriter.WriteStartElement("Attendances");
                _xmlWriter.WriteStartElement("Attendance");
                _xmlWriter.WriteElementString("SName", Name);
                _xmlWriter.WriteElementString("Date", Date);
                _xmlWriter.WriteElementString("InTime", InTime);
                _xmlWriter.WriteElementString("Status", Status ? "1" : "0");
                _xmlWriter.WriteEndElement();
                _xmlWriter.WriteEndElement();
                _xmlWriter.Flush();
                _xmlWriter.Close();
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(FilePath);
                XPathNavigator navigator = doc.CreateNavigator();
                navigator.MoveToChild("Attendances", "");
                _xmlWriter?.Close();
                _xmlWriter = navigator.AppendChild();
                _xmlWriter.WriteStartElement("Attendance");
                _xmlWriter.WriteElementString("SName", Name);
                _xmlWriter.WriteElementString("Date", Date);
                _xmlWriter.WriteElementString("InTime", InTime);
                _xmlWriter.WriteElementString("Status", Status ? "1" : "0");
                _xmlWriter.WriteEndElement();
                _xmlWriter.Flush();
                _xmlWriter.Close();
                doc.Save(FilePath);
            }
        }

        private async void btnEndAttendance_Clicked(object sender, EventArgs e)
        {
            
            bool isSubmit = await DisplayAlert("Confirm", "Are you sure of submitting attendance?", "Yes","Cancel");
            if(isSubmit)
            {
                await TakePhotoAsync(true);
                if(IsEnd)
                {
                    for (int i = 0; i < typeof(PreAbs).GetProperties().Length-2; i++)
                    {
                        if (((Tuple<bool, string>)preAbs[i]).Item1 == false)
                        {
                            WriteXmlFile( ((Tuple<bool, string>)preAbs[i]).Item2, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), false);
                        }
                    }
                }
                else 
                {
                    await DisplayAlert("Msg", "Authorization failed for submitting attendance", "ok");
                }

                lblMsg.Text = "";
                _img.Source = null;
            }
           
        }
    }
}
