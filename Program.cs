using System;
using System.Net.Http;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CSHttpClientSample
{
    static class Program
    {

        static void Main()
        {
            //MakeRequest();
            //Console.WriteLine("Hit ENTER to exit...");
            //Console.ReadLine();

            var apiUrl = "https://api.videoindexer.ai";
            var accountId = "9c06efd6-d53d-49eb-82f2-f7e88e19447c";
            var location = "trial"; // replace with the account's location, or with “trial” if this is a trial account
            var apiKey = "53e99203f4e54d088d629582dfaa19c0";

            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

            // create the http client
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            // obtain account access token
            var accountAccessTokenRequestResult = client.GetAsync($"{apiUrl}/auth/{location}/Accounts/{accountId}/AccessToken?allowEdit=true").Result;
            var accountAccessToken = accountAccessTokenRequestResult.Content.ReadAsStringAsync().Result.Replace("\"", "");

            client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");

            // upload a video

            //CloudBlobContainer blobContainer = BlobClient
            //blobContainer.FetchAttributes();
            //string count = blobContainer.Metadata["ItemCount"];
            //int ItemCount;
          
                    // Container is not Empty
                    Console.WriteLine("Uploading...");
                    // get the video from URL
                    BlobServiceClient backupBlobClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=mediajoystorage;AccountKey=Ve5oaMBqE2E5oJsc9DlH7IvxdwPL4RcwtMBAiYO3oZzyewtniqIoOcran2cZUXBAMVQdzw1laadhuOxLYdJiLw==;EndpointSuffix=core.windows.net");
                    BlobContainerClient backupContainer = backupBlobClient.GetBlobContainerClient("mediafiles");
                     if(backupContainer.GetBlobs()!=null)
                    { 
                        foreach (BlobItem blobItem in backupContainer.GetBlobs())
                        {


                        Console.WriteLine("\t" + blobItem.Name);
                        var videoUrl = "https://mediajoystorage.blob.core.windows.net/mediafiles/" + blobItem.Name;
                        var supportedTypes = new[] { "3gp", "mp4", "mp3", "avi" };
                        var fileExt = System.IO.Path.GetExtension(blobItem.Name).Substring(1);
                        var content = new MultipartFormDataContent();
                        if (supportedTypes.Contains(fileExt))
                        {

                            // as an alternative to specifying video URL, you can upload a file.
                            // remove the videoUrl parameter from the query string below and add the following lines:
                            //FileStream video =File.OpenRead(Globals.VIDEOFILE_PATH);
                            //byte[] buffer = new byte[video.Length];
                            //video.Read(buffer, 0, buffer.Length);
                            //content.Add(new ByteArrayContent(buffer));

                            var uploadRequestResult = client.PostAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos?accessToken={accountAccessToken}&name={blobItem.Name}&description=blobItem.Properties&privacy=private&partition=some_partition&videoUrl={videoUrl}", content).Result;
                            var uploadResult = uploadRequestResult.Content.ReadAsStringAsync().Result;

                            // get the video id from the upload result
                            var videoId = JsonConvert.DeserializeObject<dynamic>(uploadResult)["id"];
                            Console.WriteLine("Uploaded");
                            Console.WriteLine("Video ID: " + videoId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Media Type");
                        }
                    }
                }
                else
                {
                
                Console.WriteLine("Blob is empty");
                }


            }
        }
    }

