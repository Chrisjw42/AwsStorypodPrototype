using System;
using Amazon.DynamoDBv2.DataModel;


namespace AwsConsoleApp1
{
    public class VideoFile : File
    {
        public Guid Id { get; set; }

        public S3Link FileOnS3
        {
            get;
            set;
        }

    }
}
