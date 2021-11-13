using FileParser.Implementation;
using FileParser.Interface;
using Moq;
using NUnit.Framework;
using System;
using System.Data;
using System.IO;

namespace FileParser.UnitTests
{
    [TestFixture]
    public class FileParserTest
    {
        Mock<IFileParserService> mockFileParserService;
        FileParserService fileParser;
        DataTable dt;
        [SetUp]
        public void Setup()
        {
            string json;
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            var path = Path.Combine(projectDirectory, @"TestData.json");

            using (StreamReader r = new StreamReader(path))
            {
                json = r.ReadToEnd();
            }
            var testData = (TestModel)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(TestModel));
            Helper helper = new Helper();
            dt = helper.ToDataTable(testData.Data);
        }

        [Test]
        public void TestConvertToCSVMethod()
        {
            string commaSeparatedValues = "ISIN,CFICode,Venue,ContractSize\r\nDE000ABCDEFG,FFICSX,XEUR,20\r\nPL0ABCDEFGHI,FFICSX,WDER,25\r\n";

            mockFileParserService = new Mock<IFileParserService>(MockBehavior.Strict);
            mockFileParserService.Setup(p => p.ConvertToCsv(dt)).Returns(commaSeparatedValues);

            fileParser = new FileParserService();
            var result = fileParser.ConvertToCsv(dt);

            Assert.AreEqual(commaSeparatedValues, result);

        }
    }
}