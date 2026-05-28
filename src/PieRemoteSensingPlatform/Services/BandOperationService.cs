using System.Collections.Generic;
using System.Linq;
using PIE.CommonAlgo;
using PIE.SystemAlgo;
using PieRemoteSensingPlatform.Models;

namespace PieRemoteSensingPlatform.Services
{
    public sealed class BandOperationService
    {
        private readonly PieAlgorithmRunner algorithmRunner;

        public BandOperationService(PieAlgorithmRunner algorithmRunner)
        {
            this.algorithmRunner = algorithmRunner;
        }

        public ISystemAlgo CreateIndexAlgorithm(RemoteSensingIndexDefinition definition, string inputFilePath, string outputFilePath)
        {
            var parameters = new BandOper_Exchange_Info
            {
                StrExp = definition.Expression,
                SelectFileBands = definition.Bands.ToList(),
                SelectFileNames = RepeatInputFile(inputFilePath, definition.Bands.Length),
                OutputFilePath = outputFilePath,
                FileTypeCode = definition.FileTypeCode,
                PixelDataType = definition.PixelDataType
            };

            return algorithmRunner.CreateAlgorithm(
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.BandOperAlgo",
                "BandOperAlgo",
                definition.DisplayName,
                parameters);
        }

        private static List<string> RepeatInputFile(string inputFilePath, int count)
        {
            return Enumerable.Repeat(inputFilePath, count).ToList();
        }
    }
}
