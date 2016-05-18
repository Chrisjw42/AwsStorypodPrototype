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
            ReadJson(@"../../ExampleData/videoData.json");
        }        

        /// <summary>
        /// Reads JSON from file (will eventually be any source) and returns an array of documents
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<Document> ReadJson(string fileName)
        {
            StreamReader sr = null;
            //StreamWriter sw = null;
            JsonTextReader jsonRdr = null;
            JsonTextWriter jsonWrt = null;
            string jsonInput;
            JsonSerializer jsSer;
            List<Video> videosFromDaJAson;

            try
            {
                sr = new StreamReader(fileName);
                //sw = new StreamWriter();
                jsonRdr = new JsonTextReader(sr);
                //jsonWrt = new JsonTextWriter(sw);
                jsonInput = sr.ReadToEnd();
                jsSer = new JsonSerializer();
                videosFromDaJAson = JsonConvert.DeserializeObject<List<Video>>(jsonInput);
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
            List<Document> docsToReturn = new List<Document>();

            Console.WriteLine("We've come this far");

            try
            {
                foreach (Video v in videosFromDaJAson)
                {

                    //string json = jsSer.Serialize(jsonWrt, v);

                    Document tmpDoc = null;
                    try
                    {
                        Console.WriteLine(v.Title);
                        //tmpDoc = Document.FromJson(json);                   
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }
                    
                    //docsToReturn.Add(tmpDoc);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("{0}\n{1}", e.Message, e.StackTrace);
            }
            return docsToReturn;
        }

    }
}
