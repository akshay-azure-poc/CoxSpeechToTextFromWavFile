using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace CoxSpeechToText
{
    class Program
    {
        static void Main(string[] args)
        {
            string _filename = "C:\\Repo\\Customers\\COX\\TextToSpeech\\PyronCalls\\Brandon Jackson 01 04 300102598160190104.wav";
            //string _filename = "C:\\Apps\\outputaudio.wav";
            SpeechToTextAsyncFromWavFile(_filename).Wait();
            //SpeechToTextFromWavFileInput(_filename).Wait();
            Console.WriteLine("Please press a key to continue.");
            Console.ReadLine();
        }

        public static async Task SpeechToTextFromWavFileInput(string wavfileName)
        {
            var taskCompleteionSource = new TaskCompletionSource<int>();
            var config = SpeechConfig.FromSubscription("64398c5cf34648c8bca0236a8f65c751", "westus");

            var transcriptionStringBuilder = new StringBuilder();

            using (var audioInput = AudioConfig.FromWavFileInput(wavfileName))
            {
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {
                    try
                    {
                        // Subscribes to events.  
                        recognizer.Recognizing += (sender, eventargs) =>
                        {
                        //TODO: Handle recognized intermediate result  
                    };

                        recognizer.Recognized += (sender, eventargs) =>
                        {
                            if (eventargs.Result.Reason == ResultReason.RecognizedSpeech)
                            {
                                transcriptionStringBuilder.Append(eventargs.Result.Text);
                            }
                            else if (eventargs.Result.Reason == ResultReason.NoMatch)
                            {
                            //TODO: Handle not recognized value  
                        }
                        };

                        recognizer.Canceled += (sender, eventargs) =>
                        {
                            if (eventargs.Reason == CancellationReason.Error)
                            {
                            //TODO: Handle error  
                        }

                            if (eventargs.Reason == CancellationReason.EndOfStream)
                            {
                                Console.WriteLine(transcriptionStringBuilder.ToString());
                            }

                            taskCompleteionSource.TrySetResult(0);
                        };

                        recognizer.SessionStarted += (sender, eventargs) =>
                        {
                        //Started recognition session  
                    };

                        recognizer.SessionStopped += (sender, eventargs) =>
                        {
                        //Ended recognition session  
                        taskCompleteionSource.TrySetResult(0);
                        };

                        // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.  
                        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                        // Waits for completion.  
                        // Use Task.WaitAny to keep the task rooted.  
                        Task.WaitAny(new[] { taskCompleteionSource.Task });

                        // Stops recognition.  
                        await recognizer.StopContinuousRecognitionAsync();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            Console.ReadKey();
        }
   
        public static async Task SpeechToTextAsyncFromWavFile(string wavfileName)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("64398c5cf34648c8bca0236a8f65c751", "westus");

            var stopRecognition = new TaskCompletionSource<int>();

            // Creates a speech recognizer using file as audio input.
            // Replace with your own audio file name.
            using (var audioInput = AudioConfig.FromWavFileInput(wavfileName))
            {
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {
                    var transcriptionStringBuilder = new StringBuilder();
                    // Subscribes to events.
                    recognizer.Recognizing += (s, e) =>
                    {
                        Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                    };

                    recognizer.Recognized += (s, e) =>
                    {
                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            transcriptionStringBuilder.Append(e.Result.Text);
                            Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        }
                    };

                    recognizer.Canceled += (s, e) =>
                    {
                        Console.WriteLine($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }

                        if (e.Reason == CancellationReason.EndOfStream)
                        {
                            Console.WriteLine(transcriptionStringBuilder.ToString());
                        }
                        stopRecognition.TrySetResult(0);
                    };

                    recognizer.SessionStarted += (s, e) =>
                    {
                        Console.WriteLine("\n    Session started event.");
                    };

                    recognizer.SessionStopped += (s, e) =>
                    {
                        Console.WriteLine("\n    Session stopped event.");
                        Console.WriteLine("\nStop recognition.");
                        stopRecognition.TrySetResult(0);
                    };

                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });

                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }
            }
        }
        public static async Task SpeechToTextAsyncwithMicrophone()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "eastus").
            var config = SpeechConfig.FromSubscription("64398c5cf34648c8bca0236a8f65c751", "westus");

            // Creates a speech recognizer.
            using (var recognizer = new SpeechRecognizer(config))
            {
                Console.WriteLine("Say something...");

                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query. 
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.

                var result = await recognizer.RecognizeOnceAsync();
                
                // Checks result.
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"We recognized: {result.Text}");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }
    }
}
