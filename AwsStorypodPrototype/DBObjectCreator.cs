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

        public DBObjectCreator(DatabaseConnection dbConn)
        {
            DbConn = dbConn;
            Console.WriteLine("DBObjCreator successfully created");
            List<Video> testList = ConvertJsonToObjects(@"../../ExampleData/videoData.json");
            List<Document> testDocList = GenerateListofDocuments(testList);
        }        

        /// <summary>
        /// Reads JSON from file (will eventually be any source) and returns 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<Video> ConvertJsonToObjects(string fileName)
        {
            // We could potentially pass an Enum specifying the type of data being passed in
            StreamReader sr = null;
            JsonTextReader jsonRdr = null;
            string jsonInput;
            JsonSerializer jsSer;
            List<VideoInfo> videoInfoFromJson; // will need to generalise

            try
            {
                sr = new StreamReader(fileName);
                jsonRdr = new JsonTextReader(sr);
                jsonInput = sr.ReadToEnd();
                jsSer = new JsonSerializer();
                videoInfoFromJson = JsonConvert.DeserializeObject<List<VideoInfo>>(jsonInput);
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

            List<Video> videoList = new List<Video>();

            foreach (VideoInfo vInf in videoInfoFromJson)
            {
                try
                {
                    Video tmp = new Video();
                    tmp.Title = vInf.Title;
                    tmp.User = new User();
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
                    docsToReturn.Add(CreateDBDocument(v));
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
        /// Will eventually need to be generic
        /// </summary>
        /// <returns></returns>
        public Document CreateDBDocument(Video obj)
        {
            string json = JsonConvert.SerializeObject(obj);

            Document doc = null;
            try
            {
                Console.WriteLine(obj.Title);
                doc = Document.FromJson(json);
            }
            catch (Exception e)
            {
                throw new AmazonDynamoDBException(e.Message);
            }
            return doc;
        }

    }
}
