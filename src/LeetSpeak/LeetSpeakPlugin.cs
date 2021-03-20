// ReSharper disable UnusedMember.Global
// ReSharper disable DelegateSubtraction
// ReSharper disable UnusedType.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

using System;
using System.Collections.Generic;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Plugin;

namespace LeetSpeak
{
    public class LeetSpeakPlugin : IDalamudPlugin
    {
        private readonly Dictionary<char, string> _mappedChars = new Dictionary<char, string>();

        private readonly Dictionary<string, string> _mappedWords = new Dictionary<string, string>();
        private readonly Random _random = new Random();
        private DalamudPluginInterface _pluginInterface;
        public string Name => "LeetSpeak";

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
            pluginInterface.Framework.Gui.Chat.OnChatMessage += ChatOnOnChatMessage;

            // add words
            _mappedWords.Add("leet", @"1337");
            _mappedWords.Add("bye", @"bai");
            _mappedWords.Add("and", @"nd");
            _mappedWords.Add("dude", @"d00d");
            _mappedWords.Add("girl", @"gurl");
            _mappedWords.Add("guys", @"guise");
            _mappedWords.Add("hacks", @"h4x0rz");
            _mappedWords.Add("hi", @"hai");
            _mappedWords.Add("cool", @"kewl");
            _mappedWords.Add("ok", @"kk");
            _mappedWords.Add("fuck", @"fuq");
            _mappedWords.Add("the", @"teh");
            _mappedWords.Add("sucks", @"sux");
            _mappedWords.Add("owo", @"r4wr");
            _mappedWords.Add("uwu", @"r4wr");
            _mappedWords.Add("like", @"liek");
            _mappedWords.Add("smart", @"smat");
            _mappedWords.Add("mate", @"m8");
            _mappedWords.Add("power", @"powwah");
            _mappedWords.Add("porn", @"pr0n");
            _mappedWords.Add("what", @"wut");
            _mappedWords.Add("you", @"u");
            _mappedWords.Add("are", @"r");
            _mappedWords.Add("why", @"y");
            _mappedWords.Add("yes", @"yass");
            _mappedWords.Add("yeah", @"ya");
            _mappedWords.Add("rocks", @"roxx0rs");
            _mappedWords.Add("yay", @"w00t");
            _mappedWords.Add("whatever", @"w/e");

            // add characters (maybe too leet)
            _mappedChars.Add('a', @"4");
            _mappedChars.Add('b', @"8");
            _mappedChars.Add('c', @"(");
            _mappedChars.Add('d', @"|)");
            _mappedChars.Add('e', @"3");
            _mappedChars.Add('g', @"6");
            _mappedChars.Add('h', @"|-|");
            _mappedChars.Add('i', @"!");
            _mappedChars.Add('k', @"|<");
            _mappedChars.Add('l', @"|_");
            _mappedChars.Add('m', @"|\/|");
            _mappedChars.Add('n', @"/\/");
            _mappedChars.Add('o', @"0");
            _mappedChars.Add('r', @"|2");
            _mappedChars.Add('s', @"5");
            _mappedChars.Add('t', @"7");
            _mappedChars.Add('u', @"|_|");
            _mappedChars.Add('v', @"\/");
            _mappedChars.Add('w', @"\/\/");
            _mappedChars.Add('y', @"'/");
        }

        public void Dispose()
        {
            _pluginInterface.Framework.Gui.Chat.OnChatMessage -= ChatOnOnChatMessage;
            _pluginInterface.Dispose();
        }

        private void ChatOnOnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message,
            ref bool isHandled)
        {
            foreach (var payload in message.Payloads)
                if (payload is TextPayload textPayload)
                    textPayload.Text = LeetSpeakify(textPayload.Text);
        }

        private string LeetSpeakify(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            input = input.ToLower();
            var words = input.Split(' ');
            var output = new List<string>();

            foreach (var word in words)
                if (_mappedWords.ContainsKey(word))
                {
                    output.Add(_mappedWords[word]);
                }
                else
                {
                    var outputWord = string.Empty;
                    foreach (var ch in word)
                    {
                        var outputCh = _mappedChars.ContainsKey(ch) ? _mappedChars[ch] : ch.ToString();
                        if (_random.Next(0, 2) == 0) outputCh = outputCh.ToUpper();
                        outputWord += outputCh;
                    }

                    output.Add(outputWord);
                }

            return string.Join(" ", output);
        }
    }
}