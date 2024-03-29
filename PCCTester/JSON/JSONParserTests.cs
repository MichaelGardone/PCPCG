using PCC.ContentRepresentation.Features;
using PCC.Utility.Range;
using PCC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCCTester.JSON
{
    [TestClass]
    public class JSONParserTests
    {
        string TEST_BASE_PATH =
            Directory.GetParent(
                Directory.GetParent(
                    Directory.GetParent(
                        Directory.GetCurrentDirectory()).ToString()).ToString()).ToString();


        [TestMethod]
        public void PPParseTest()
        {
            
        }

        [TestMethod]
        public void CherriesParseTest()
        {
            Feature feature = new Feature("cherries", new IntRange(
                new List<Tuple<int, int>>() {
                    new Tuple<int, int>(0, 2),
                    new Tuple<int, int>(4, 6),
                    new Tuple<int, int>(9, 11),
                    new Tuple<int, int>(13, 20),
                })
            );

            //Dictionary<string, Feature> features = JSONParser.ReadFeatures(TEST_BASE_PATH + "/data/cherries.json");

        }
    }
}
