using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
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
                Console.WriteLine("C - Create a new bucket\nU - Upload a file\nX - Exit\nL - List buckets\nD - DynamoDB");
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
                else if (input.ToUpper() == "D")
                {
                    Console.WriteLine("---Welcome to DynamoDB test---");
                    DatabaseConnection database = new DatabaseConnection();
                    do
                    {
                        Console.WriteLine("L - List all DB tables\nC - Create a Table\nX - Exit DynamoDB");
                        input = Console.ReadLine();
                        
                        if(input.ToUpper() == "L")
                        {
                            database.ListCurrentTables();
                        }
                        else if(input.ToUpper() == "C")
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

                                database.CreateTable(tblName);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.WriteLine("\nHere's the stacktrace:\n" + e.StackTrace);
                            }
                        }                        
                    } while (input.ToUpper() != "X");
                    input = "You'll never see this, man.";
                }
            } while (!(input.ToUpper() == "X" || input.ToUpper() == "N"));
        }
    }
}