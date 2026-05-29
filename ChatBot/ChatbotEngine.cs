using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBOT.ChatBot
{
    // Delegate for chatbot response events
    public delegate void ResponseGeneratedHandler(string response, ResponseType type);

    public enum ResponseType
    {
        Normal,
        Sentiment,
        Memory,
        Error,
        Exit
    }

    // Stores user memory/recall info
    public class UserMemory
    {
        public string Name { get; set; } = "";
        public string FavouriteTopic { get; set; } = "";
        public string LastTopic { get; set; } = "";
        public List<string> InterestHistory { get; set; } = new List<string>();
    }

    public class ChatbotEngine
    {
        private readonly Random _random = new Random();
        private UserMemory _memory = new UserMemory();

        // Event raised whenever the bot generates a response
        public event ResponseGeneratedHandler? OnResponse;

        //Keyword multiple responses (Dictionary<string, List<string>>)
        private readonly Dictionary<string, List<string>> _keywordResponses = new()
        {
            ["password"] = new List<string>
            {
                "Use strong passwords with a mix of uppercase, lowercase, numbers, and symbols. Never reuse passwords across accounts.",
                "A good password is at least 12 characters long. Consider using a passphrase like 'Green$Horse!Mountain9'.",
                "Enable a password manager to keep track of unique, complex passwords for every site you use.",
                "Avoid using personal details like birthdays or names in your passwords attackers check those first!"
            },
            ["phishing"] = new List<string>
            {
                "Be cautious of emails asking for personal information. Always verify the sender's address before clicking any links.",
                "Scammers often disguise themselves as trusted organisations. When in doubt, go directly to the official website.",
                "Look out for urgent language like 'Your account will be closed!' this is a classic phishing tactic.",
                "Hover over links before clicking to check if the URL looks suspicious or different from what you'd expect."
            },
            ["scam"] = new List<string>
            {
                "Online scams often promise something too good to be true. If it sounds too good, it probably is.",
                "Never send money or gift cards to someone you've only met online this is a major red flag.",
                "Report any suspected scams to the South African Police Service (SAPS) or your bank immediately.",
                "Scammers use pressure tactics to make you act fast. Slow down, verify, and consult someone you trust."
            },
            ["privacy"] = new List<string>
            {
                "Review the privacy settings on your social media accounts regularly to control who sees your information.",
                "Be mindful of what personal data you share online even small details can be pieced together by attackers.",
                "Use a VPN when connecting to public Wi-Fi to protect your browsing data from snoopers.",
                "Privacy is a right. Read app permissions carefully before granting access to your camera, location, or contacts."
            },
            ["malware"] = new List<string>
            {
                "Keep your antivirus software updated and run regular scans to detect any malicious software.",
                "Avoid downloading software from untrusted websites stick to official sources and app stores.",
                "Malware can hide in email attachments. Never open files from unknown senders.",
                "Ransomware can encrypt your files. Always back up important data to an offline or cloud location."
            },
            ["safe browsing"] = new List<string>
            {
                "Always look for 'https://' in the URL bar before entering sensitive information on a website.",
                "Keep your browser and plugins updated to patch security vulnerabilities.",
                "Use browser extensions like uBlock Origin to block malicious ads and trackers.",
                "Avoid clicking pop-up ads many are designed to download malware onto your device."
            },
            ["two factor"] = new List<string>
            {
                "Enable two-factor authentication (2FA) on all accounts,it adds a critical extra layer of security.",
                "Even if someone steals your password, 2FA stops them from logging in without your phone or email code.",
                "Use an authenticator app like Google Authenticator rather than SMS codes where possible it's more secure."
            },
            ["vpn"] = new List<string>
            {
                "A VPN encrypts your internet traffic, keeping you safer on public Wi-Fi networks.",
                "Choose a reputable paid VPN free VPNs often log and sell your data, defeating the purpose.",
                "VPNs help protect your privacy but aren't a silver bullet always pair them with good browsing habits."
            }
        };

        //Sentiment keywords
        private readonly Dictionary<string, string> _sentimentPrefixes = new()
        {
            ["worried"] = "It's completely understandable to feel that way. Cybersecurity can feel overwhelming, but you're taking the right steps by learning. ",
            ["scared"] = "Don't worry knowledge is your best defence. Let me share something helpful: ",
            ["anxious"] = "Take a breath you've got this! Here's what you need to know: ",
            ["frustrated"] = "I hear you this stuff can be tricky. Let me try to explain it clearly: ",
            ["confused"] = "No problem at all let me break this down simply for you: ",
            ["curious"] = "Great curiosity! The more you know, the safer you'll be. Here's the detail: ",
            ["interested"] = "Awesome! Being interested in cybersecurity is the first step to staying safe. ",
            ["unsure"] = "That's okay it's better to ask than to guess. Here's some guidance: "
        };

        //Follow-up / conversation flow triggers
        private readonly List<string> _followUpTriggers = new()
        {
            "tell me more", "explain more", "more details", "give me another tip",
            "another tip", "elaborate", "go on", "continue", "more", "what else"
        };

        //General conversation
        private readonly Dictionary<string, string> _generalResponses = new()
        {
            ["how are you"] = "I'm a bot, so I don't have feelings, but I'm fully charged and ready to help keep you safe online!",
            ["whats your purpose"] = "I'm your Cybersecurity Awareness Bot! I'm here to teach you how to stay safe online.",
            ["what can i ask"] = "You can ask me about: passwords, phishing, scams, privacy, malware, safe browsing, VPNs, and two-factor authentication.",
            ["hello"] = "Hello! Great to see you. Ask me anything about cybersecurity.",
            ["hi"] = "Hi there! I'm your cybersecurity assistant. What would you like to know?",
            ["help"] = "I can help with: passwords, phishing, scams, privacy, malware, safe browsing, VPNs, and two-factor authentication. Just ask!"
        };

  
        public void SetUserName(string name)
        {
            _memory.Name = name;
        }

        public string GetUserName() => _memory.Name;

        // Main processing method — returns (response text, ResponseType)
        public (string response, ResponseType type) ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return ("Please type a message first!", ResponseType.Error);

            string cleaned = input.ToLower().Trim()
                .Replace("?", "").Replace(".", "").Replace("'", "").Replace(",", "");

            // Exit check
            if (cleaned == "exit" || cleaned == "quit" || cleaned == "bye")
                return ($"Goodbye, {_memory.Name}! Stay safe online. ", ResponseType.Exit);

            // Follow-up / conversation flow
            if (_followUpTriggers.Any(t => cleaned.Contains(t)))
            {
                if (!string.IsNullOrEmpty(_memory.LastTopic) && _keywordResponses.ContainsKey(_memory.LastTopic))
                {
                    string tip = GetRandomResponse(_memory.LastTopic);
                    return ($"Sure! Here's another tip on {_memory.LastTopic}: {tip}", ResponseType.Normal);
                }
                return ("I'd love to elaborate! Could you tell me which topic you'd like more on — for example, passwords, phishing, or privacy?", ResponseType.Normal);
            }

            // Detect sentiment FIRST, then find topic
            string sentimentPrefix = DetectSentiment(cleaned);

            // Check "I'm interested in X" memory pattern
            if (cleaned.Contains("interested in") || cleaned.Contains("i like") || cleaned.Contains("i love") || cleaned.Contains("i want to learn about"))
            {
                foreach (var keyword in _keywordResponses.Keys)
                {
                    if (cleaned.Contains(keyword))
                    {
                        _memory.FavouriteTopic = keyword;
                        if (!_memory.InterestHistory.Contains(keyword))
                            _memory.InterestHistory.Add(keyword);
                        _memory.LastTopic = keyword;
                        string tip = GetRandomResponse(keyword);
                        return ($"Great! I'll remember that you're interested in {keyword}. {tip}", ResponseType.Memory);
                    }
                }
            }

            // Keyword recognition
            foreach (var keyword in _keywordResponses.Keys)
            {
                if (cleaned.Contains(keyword))
                {
                    _memory.LastTopic = keyword;
                    string tip = GetRandomResponse(keyword);
                    string personalised = BuildPersonalisedResponse(keyword, sentimentPrefix + tip);
                    ResponseType rType = string.IsNullOrEmpty(sentimentPrefix) ? ResponseType.Normal : ResponseType.Sentiment;
                    return (personalised, rType);
                }
            }

            // General conversation
            foreach (var key in _generalResponses.Keys)
            {
                if (cleaned.Contains(key))
                    return (_generalResponses[key], ResponseType.Normal);
            }

            // Memory recall — reference favourite topic
            if (!string.IsNullOrEmpty(_memory.FavouriteTopic))
            {
                string recallTip = GetRandomResponse(_memory.FavouriteTopic);
                return ($"I'm not sure I understood that. As someone interested in {_memory.FavouriteTopic}, you might find this useful: {recallTip}", ResponseType.Memory);
            }

            // Default error/fallback
            return ("I'm not sure I understand that. Try asking about passwords, phishing, scams, privacy, malware, safe browsing, VPNs, or two-factor authentication.", ResponseType.Error);
        }

        //Picks a random response for a keyword
        private string GetRandomResponse(string keyword)
        {
            if (!_keywordResponses.ContainsKey(keyword)) return "";
            var list = _keywordResponses[keyword];
            return list[_random.Next(list.Count)];
        }

        //Prepend memory personalisation if relevant
        private string BuildPersonalisedResponse(string keyword, string baseResponse)
        {
            if (!string.IsNullOrEmpty(_memory.FavouriteTopic) && _memory.FavouriteTopic == keyword)
                return $"As someone interested in {keyword}, here's a tip especially for you: {baseResponse}";
            return baseResponse;
        }

        //Returns a sentiment prefix if sentiment detected, else empty string
        private string DetectSentiment(string input)
        {
            foreach (var kvp in _sentimentPrefixes)
            {
                if (input.Contains(kvp.Key))
                    return kvp.Value;
            }
            return "";
        }

        // Returns list of all supported topics for UI hints
        public List<string> GetTopics() => _keywordResponses.Keys.ToList();

        // Returns user memory summary for display
        public UserMemory GetMemory() => _memory;
    }
}