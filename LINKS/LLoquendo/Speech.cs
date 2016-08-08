﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTTS7Lib;
using System.Text.RegularExpressions;
using System.Reflection;

namespace LLoquendo
{
    public class Speech
    {
        static List<LoqSynth> SpeechSynth = new List<LoqSynth>();
        static string lastVoice = "";
        static string lastVolume = "";
        static string lastRate = "";
        static string lastPhrase = "";
        //[LLoquendo.Speech.Speak("phrase","volume","rate","voice")]
        public static string Speak(string phrase, string volume, string rate, string voice)
        {
            if (SpeechSynth.Count == 0)
            {
                SpeechSynth.Add(new LoqSynth(voice));
            }
     
//#if LINKSEXISTS
            try
            {
                if (jarvisWPF.PublicClass.SpeechSynth != null)
                {
                    while (jarvisWPF.PublicClass.SpeechSynth.SpeechPrompts.Count > 0)
                    {
                        System.Threading.Thread.Sleep(300);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Can't wait for LINKS to finish talking...");
                //System.Diagnostics.Debugger.Break();
            }
//#endif


            lastVoice = voice;
            lastVolume = volume;
            lastRate = rate;
            lastPhrase = phrase;

            return SpeechSynth.FirstOrDefault().Speak(phrase, volume, rate, voice);

            //if (SpeechSynth.Find(ls => ls.Voice == voice) == null)
            //{
            //    SpeechSynth.Add(new LoqSynth(voice));
            //}

            //foreach (LoqSynth ls in SpeechSynth.FindAll(ls => ls.Voice != voice))
            //{
            //    ls.Speak(phrase, "0", rate, "");
            //    Console.WriteLine("foreach voice: " + ls.Voice);
            //}

            //Console.WriteLine("return voice: " + voice);
            //return SpeechSynth.First(ls => ls.Voice == voice).Speak(phrase, volume, rate, voice);
        }

        //[LLoquendo.Speech.Stop()]
        public static string Stop()
        {
            SpeechSynth.FirstOrDefault().LoquendoX.Stop();
            return "";
        }

        public static string RepeatLastPhrase()
        {
            if (lastPhrase.Contains("=English"))
            {
                lastPhrase = lastPhrase.Substring((@"\language=English ".Length + 2));
            }
            SpeechSynth.FirstOrDefault().Speak(lastPhrase, lastVolume, lastRate, lastVoice);
            return "";
        }

        public static string Stop(string phrase)
        {
            SpeechSynth.FirstOrDefault().LoquendoX.Stop();
            System.Threading.Thread.Sleep(500);

            if (lastPhrase.Contains("EnglishUs"))
            {
                phrase = @"\language=EnglishUs " + phrase;
            }
            else if (lastPhrase.Contains("EnglishGb"))
            {
                phrase = @"\language=EnglishGb " + phrase;
            }

            SpeechSynth.FirstOrDefault().Speak(phrase, lastVolume, lastRate, lastVoice);
            return "";
        }

        public static string Pause()
        {
            SpeechSynth.FirstOrDefault().LoquendoX.Pause();
            return "";
        }

        public static string Resume()
        {
            SpeechSynth.FirstOrDefault().LoquendoX.Resume();
            return "";
        }


        static void Main(string[] args)
        {
            Console.Title = "LINKS - Loquendo Plugin";                      
            
            Console.WriteLine(((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0]).Title);
            Console.WriteLine("Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine(((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright + " " + "All Rights Reserved");                        
            Console.WriteLine("================================================================================");
            Console.WriteLine();
            Console.WriteLine("Syntax:");
            Console.WriteLine("  [LLoquendo.Speech.Speak(\"phrase\",\"volume\",\"rate\",\"voice\")]");
            Console.WriteLine("  [LLoquendo.Speech.Stop(\"phrase to speak after stop speaking\")]");
            Console.WriteLine("  [LLoquendo.Speech.Stop()]");
            Console.WriteLine("  [LLoquendo.Speech.Pause()]");
            Console.WriteLine("  [LLoquendo.Speech.Resume()]");
            Console.WriteLine("  [LLoquendo.Speech.RepeatLastPhrase()]");
            Console.WriteLine();
            Console.WriteLine("Sample action from LINKS:");
            Console.WriteLine("  LLoquendo.Speech.Speak(\"I am Kate.\\voice=Simon I am Simon.\",\"100\",\"50\",\"Kate\")");
            Console.WriteLine();
            Console.WriteLine("================================================================================");
            Console.WriteLine();

            try
            {
                Console.WriteLine("Testing Kate's and Simon's voice...");
                LLoquendo.Speech.Speak("I am Kate.\\voice=Simon I am Simon.", "100", "50", "Kate");
                LLoquendo.Speech.Speak("Simon again.", "100", "10", "Simon");
                Console.WriteLine("Testing Simon's voice...");
                LLoquendo.Speech.Speak("And Kate again.", "100", "50", "");
                Console.WriteLine("Testing Kates's voice...");
            }
            catch (Exception error)
            {
                Console.WriteLine();
                Console.WriteLine("Error: " + error.Message);
                Console.WriteLine("Please install Loquendo SDK to use this plugin.");                
            }

            Console.WriteLine();
            Console.Write("Press any key to exit.");
            Console.ReadKey();
            //string t = GetParsedPhrase("\\hello_01 what time is it", PhraseFor.Loquendo);
            //t = GetParsedPhrase("\\hello_01 what time is it", PhraseFor.Other);
        }
    }


    class LoqSynth
    {
        LTTS7 loquendoX = null;
        string _voice = "";
        private List<string> voices;

        internal LTTS7 LoquendoX
        {
            get
            {
                return loquendoX;
            }

            set
            {
                loquendoX = value;
            }
        }

        internal string Voice
        {
            get
            {
                return _voice;
            }

            set
            {
                _voice = value;
            }
        }

        internal LoqSynth(string voice)
        {
            LoquendoX = new LTTS7();
            Voice = voice;
            GatherVoices();

            ((_DLTTS7)loquendoX).Voice = Voice;

            LoquendoX.SetAttribute("TextFormat", "SSML");
            LoquendoX.AudioChannels = "Stereo";
            LoquendoX.Device = 0;
            LoquendoX.Volume = 100;
            //LoquendoX.EndOfSpeech += new _DLTTS7Events_EndOfSpeechEventHandler(Loquendo_EndOfSpeech);
        }

        private void GatherVoices()
        {
            voices = new List<string>();

            string enumVoice = LoquendoX.EnumFirstVoice("");
            while (enumVoice != "")
            {
                voices.Add(enumVoice);
                enumVoice = LoquendoX.EnumNextVoice();
                //string lan = LoquendoX.GuessLanguage(enumVoice);
                //lan = LoquendoX.GuessFileLanguage(enumVoice);
            }
            
        }

        internal string Speak(string phrase, string volume, string rate, string voice)
        {
            string retVal = string.Empty;
            //System.Diagnostics.Debugger.Break();

            try
            {
                //System.Diagnostics.Debugger.Break();
                if (rate == string.Empty)
                {
                    rate = "50";
                }
                
                if (voice != string.Empty && voices.Find(v => v == voice) == null)
                {
                    if (phrase.Contains("\\"))
                    {
                        phrase = GetParsedPhrase(phrase, PhraseFor.Other);
                    }
                    retVal = phrase;
                }
                else
                {
                    
                    if (volume == "")
                    {
                        //LoquendoX.Volume = jarvisWPF.PublicClass.SpeechSynth.VoiceConfigs.Get(Voice).VoiceVolume;
//#if LINKSEXISTS
                        try
                        {
                            volume = jarvisWPF.PublicClass.SpeechSynth.VoiceConfigs.Get(Voice).VoiceVolume.ToString();
                        }
                        catch
                        {
                            Console.WriteLine("Can't get volume from LINKS...");
                            //System.Diagnostics.Debugger.Break();
                        }
//#endif
                    }
                    else
                    {
                        //LoquendoX.Volume = int.Parse(volume);
                    }
                    //LoquendoX.Speed = int.Parse(rate);

                    phrase = phrase.Replace("\\\\", "\\");

                    //if (voice != "USE FIRST VOICE")
                    
                    if (phrase.StartsWith(@"\language", StringComparison.OrdinalIgnoreCase))
                    {
                        LoquendoX.Read(string.Format(@"\voice={0} \volume={2} \speed={3} {1}", voice, phrase, volume, rate));
                    }
                    else
                    {
                        string lang = null;
//#if LINKSEXISTS
                        try
                        {
                            if (jarvisWPF.PublicClass.SpeechSynth != null)
                            {
                                try
                                {
                                    lang = jarvisWPF.PublicClass.SpeechSynth.VoiceConfigs.Get(Voice).VoiceLanguage;
                                }
                                catch { }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Can't get language from LINKS...");
                            //System.Diagnostics.Debugger.Break();
                        }
//#endif

                        LoquendoX.Read(string.Format(@"\language={4} \voice={0} \volume={2} \speed={3} {1}", voice, phrase, volume, rate, lang));
                    }
                }
                //Task.WaitAll();
            }
            catch
            {
                if (phrase.Contains("\\"))
                {
                    phrase = GetParsedPhrase(phrase, PhraseFor.Other);
                }

                retVal = phrase;
            }

            return retVal;
        }

        private void Loquendo_StartOfSpeech()
        {
            //throw new NotImplementedException();
        }

        private void Loquendo_EndOfSpeech()
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string log = AppData + "\\LINKS\\Customization\\Plugins\\LLoquendo\\";
            System.IO.Directory.CreateDirectory(log);
            log += "log.txt";
            System.IO.File.WriteAllText(log, "Speech Completed");
            System.Threading.Thread.Sleep(100);
        }

        enum PhraseFor
        {
            Loquendo, Other
        }

        private string GetParsedPhrase(string phrase, PhraseFor phraseFor)
        {
            string retVal = "";
            string pattern = @"(\\\w+)";

            // Instantiate the regular expression object.
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase);

            // Replace \word with string.empty or \item=word
            retVal = regEx.Replace(phrase, delegate (Match match) {
                if (match.Groups[1].Value == "") return match.Value;
                else return phraseFor == PhraseFor.Other ? string.Empty : "\\item=" + match.Value.Substring(1);
            });

            return retVal.Trim();
        }
    }
}


