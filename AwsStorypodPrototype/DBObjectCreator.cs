using System;
using System.Collections.Generic;
using System.IO;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.SecurityToken;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AwsConsoleApp1
{
    class DBObjectCreator
    {
        // Store a reference to the connection (which holds the currently connected client)
        private DatabaseConnection _DbConn;
        public DatabaseConnection DbConn
        {
            get
            {
                return _DbConn;
            }

            set
            {
                _DbConn = value;
            }
        }

        // ctor
        public DBObjectCreator(DatabaseConnection dbConn)
        {
            DbConn = dbConn;
            Console.WriteLine("DBObjCreator successfully created");
        }

        public List<Document> TestMethod()
        {
            List<Video> testList = ConvertJsonToVideos(@"../../ExampleData/videoData.json");
            List<Document> testDocList = GenerateListofDocuments(testList);
            return testDocList;
        }

        /// <summary>
        /// Reads JSON from file (will eventually be any source) and returns 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<Video> ConvertJsonToVideos(string fileName)
        {
            // We could potentially pass an Enum specifying the type of data being passed in
            StreamReader sr = null;
            JsonTextReader jsonRdr = null;
            string jsonInput;
            JsonSerializer jsSer;
            List<VideoInfo> listVideoInfosFromJson; // will need to generalise

            try
            {
                sr = new StreamReader(fileName);
                jsonRdr = new JsonTextReader(sr);
                jsonInput = sr.ReadToEnd();
                jsSer = new JsonSerializer();
                listVideoInfosFromJson = JsonConvert.DeserializeObject<List<VideoInfo>>(jsonInput);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
                return null; // return null, or throw exception?? 
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (jsonRdr != null)
                {
                    jsonRdr.Close();
                }
            }

            // Dummys for testing
            int i = 0;
            User chris = new User();
            chris.Name = "Chris Williams";
            User DTrump = new User();
            DTrump.Name = "Donald Trump";

            List<Video> videoList = new List<Video>();

            foreach (VideoInfo vInf in listVideoInfosFromJson)
            {
                i++;
                try
                {
                    Video tmp = new Video();
                    tmp.Title = vInf.Title;
                    tmp.DateTime = DateTime.Now;
                    //if (i % 2 == 0)
                    //{
                    //    tmp.User = chris;
                    //}
                    //else
                    //{
                    //    tmp.User = DTrump;
                    //}                    
                    tmp.User = DTrump;
                    videoList.Add(tmp);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}\n{1}", e.Message, e.StackTrace);
                }
            }
            return videoList;
        }

        /// <summary>
        /// Generate a List of DB Documents from a List of Objects, will need to be generic.
        /// </summary>
        /// <param name="listOfObjects"></param>
        /// <returns></returns>
        public List<Document> GenerateListofDocuments(List<Video> listOfObjects)
        {
            List<Document> docsToReturn = new List<Document>();

            // Attempt to create a document from each object
            foreach (Video v in listOfObjects)
            {
                try
                {
                    docsToReturn.Add(CreateDBDocumentFromVideo(v));
                }
                catch (AmazonDynamoDBException e)
                {
                    Console.WriteLine("{0}\n{1}", e.Message, e.StackTrace);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}\n{1}", e.Message, e.StackTrace);
                }
            }
            return docsToReturn;
        }

        /// <summary>
        /// Will probably eventually need to be generic
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public Document CreateDBDocumentFromVideo(Video vid)
        {
            string json = JsonConvert.SerializeObject(vid);

            Random rng = new Random();

            Document doc = null;
            try
            {

                Console.WriteLine(vid.Title);
                doc = Document.FromJson(json);

                doc["id"] = rng.Next();
                doc["User"] = vid.User.Name;
                doc["VideoName"] = vid.Title;
                doc["DateTime"] = DateTime.Now.ToString();
                //doc["VideOriginal"] = vid.VideOrginal.Location.ToString() ?? "www.unkownlocation.com";
                //doc["VideoTranscoded"] = vid.VideoTranscoded.Location.ToString() ?? "www.unkownlocation.com";
            }
            catch (Exception e)
            {
                throw new AmazonDynamoDBException(e.Message);
            }
            return doc;
        }

        /// <summary>
        /// Is this REALLY a good idea? It may just end up creating duplicate video objects. There's also the issue of fully reconstructing the video. Let's look in to the need for this at a later date.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public Video CreateVideoFromDBDocument(Document doc)
        {
            Video vid = new Video();
            

            return vid;
        }

    }
}
