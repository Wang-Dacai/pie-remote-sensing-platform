using System.Collections.Generic;
using System.Linq;

namespace PieRemoteSensingPlatform.Models
{
    public sealed class RemoteSensingIndexDefinition
    {
        private RemoteSensingIndexDefinition(string key, string displayName, string expression, IEnumerable<int> bands)
        {
            Key = key;
            DisplayName = displayName;
            Expression = expression;
            Bands = bands.ToArray();
            FileTypeCode = "GTiff";
            PixelDataType = 7;
        }

        public string Key { get; private set; }

        public string DisplayName { get; private set; }

        public string Expression { get; private set; }

        public int[] Bands { get; private set; }

        public string FileTypeCode { get; private set; }

        public int PixelDataType { get; private set; }

        public static RemoteSensingIndexDefinition VegetationCoverage()
        {
            return new RemoteSensingIndexDefinition(
                "vegetation-coverage",
                "植被盖度",
                "(b1-b2)/(b1+b2)",
                new[] { 5, 4 });
        }

        public static RemoteSensingIndexDefinition Evi()
        {
            return new RemoteSensingIndexDefinition(
                "evi",
                "增强型植被指数 EVI",
                "2.5*(b1-b2)/(b1+6*b2-7.5*b3+1)",
                new[] { 5, 4, 2 });
        }

        public static RemoteSensingIndexDefinition WaterExtraction()
        {
            return new RemoteSensingIndexDefinition(
                "water-extraction",
                "水体提取",
                "(((b1-b2)/(b1+b2))>0.5)*((b1-b2)/(b1+b2))+(((b1-b2)/(b1+b2))<=0.5)*0",
                new[] { 3, 5 });
        }

        public static RemoteSensingIndexDefinition BlackOdorousWater()
        {
            return new RemoteSensingIndexDefinition(
                "black-odorous-water",
                "黑臭水体监测",
                "((((b1-b2)/(b1+b2))>=(-0.03))AND(((b1-b2)/(b1+b2))<=(-0.015)))*((b1-b2)/(b1+b2))+((((b1-b2)/(b1+b2))<=(-0.03))OR(((b1-b2)/(b1+b2))>=(-0.015)))*0",
                new[] { 2, 3 });
        }

        public static RemoteSensingIndexDefinition BuildingLandscape()
        {
            return new RemoteSensingIndexDefinition(
                "building-landscape",
                "建筑景观聚类",
                "(((b1-b2)/(b1+b2))>(-0.03))*((b1-b2)/(b1+b2))+(((b1-b2)/(b1+b2))<=(-0.03))*0",
                new[] { 6, 5 });
        }

        public static RemoteSensingIndexDefinition Wetness()
        {
            return new RemoteSensingIndexDefinition(
                "wetness",
                "湿度监测",
                "0.0315*b1+0.2021*b2+0.1594*b3-0.6806*b4-0.6109*b5",
                new[] { 2, 3, 5, 6, 7 });
        }

        public static RemoteSensingIndexDefinition SoilSalinity()
        {
            return new RemoteSensingIndexDefinition(
                "soil-salinity",
                "土壤盐度指数",
                "(b1-b2)/(b1+b2)",
                new[] { 4, 5 });
        }

        public static RemoteSensingIndexDefinition SoilStrength()
        {
            return new RemoteSensingIndexDefinition(
                "soil-strength",
                "土壤强度指数",
                "(b1+b2+b3)/3",
                new[] { 3, 4, 5 });
        }

        public static RemoteSensingIndexDefinition Greenness()
        {
            return new RemoteSensingIndexDefinition(
                "greenness",
                "绿度",
                "(b1-b2)/(b1+b2)",
                new[] { 4, 5 });
        }

        public static RemoteSensingIndexDefinition Dryness()
        {
            return new RemoteSensingIndexDefinition(
                "dryness",
                "干度",
                "(((2*b6/(b5+b4)-(b4/(b3+b4)+b2/(b2+b5))))/((2*b6/(b5+b4)+(b4/(b3+b4)+b2/(b2+b5))))+((b5-b3)-(b1+b4))/((b5+b3)+(b1+b4)))/2",
                new[] { 2, 3, 4, 5, 6, 7 });
        }

        public static RemoteSensingIndexDefinition BlackOdorousWaterQuickLook()
        {
            return new RemoteSensingIndexDefinition(
                "black-odorous-water-quick-look",
                "黑臭水体快速识别",
                "((((b1-b2)/(b1+b2))>=(-0.03))AND(((b1-b2)/(b1+b2))<=(-0.015)))*((b1-b2)/(b1+b2))+((((b1-b2)/(b1+b2))<=(-0.03))OR(((b1-b2)/(b1+b2))>=(-0.015)))*0",
                new[] { 2, 3 });
        }

        public static RemoteSensingIndexDefinition SoilStrengthQuickLook()
        {
            return new RemoteSensingIndexDefinition(
                "soil-strength-quick-look",
                "土壤强度快速计算",
                "(b1+b2+b3)/3",
                new[] { 3, 4, 5 });
        }
    }
}
