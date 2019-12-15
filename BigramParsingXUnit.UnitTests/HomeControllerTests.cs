using System;
using Xunit;
using BigramParsing.Models;
using BigramParsing.Controllers;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;

namespace BigramParsingXUnit.UnitTests
{
    public class HomeControllerTests
    {
        // testing what the values and keys would look like if they each got converted into a string
        [Theory]
        [InlineData("./InputTestFiles/one_bigram_one_line.txt", "the dog", "1")]

        [InlineData("./InputTestFiles/one_bigram_three_lines.txt", "the dog", "1")]

        [InlineData("./InputTestFiles/four_bigram_contractions", "this isn't isn't this", "2 2")]

        [InlineData("./InputTestFiles/many_bigrams_much_spacing_caps_contractions_numbers.zoo",
                    "another example example of of some some another example can can we we have have some " +
                    "some bigram bigram soup soup some soup and and more more soup soup another of bigram " +
                    "soup with with of another and soup 3 3 soup soup 5 5 soup soup 4 4 soup soup " +
                    "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm",
                    "3 2 2 2 1 1 1 1 2 3 1 1 2 2 1 1 1 1 1 2 2 1 1 2 2 1")]

        [InlineData("./InputTestFiles/one_word.txt", "", "")]

        [InlineData("./InputTestFiles/one_line_leadingtrailinginbetween_space.txt",
                    "a line line out out the the dog dog door door line line fine fine door door dog dog out " +
                    "the line out of of line line soup soup time time time time door door time time soup time fancy " +
                    "fancy cat cat line",
                    "1 2 2 1 1 1 1 1 1 1 1 1 1 2 2 1 1 1 1 1 1 1")]

        [InlineData("./InputTestFiles/all_last_line.txt",
                    "this is is this is test test testing testing house house plant plant to to see see if if this " +
                    "is see if bigram bigram house plant bigram bigram bigram house cat cat see if house plant cat " +
                    "cat house plant house house plan", "3 1 1 1 1 5 1 1 3 1 1 1 2 1 2 2 1 1 1 2 2 1")]
        public void ExtractAndGroupBigrams(string path, string varKeys, string varVals)
        {
            var homeController = new HomeController(new NullLogger<HomeController>());

            using var fileStream = File.OpenRead(path);
            var bigramData = new BigramDataViewModel
            {
                BigramDictionary = homeController.ExtractAndGroupBigrams(FilePathToFormFile(fileStream))
            };

            string dictoKeys = string.Join(" ", bigramData.BigramDictionary.Keys.ToArray());
            string dictoVals = string.Join(" ", bigramData.BigramDictionary.Values.ToArray());

            Assert.Equal(varKeys, dictoKeys);
            Assert.Equal(varVals, dictoVals);
        }

        // turns a filestream into a formfile - path is source, IFormFile is destination
        public FormFile FilePathToFormFile(FileStream fileStream)
        {
            var inputTestFile = new FormFile(fileStream, 0, fileStream.Length, "toBigram", fileStream.Name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "plain/text"
            };
            return inputTestFile;
        }
    }
}
