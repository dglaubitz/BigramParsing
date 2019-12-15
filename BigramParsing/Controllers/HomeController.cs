using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BigramParsing.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;


namespace BigramParsing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        // handles the file upload, validates, sends off to do work to put it into a dictionary
        [HttpPost]
        public IActionResult Upload(IFormFile toBigram)
        {
            var bigramData = new BigramDataViewModel();
            ViewBag.error = null;
            ViewBag.badFile = null;

            try
            {
                if (toBigram.Length > 0)
                {
                    bigramData.BigramDictionary = ExtractAndGroupBigrams(toBigram);
                }
                else
                {
                    ViewBag.error = "File empty - try a different one";
                    return View("Index");
                }
            }
            catch
            {
                ViewBag.badFile = "Bad file/no file!";
                return View("Index");
            }
            return View("Index", bigramData);
        }

        // called by Upload to read into bigrams and organize into dictionary
        public Dictionary<string,int> ExtractAndGroupBigrams(IFormFile toBigram)
        {
            Dictionary<string, int> bigramDictionary = new Dictionary<string, int>();
            string[] gramChunk = new string[2];

            try
            {
                // reading in from file a character at a time, making chunk assignments by word,
                // comparing bigram to dictionary along the way
                using var streamReader = new StreamReader(toBigram.OpenReadStream());
                gramChunk[0] = FindNextStreamReaderWord(streamReader);
                do
                {
                    gramChunk[1] = FindNextStreamReaderWord(streamReader);
                    if (!gramChunk.Any(x => x.Length <= 0))
                    {
                        UpdateBigramSet(bigramDictionary, String.Join(" ", gramChunk));
                        gramChunk[0] = gramChunk[1];
                    }
                } while (gramChunk[1].Length > 0 && !streamReader.EndOfStream);
            }
            catch (Exception ex)
            {
                ViewBag.error = "Error reading file";
                _logger.LogInformation("File read error: {0}", ex);
                return null;
            }
            if (bigramDictionary.Count < 1)
                ViewBag.error = "No bigrams!";

            return (bigramDictionary);
        }

        // reads the next word in from the file
        public string FindNextStreamReaderWord(StreamReader streamReader)
        {
            StringBuilder word = new StringBuilder();
            char tempChunkChar;

            // skipping leading spaces (and other) to find first char in next word
            do
            {
                tempChunkChar = (char)streamReader.Read();
            } while ((tempChunkChar <= 32 || tempChunkChar >= 127) && !streamReader.EndOfStream);

            // found beginning of word, read in until no more word
            if (!streamReader.EndOfStream)
            {
                do
                {
                    word.Append(tempChunkChar);
                    tempChunkChar = (char)streamReader.Read();
                } while (tempChunkChar > 32 && tempChunkChar < 127);
            }
            return word.ToString().ToLower().Trim();
        }

        // determines if entry exists in dictionary, counts if it does, adds it in if it doesn't
        public void UpdateBigramSet(Dictionary<string,int> bigramDictionary, string bigram)
        {
            if (!bigramDictionary.ContainsKey(bigram))
            {
                bigramDictionary.Add(bigram, 1);
            }
            else
            {
                bigramDictionary[bigram]++;
            }
        }     
    }
}
