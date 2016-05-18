using System;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace MediaToolkit_Test
{
    class MediaToolKitTest
    {
        static void Main(string[] args)
        {
            /* 
             *  For each variable you need to specify the FULL path to files.
             *  You don't necessarily need to have all these variables, they're only been used to show
             *  that the wrapper is working correctly.
             *  The supported format that can be used: https://github.com/AydinAdn/MediaToolkit/wiki/Supported-file-formats
             */

            // Initial input video
            string inputPath = @"c:\Path\To\File.mkv";
            // Converted video (eg. mkv --> mp4)
            string convertPath = @"c:\Path\To\File.mp4";
            // Trimmed down video
            string cutPath = @"c:\Path\To\File.mp4";
            // No audio video output
            string videooutputPath = @"c:\Path\To\File.mp4";
            // Audio output
            string audiooutputPath = @"c:\Path\To\File.m4a";
            // The merged no audio videa and audio files
            string mergeoutputPath = @"c:\Path\To\File.mp4";

            // Declarations of the MediaFile objects
            var inputFile = new MediaFile { Filename = inputPath };
            var convertFile = new MediaFile { Filename = convertPath };
            var cutFile = new MediaFile { Filename = cutPath };
            var videoOutputFile = new MediaFile { Filename = videooutputPath };
            var audioOutputFile = new MediaFile { Filename = audiooutputPath };
            var mergeOutputFile = new MediaFile { Filename = mergeoutputPath };

            // Use the Media ToolKit engine
            using (var engine = new Engine())
            {
                // Extract metadata from the video
                // Could be used to programmatically determine what to do with specific formats, bit rates etc. 
                // Example: var data = inputFile.Metadata.AudioData.BitRateKbs;
                engine.GetMetadata(inputFile);
                
                // Conversion options for cutting the video.
                // Currently no options set.
                var options = new ConversionOptions();

                Console.WriteLine("\n------------\nConverting...\n------------");
                // Convert file to from mkv to mp4.
                engine.Convert(inputFile, convertFile);

                Console.WriteLine("\n------------\nCutting...\n------------");
                // Cut video
                // The output video starts at 30 sec mark of the input video and ends 180 secs later.
                options.CutMedia(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(180));
                engine.Convert(convertFile, cutFile, options);

                Console.WriteLine("\n------------\nEncoding...\n------------");
                // Encode cut video to 480p and ratio of 16:9
                Encode(cutFile, convertFile);

                Console.WriteLine("\n------------\nSplitting Audio & Video...\n------------");
                // Custom ffmpeg command to split audio and video
                string splitcommand = string.Format("-i {0} -map 0:0 -vcodec copy {1} -map 0:1 -acodec copy {2}", convertFile.Filename, videoOutputFile.Filename, audioOutputFile.Filename);
                engine.CustomCommand(splitcommand);

                Console.WriteLine("\n------------\nMerging Audio & Video...\n------------");
                // Custom ffmpeg command to merge audio & video
                string mergecommand = string.Format("-i {0} -i {1} -map 0:0 -map 1:0 -shortest {2}", videoOutputFile.Filename, audioOutputFile.Filename, mergeOutputFile.Filename);
                engine.CustomCommand(mergecommand);

                Console.WriteLine("\nDone!");
            }
        }

        // Set video quality and aspect ratio
        public static void Encode(MediaFile input, MediaFile output)
        {
            using (var engine = new Engine())
            {
                // There is the option to output progress on conversion
                //engine.ConvertProgressEvent += ConvertProgressEvent;
                //engine.ConversionCompleteEvent += engine_ConversionCompleteEvent;
                var options = new ConversionOptions
                {
                    // Video size/quality
                    VideoSize = VideoSize.Hd480,
                    // Aspect Ration
                    VideoAspectRatio = VideoAspectRatio.R16_9
                };

                engine.Convert(input, output, options);
            }
        }

        // Progress Event methods
        private void ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            Console.WriteLine("\n------------\nConverting...\n------------");
            Console.WriteLine("Bitrate: {0}", e.Bitrate);
            Console.WriteLine("Fps: {0}", e.Fps);
            Console.WriteLine("Frame: {0}", e.Frame);
            Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            Console.WriteLine("SizeKb: {0}", e.SizeKb);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }

        private void engine_ConversionCompleteEvent(object sender, ConversionCompleteEventArgs e)
        {
            Console.WriteLine("\n------------\nConversion complete!\n------------");
            Console.WriteLine("Bitrate: {0}", e.Bitrate);
            Console.WriteLine("Fps: {0}", e.Fps);
            Console.WriteLine("Frame: {0}", e.Frame);
            Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            Console.WriteLine("SizeKb: {0}", e.SizeKb);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }
    }
}
