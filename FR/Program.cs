using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;
namespace FR
{
    public class Program
    {
        FaceServiceClient faceServiceClient = new FaceServiceClient("948312e2e5954e7c829948f8e09670ee", "https://centralindia.api.cognitive.microsoft.com/face/v1.0");
  
        public async void CreatPersonGroup(String PersonGroupID, string PersonGroupName)
        {
            try
            {
               await faceServiceClient.CreatePersonGroupAsync(PersonGroupID, PersonGroupName) ;
                
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in creating person group.\n" + ex.Message);
            }
        }

        
        
        public async void AddPersonToGroup(String PersonGroupID, string name, string pathImg)
        {
            try
            {
                await  faceServiceClient.GetPersonGroupAsync(PersonGroupID);
                CreatePersonResult person = await faceServiceClient.CreatePersonAsync(PersonGroupID, name);
                DetectAndRegisterFace(PersonGroupID,person,pathImg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in adding person to group.\n" + ex.Message);
            }
        }

        private async void DetectAndRegisterFace(string personGroupID, CreatePersonResult person, string pathImg)
        {
            foreach (var imgPath in Directory.GetFiles(pathImg, "*.jpeg"))
            {
                using(Stream s = File.OpenRead(imgPath))
                {
                   await faceServiceClient.AddPersonFaceAsync(personGroupID, person.PersonId, s) ;
                }
            }
        }

        public async void TrainingAI(string PersonGroupID)
        {
           await faceServiceClient.TrainPersonGroupAsync(PersonGroupID) ;
            TrainingStatus trainingStatus = null;
            while(true)
            {
                trainingStatus = await  faceServiceClient.GetPersonGroupTrainingStatusAsync(PersonGroupID);
                if(trainingStatus.Status != Status.Running)
                    break;
             // await  Task.Delay(1000);
            }
            Console.WriteLine("Training AI Completed");
        }

        public async void RecognitionFace(String PersonGroupId,string imgPath)
        {
           
            using(Stream s = File.OpenRead(imgPath))
            {
                var faces = await faceServiceClient.DetectAsync(s);
                var faceIds = faces.Select(face => face.FaceId).ToArray();
                try 
                {
                    var results = await faceServiceClient.IdentifyAsync(PersonGroupId, faceIds);
                    foreach(var identifyResults in results)
                    {
                        Console.Write($"Result of face: {identifyResults.FaceId}\n");
                        if(identifyResults.Candidates.Length ==0)
                            Console.WriteLine("No one identified");
                        else
                        {
                            //Get top 1 among all candidates returned
                            var candidateId = identifyResults.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(PersonGroupId, candidateId);
                            Console.WriteLine($"Identified as: {person.Name}");
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error in recognizing face: {ex.Message}");
                }
            }
        }
        static  void Main()
        {
            //This block is used for training the photo and verifying it after training
            //For training comment line of p.RecognitionFace
            //for verfying comment line no. 100,101,102
            Program p = new Program();
         //  p.CreatPersonGroup("studentn", "studentnone");
         //   p.AddPersonToGroup("studentn", "Vaishnavi", @"C:\Users\lenovo\OneDrive\Desktop\Vaishnavi");
         //   p.TrainingAI("studentn");
            p.RecognitionFace("studentn", @"C:\Users\lenovo\OneDrive\Desktop\Vaishnavi\vaishanvi.jpeg");
            Console.ReadLine();
        }
    }
}
