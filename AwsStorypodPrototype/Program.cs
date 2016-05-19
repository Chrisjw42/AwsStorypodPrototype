using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3;
using Amazon.S3.Model;

namespace AwsConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
            Tester testes = new Tester();
            string input = "";

            do
            {
                Console.WriteLine("C - Create a new bucket\nU - Upload a file\nL - List buckets\nD - DynamoDB\nX - Exit");
                input = Console.ReadLine();
                if (input.ToUpper() == "C")
                {
                    string inputInner = "";
                    do
                    {
                        Console.WriteLine("Input name");
                        inputInner = Console.ReadLine();
                    } while (inputInner == "");
                    testes.CreateBucket(inputInner);
                }
                else if (input.ToUpper() == "L")
                {
                    testes.ListBuckets();
                }
                else if (input.ToUpper() == "U")
                {
                    FileInfo file = new FileInfo(@"C:\Users\Chris\Desktop\poppy.jpg");
                    testes.UploadFile(file, testes.SelectBucket());
                }
                else if (input.ToUpper() == "D")///////////////////////////////////////////////////////////////////
                {
                    Console.WriteLine("---Welcome to DynamoDB test---");
                    DatabaseConnection dbConn = new DatabaseConnection();
                    do
                    {
                        Console.WriteLine("L - List all DB tables\nLT - Scan video table!\nC - Create a Table\nX - Exit DynamoDB\nT - Test");
                        input = Console.ReadLine();

                        if (input.ToUpper() == "L")
                        {
                            dbConn.ListTables();
                        }
                        else if (input.ToUpper() == "LT")
                        {
                            Console.WriteLine("Input search term or press enter to show all Videos");
                            string searchTerm = Console.ReadLine();
                            Console.WriteLine("...Please wait");
                            dbConn.ListTableObjects(dbConn.GetTable("Video"), searchTerm);
                        }
                        else if (input.ToUpper() == "C")
                        {
                            try
                            {
                                string tblName = "";
                                do
                                {
                                    Console.WriteLine("Preparing to create new table, please enter table name:");
                                    tblName = Console.ReadLine();
                                    tblName = tblName.ToLower();

                                    // More validation needed here obviously

                                } while (tblName == "");

                                dbConn.CreateTable(tblName);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.WriteLine("\nHere's the stacktrace:\n" + e.StackTrace);
                            }
                        }
                        else if (input.ToUpper() == "T")
                        {
                            DBObjectCreator dbObj = new DBObjectCreator(dbConn);
                            //Table tableOfChoice = database.GetTable()
                            List<Document> vids = dbObj.TestMethod();

                            Table tbl = dbConn.GetTable("Video");
                            foreach (Document d in vids)
                            {
                                dbConn.AddToTable(d, tbl);
                            }
                        }
                    } while (input.ToUpper() != "X");
                    input = "You'll never see this, man.";
                }
            } while (!(input.ToUpper() == "X" || input.ToUpper() == "N"));
        }
    }
}