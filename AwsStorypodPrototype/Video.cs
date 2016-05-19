using System;
using Amazon.DynamoDBv2.DataModel;

namespace AwsConsoleApp1
{
    //[DynamoDBTable("Video")]
    class Video
    {
        private string _Title;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }

        public VideoFile VideoTranscoded
        {
            get;
            set;
        }

        public VideoFile VideOrginal
        {
            get;
            set;
        }

        public DateTime DateTime
        {
            get;
            set;
        }

        public User User
        {
            get;
            set;
        }

        public Video()
        {
        }
    }
}
