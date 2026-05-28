using System.Collections.Generic;
using System.Windows.Forms;
using PIE.Carto;
using PIE.CommonAlgo;
using PIE.Controls;
using PIE.DataSource;
using PIE.Framework;
using PIE.SystemAlgo;

namespace PieRemoteSensingPlatform.Commands
{
    internal abstract class SupervisedClassificationCommandBase : BaseCommand
    {
        private SupervisedClassification_Exchange_Info classificationInfo;

        protected abstract string DisplayName { get; }

        protected abstract int ClassifierType { get; }

        protected abstract string AlgorithmClassName { get; }

        public override void OnClick()
        {
            var inputFilePath = SelectInputRaster();
            if (string.IsNullOrWhiteSpace(inputFilePath))
            {
                return;
            }

            var outputFilePath = SelectOutputRaster();
            if (string.IsNullOrWhiteSpace(outputFilePath))
            {
                return;
            }

            var roiLayer = FindRoiLayer();
            if (roiLayer == null)
            {
                MessageBox.Show("未找到名为 roi_layer 的 ROI 图层，请先在地图中创建训练样本。", DisplayName);
                return;
            }

            var selectedBandCount = 3;
            classificationInfo = new SupervisedClassification_Exchange_Info
            {
                InputFilePath = inputFilePath,
                OutputFilePath = outputFilePath,
                SelBandNums = selectedBandCount,
                ClassifierType = ClassifierType,
                SelBandIndexs = BuildBandIndexes(selectedBandCount)
            };

            var roiParameters = new ROIStatistics_Exchange_Info
            {
                ClassifierType = classificationInfo.ClassifierType,
                currentFileName = classificationInfo.InputFilePath,
                selBandIndex = classificationInfo.SelBandIndexs.ToArray(),
                selBandNums = classificationInfo.SelBandNums,
                pROILayer = roiLayer
            };

            var roiAlgorithm = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.ROIStatisticsAlgo");
            if (roiAlgorithm == null)
            {
                MessageBox.Show("无法创建 ROI 统计算法。", DisplayName);
                return;
            }

            roiAlgorithm.Params = roiParameters;
            if (!AlgoFactory.Instance().ExecuteAlgo(roiAlgorithm))
            {
                MessageBox.Show("ROI 统计执行失败。", DisplayName);
                return;
            }

            FillClassificationParameters((ROIStatistics_Exchange_Info)roiAlgorithm.Params, selectedBandCount);

            var classificationAlgorithm = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", AlgorithmClassName);
            if (classificationAlgorithm == null)
            {
                MessageBox.Show("无法创建分类算法。", DisplayName);
                return;
            }

            classificationAlgorithm.Params = classificationInfo;
            if (AlgoFactory.Instance().ExecuteAlgo(classificationAlgorithm))
            {
                AddOutputLayer();
            }
        }

        private static List<int> BuildBandIndexes(int selectedBandCount)
        {
            var indexes = new List<int>();
            for (var i = 0; i < selectedBandCount; i++)
            {
                indexes.Add(i + 1);
            }

            return indexes;
        }

        private static string SelectInputRaster()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "选择分类影像";
                dialog.Filter = "Raster Files|*.tif;*.tiff;*.img;*.dat|All Files|*.*";
                dialog.Multiselect = false;
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        private static string SelectOutputRaster()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "保存分类结果";
                dialog.Filter = "IMG 文件|*.img|TIF 文件|*.tif";
                dialog.DefaultExt = "img";
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        private IGraphicsLayer FindRoiLayer()
        {
            var layers = m_HookHelper.ActiveView.FocusMap.GetAllLayer();
            foreach (var layer in layers)
            {
                if (layer.Name == "roi_layer")
                {
                    return layer as IGraphicsLayer;
                }
            }

            return null;
        }

        private void FillClassificationParameters(ROIStatistics_Exchange_Info roiParameters, int selectedBandCount)
        {
            var roiCount = roiParameters.vecROIName.Count;
            classificationInfo.ROINums = roiCount;
            classificationInfo.ListRoiNames = roiParameters.vecROIName;
            classificationInfo.ListRoiColors = roiParameters.vecROIColor;
            classificationInfo.ROIMean = new List<double>();
            classificationInfo.ROICof = new List<double>();

            for (var i = 0; i < roiCount; i++)
            {
                for (var j = 0; j < selectedBandCount; j++)
                {
                    classificationInfo.ROIMean.Add(roiParameters.vecMean[i * selectedBandCount + j]);
                }

                for (var k = 0; k < selectedBandCount * selectedBandCount; k++)
                {
                    classificationInfo.ROICof.Add(roiParameters.vecCof[i * selectedBandCount * selectedBandCount + k]);
                }
            }
        }

        private void AddOutputLayer()
        {
            var layer = LayerFactory.CreateDefaultLayer(classificationInfo.OutputFilePath);
            if (layer == null)
            {
                MessageBox.Show("分类结果图层为空。", DisplayName);
                return;
            }

            m_HookHelper.ActiveView.FocusMap.AddLayer(layer);
            m_HookHelper.ActiveView.PartialRefresh(ViewDrawPhaseType.ViewAll);
        }
    }
}
