using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace AwsConsoleApp1
{
    class Tester
    {
        /// <summary>
        /// Print all buckets (and some information) to the console.
        /// </summary>
        public void ListBuckets()
        {
            using (IAmazonS3 s3Client = new AmazonS3Client())
            {
                ListBucketsResponse response = s3Client.ListBuckets();
                List<S3Bucket> bucketsYo = response.Buckets;

                int indexCounter = 0;
                foreach (S3Bucket buck in bucketsYo)
                {
                    Console.WriteLine("\nBucket Found! Index {0}", indexCounter);
                    Console.WriteLine("BucketName: {0}\nCreationDate: {1}", buck.BucketName, buck.CreationDate);
                    indexCounter++;
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Get a List of all buckets to use in operations
        /// </summary>
        /// <returns>List of S3Bucket</returns>
        public List<S3Bucket> GetAllBuckets()
        {
            using (IAmazonS3 s3Client = new AmazonS3Client())
            {
                ListBucketsResponse response = s3Client.ListBuckets();
                List<S3Bucket> buckets = response.Buckets;
                return buckets;                
            }
        }

        /// <summary>
        /// Create a bucket with the desired name
        /// </summary>
        /// <param name="name"></param>
        public void CreateBucket(string name)
        {
            using (IAmazonS3 s3Client = new AmazonS3Client())
            {
                try
                {
                    Console.WriteLine("Bucket name is being converted to lowercase due to Amazon restrictions.\n...");
                    // Create a request using passed name
                    PutBucketRequest rq = new PutBucketRequest();
                    rq.BucketName = name.ToLower();
                    // Try to create bucket
                    PutBucketResponse rsp = s3Client.PutBucket(rq);
                    Console.WriteLine("Bucket created successfully!");
                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine("Fuck it, no Bucket.");
                    Console.WriteLine(e.Message);
                }
            }
        }

        public S3Bucket SelectBucket()
        {
            Console.WriteLine("Preparing to upload a file, listing all buckets...\n...");
            ListBuckets();
            List<S3Bucket> listBuckets = GetAllBuckets();
            
            string input = "";
            int index;
            do
            {
                Console.WriteLine("Please input the index number of the bucket you'd like to upload to: ");
                input = Console.ReadLine();
                // While it isn't an int or it's out of range
            } while (!(int.TryParse(input, out index)) || (index < 0 || index > listBuckets.Count -1));
            Console.WriteLine("Got dat int");
            return listBuckets[index];
        }

        public void UploadFile(FileInfo file, S3Bucket targetBucket)
        {
            using (IAmazonS3 s3Client = new AmazonS3Client())
            {
                try
                {
                    FileStream fs = new FileStream(file.FullName, FileMode.Open);
                    
                    PutObjectRequest put = new PutObjectRequest();
                    put.Metadata.Add("hello", "joe");
                    put.BucketName = targetBucket.BucketName;
                    put.Key = file.Name;
                    put.InputStream = fs;
                   
                    PutObjectResponse rsp = s3Client.PutObject(put);
                    
                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("hey!");
                }
            }
        }
    }
}
