﻿using System;

namespace AwsConsoleApp1
{
    class VideoInfo
    {
        // Hash key, comparable to a SQL primary key
        public ulong Key { get; set; }

        // Potential Range key
        public string Title { get; set; }

        // DB Property
        public string CreationDate { get; set; }

        // DB Property
        public string Creator { get; set; }

        public override string ToString()
        {
            return Title + Creator;
        }
    }
}
