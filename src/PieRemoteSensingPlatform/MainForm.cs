using System;
using System.Reflection;
using System.Windows.Forms;
using PIE.AxControls;
using PIE.Carto;
using PIE.CommonAlgo;
using PIE.Controls;
using PIE.ControlsUI;
using PIE.DataSource;
using PIE.Geometry;
using PIE.Plugin;
using PIE.SystemAlgo;
using PIE.SystemUI;
using PieRemoteSensingPlatform.Commands;
using PieRemoteSensingPlatform.Models;
using PieRemoteSensingPlatform.Services;

namespace PieRemoteSensingPlatform
{
    public partial class MainForm : Form
    {
        private readonly PieAlgorithmRunner algorithmRunner;
        private readonly BandOperationService bandOperationService;
        private readonly MapControl mapControl;
        private readonly TOCControl tocControl;
        private ILayer selectedLayer;

        public MainForm()
        {
            InitializeComponent();

            algorithmRunner = new PieAlgorithmRunner();
            bandOperationService = new BandOperationService(algorithmRunner);

            tocControl = new TOCControl();
            tocControl.Dock = DockStyle.Fill;
            tocControl.MouseClick += TocControl_MouseClick;
            splitContainer1.Panel1.Controls.Add(tocControl);

            mapControl = new MapControl();
            mapControl.Dock = DockStyle.Fill;
            mapControl.MouseMove += MapControl_MouseMove;
            splitContainer1.Panel2.Controls.Add(mapControl);

            tocControl.SetBuddyControl(mapControl);
            UpdateMapStatus();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateMapStatus();
        }

        private void TocControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            IMap map = null;
            selectedLayer = null;
            var nodeType = PIETOCNodeType.Null;
            object unknown = null;
            object data = null;

            tocControl.HitTest(e.X, e.Y, ref nodeType, ref map, ref selectedLayer, ref unknown, ref data);
            删除图层ToolStripMenuItem.Visible = nodeType != PIETOCNodeType.Map;
            属性ToolStripMenuItem1.Visible = true;
            添加数据ToolStripMenuItem.Visible = true;
            缩放至图层ToolStripMenuItem.Visible = true;
            contextMenuStrip1.Show(tocControl, e.Location);
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            var point = mapControl.ToMapPoint(e.X, e.Y);
            labMapXY.Text = string.Format("X: {0:F4}, Y: {1:F4}", point.X, point.Y);
        }

        private void 添加矢量数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddDataWithDialog("Shape Files|*.shp;*.000|All Files|*.*");
        }

        private void 添加栅格数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunCommand(new RasterCommand());
        }

        private void 清除数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapControl.ClearLayers();
            RefreshMap();
        }

        private void 矢量转栅格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmVectorToRaster(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.VectorToRasterAlgo",
                "VectorToRasterAlgo",
                "矢量转栅格",
                form => form.ExChangeData);
        }

        private void 栅格转矢量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmRasterToVector(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.RasterToVectorAlgo",
                "RasterToVectorAlgo",
                "栅格转矢量",
                form => form.ExChangeData);
        }

        private void 波段合成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunBandCombination();
        }

        private void 辐射定标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmPIECalibration(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.CalibrationAlgo",
                "CalibrationAlgo",
                "辐射定标",
                form => form.ExChangeData);
        }

        private void 大气校正ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmAtmosphericCorrection(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.AtmosphericCorrectionAlgo",
                "AtmosphericCorrectionAlgo",
                "大气校正",
                form => form.ExChangeData);
        }

        private void 内部平均法定标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmCali_InnerMean(),
                "PIE.Plugin.dll",
                "PIE.Plugin.Cali_InnerMeanCommand",
                "CaliInnerMeanCommand",
                "内部平均法定标",
                form => form.ExChangeData);
        }

        private void 正射校正ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmPIEOrtho(),
                "PIE.Plugin.dll",
                "PIE.Plugin.PIEOrthoCommand",
                "PIEOrthoCommand",
                "正射校正",
                form => form.ExChangeData);
        }

        private void 影像裁剪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunImageMosaic("影像裁剪");
        }

        private void 快速拼接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunImageMosaic("快速拼接");
        }

        private void 常用滤波ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmImgProFiltCommon(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.ImgProFiltCommonAlgo",
                "ImgProFiltCommonAlgo",
                "常用滤波",
                form => form.ExChangeData);
        }

        private void 微分锐化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmImgProFiltDiffSharp(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.ImgProFiltDiffSharpAlgo",
                "ImgProFiltDiffSharpAlgo",
                "微分锐化",
                form => form.ExChangeData);
        }

        private void 定向滤波ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmImgProFiltCommon(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.ImgProFiltCommonAlgo",
                "ImgProFiltCommonAlgo",
                "定向滤波",
                form => form.ExChangeData);
        }

        private void 频率域滤波ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmImgProFiltFrequency(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.ImgProFiltFrequencyAlgo",
                "ImgProFiltFrequencyAlgo",
                "频率域滤波",
                form => form.ExChangeData);
        }

        private void iSODATA分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmISODataClassification(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.ISODataClassificationAlgo",
                "ISODataClassificationAlgo",
                "ISODATA 分类",
                form => form.ExChangeData);
        }

        private void kMeans分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDialogAlgorithm(
                () => new FrmKmeansClassification(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.KmeansClassificationAlgo",
                "KmeansClassificationAlgo",
                "KMeans 分类",
                form => form.ExChangeData);
        }

        private void 距离分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunCommand(new DistanceClassificationCommand());
        }

        private void 最大似然分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunCommand(new MaximumLikelihoodClassificationCommand());
        }

        private void 植被盖度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.VegetationCoverage());
        }

        private void 建筑景观聚类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.BuildingLandscape());
        }

        private void pM25监测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunKrigingInterpolation("PM2.5 监测");
        }

        private void 湿度监测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.Wetness());
        }

        private void 水体提取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.WaterExtraction());
        }

        private void 黑臭水体监测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.BlackOdorousWater());
        }

        private void 土壤盐度指数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.SoilSalinity());
        }

        private void 土壤强度指数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.SoilStrength());
        }

        private void 热力图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunHeatMap();
        }

        private void 绿度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.Greenness());
        }

        private void 湿度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.Wetness());
        }

        private void 干度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteIndex(RemoteSensingIndexDefinition.Dryness());
        }

        private void 热度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFeaturePlaceholder("热度");
        }

        private void 波段合成ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RunBandCombination();
        }

        private void rSEIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunPcaForward("RSEI 主成分分析");
        }

        private void 添加数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddDataWithDialog("Shape Files|*.shp;*.000|Raster Files|*.tif;*.tiff;*.dat;*.bmp;*.img;*.jpg|HDF Files|*.hdf;*.h5|NC Files|*.nc|All Files|*.*");
        }

        private void 属性ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RunCommand(new LayerPropertyCommand());
        }

        private void 删除图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedLayer == null)
            {
                return;
            }

            mapControl.FocusMap.DeleteLayer(selectedLayer);
            RefreshMap();
        }

        private void 缩放至图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunCommand(new ZoomToLayerCommand());
        }

        private void toolStripButton1_Click_2(object sender, EventArgs e)
        {
            SetCurrentTool(new MapZoomInTool());
        }

        private void toolStripButton2_Click_2(object sender, EventArgs e)
        {
            SetCurrentTool(new MapZoomOutTool());
        }

        private void toolStripButton3_Click_2(object sender, EventArgs e)
        {
            SetCurrentTool(new PanTool());
        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            RunCommand(new FullExtentCommand());
        }

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            RunCommand(new NewProjectCommand());
        }

        private void toolStripButton7_Click_1(object sender, EventArgs e)
        {
            RunCommand(new SaveProjectCommand());
        }

        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {
        }

        private void toolStripDropDownButton6_Click(object sender, EventArgs e)
        {
        }

        private void toolStripDropDownButton8_Click(object sender, EventArgs e)
        {
        }

        private void toolStripDropDownButton10_Click(object sender, EventArgs e)
        {
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void splitContainer1_Panel1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void mapCtrl_MouseMove(object sender, MouseEventArgs e)
        {
            MapControl_MouseMove(sender, e);
        }

        private void RunImageMosaic(string description)
        {
            RunDialogAlgorithm(
                () => new FrmImageFastMosaic(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.ImageMosaicParamAlgo",
                "ImageMosaicParamAlgo",
                description,
                form => form.ExChangeData);
        }

        private void RunBandCombination()
        {
            RunDialogAlgorithm(
                () => new FrmBandCombination(mapControl.FocusMap),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.BandCombinationAlgo",
                "BandCombinationAlgo",
                "波段合成",
                form => form.m_AlgoParam);
        }

        private void RunKrigingInterpolation(string description)
        {
            RunDialogAlgorithm(
                () => new FrmKrigingInterpolation(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.KrigingInterpolationAlgo",
                "KrigingInterpolationAlgo",
                description,
                form => form.m_ExchangeInfo);
        }

        private void RunPcaForward(string description)
        {
            RunDialogAlgorithm(
                () => new FrmPCA(),
                "PIE.CommonAlgo.dll",
                "PIE.CommonAlgo.TransformForwardPCAAlgo",
                "TransformForwardPCAAlgo",
                description,
                form => form.ExChangeData);
        }

        private void RunDialogAlgorithm<TForm>(
            Func<TForm> createForm,
            string assemblyName,
            string className,
            string algorithmName,
            string description,
            Func<TForm, object> getParameters)
            where TForm : Form
        {
            RunSafely(() =>
            {
                using (var form = createForm())
                {
                    if (form.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    var algorithm = algorithmRunner.CreateAlgorithm(
                        assemblyName,
                        className,
                        algorithmName,
                        description,
                        getParameters(form));

                    AttachProgressAndCompletion(algorithm);
                    algorithmRunner.ExecuteAsynchronously(algorithm);
                }
            });
        }

        private void ExecuteIndex(RemoteSensingIndexDefinition definition)
        {
            RunSafely(() =>
            {
                var inputFilePath = SelectInputRaster();
                if (string.IsNullOrWhiteSpace(inputFilePath))
                {
                    return;
                }

                var outputFilePath = SelectOutputRaster(definition.Key);
                if (string.IsNullOrWhiteSpace(outputFilePath))
                {
                    return;
                }

                MessageBox.Show("开始计算：" + definition.DisplayName, "波段运算");
                var algorithm = bandOperationService.CreateIndexAlgorithm(definition, inputFilePath, outputFilePath);
                AttachProgress(algorithm);
                algorithmRunner.ExecuteSynchronously(algorithm);
                DetachProgress(algorithm);

                ShowLayer(inputFilePath);
                ShowLayer(outputFilePath);
                ResetProgress();
                MessageBox.Show("计算完成：" + definition.DisplayName, "波段运算");
            });
        }

        private void RunHeatMap()
        {
            RunSafely(() =>
            {
                var inputFilePath = SelectVectorInput("选择点要素数据");
                if (string.IsNullOrWhiteSpace(inputFilePath))
                {
                    return;
                }

                var outputFilePath = SelectOutputRaster("heat-map");
                if (string.IsNullOrWhiteSpace(outputFilePath))
                {
                    return;
                }

                var featureDataset = DatasetFactory.OpenFeatureDataset(inputFilePath);
                if (featureDataset == null || featureDataset.GetGeomType() != GeometryType.GeometryPoint)
                {
                    MessageBox.Show("热力图输入数据必须是点要素。", "热力图");
                    return;
                }

                var extent = featureDataset.GetExtent();
                var parameters = new HotMapContruct_Exchange_Info
                {
                    InputFeatureDataset = featureDataset,
                    BUseWeightFiled = false,
                    Radius = 20,
                    BCreateFeatureDataset = false,
                    DeviceWidth = 1000,
                    DefualtWeightValue = 50,
                    OutRasterType = "GTIFF",
                    CellSize = extent.GetWidth() / 1000,
                    OutRasterFilePath = outputFilePath
                };

                var algorithm = new HotMapContructAlgo { Params = parameters };
                algorithm.Execute();
                ShowLayer(outputFilePath);
            });
        }

        private void AddDataWithDialog(string filter)
        {
            RunSafely(() =>
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Title = "添加数据";
                    dialog.Filter = filter;
                    dialog.Multiselect = false;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        ShowLayer(dialog.FileName);
                    }
                }
            });
        }

        private void ShowLayer(string filePath)
        {
            var layer = LayerFactory.CreateDefaultLayer(filePath);
            if (layer == null)
            {
                MessageBox.Show("无法加载图层：" + filePath, "图层加载");
                return;
            }

            mapControl.AddLayer(layer, 0);
            RefreshMap();
        }

        private void RunCommand(ICommand command)
        {
            RunSafely(() =>
            {
                command.OnCreate(mapControl);
                command.OnClick();
                RefreshMap();
            });
        }

        private void SetCurrentTool(object tool)
        {
            RunSafely(() =>
            {
                var command = tool as ICommand;
                if (command == null)
                {
                    return;
                }

                command.OnCreate(mapControl);
                mapControl.CurrentTool = (dynamic)tool;
            });
        }

        private void AttachProgressAndCompletion(ISystemAlgo algorithm)
        {
            AttachProgress(algorithm);
            var events = algorithm as ISystemAlgoEvents;
            if (events != null)
            {
                events.OnExecuteCompleted += Algorithm_OnExecuteCompleted;
            }
        }

        private void AttachProgress(ISystemAlgo algorithm)
        {
            var events = algorithm as ISystemAlgoEvents;
            if (events != null)
            {
                events.OnProgressChanged += Algorithm_OnProgressChanged;
            }
        }

        private void DetachProgress(ISystemAlgo algorithm)
        {
            var events = algorithm as ISystemAlgoEvents;
            if (events != null)
            {
                events.OnProgressChanged -= Algorithm_OnProgressChanged;
            }
        }

        private int Algorithm_OnProgressChanged(double complete, string message, ISystemAlgo algorithm)
        {
            progressBar.Value = Math.Max(0, Math.Min(100, Convert.ToInt32(complete)));
            labProMsg.Text = message;
            return 1;
        }

        private void Algorithm_OnExecuteCompleted(ISystemAlgo algorithm)
        {
            var events = algorithm as ISystemAlgoEvents;
            if (events != null)
            {
                events.OnProgressChanged -= Algorithm_OnProgressChanged;
                events.OnExecuteCompleted -= Algorithm_OnExecuteCompleted;
            }

            ResetProgress();
            var errorCode = -1;
            var errorMessage = string.Empty;
            algorithm.GetErrorInfo(ref errorCode, ref errorMessage);
            if (errorCode != 0)
            {
                MessageBox.Show(algorithm.Name + " 执行失败：" + errorMessage, "PIE 算法");
                return;
            }

            var outputFilePath = TryGetOutputFilePath(algorithm.Params);
            if (!string.IsNullOrWhiteSpace(outputFilePath))
            {
                ShowLayer(outputFilePath);
            }
        }

        private static string TryGetOutputFilePath(object parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            var type = parameters.GetType();
            var property = type.GetProperty("OutputFilePath", BindingFlags.Public | BindingFlags.Instance)
                ?? type.GetProperty("OutRasterFilePath", BindingFlags.Public | BindingFlags.Instance)
                ?? type.GetProperty("OutputFile", BindingFlags.Public | BindingFlags.Instance);

            return property == null ? null : property.GetValue(parameters, null) as string;
        }

        private static string SelectInputRaster()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "选择遥感影像";
                dialog.Filter = "Raster Files|*.tif;*.tiff;*.dat;*.img|All Files|*.*";
                dialog.Multiselect = false;
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        private static string SelectVectorInput(string title)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = title;
                dialog.Filter = "Shape Files|*.shp|All Files|*.*";
                dialog.Multiselect = false;
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        private static string SelectOutputRaster(string defaultName)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "保存输出栅格";
                dialog.Filter = "TIF 文件|*.tif";
                dialog.DefaultExt = "tif";
                dialog.FileName = defaultName + ".tif";
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        private void RefreshMap()
        {
            mapControl.PartialRefresh(ViewDrawPhaseType.ViewAll);
            UpdateMapStatus();
        }

        private void UpdateMapStatus()
        {
            if (mapControl == null || mapControl.FocusMap == null)
            {
                return;
            }

            labMapName.Text = mapControl.FocusMap.Name;
            labLayerCount.Text = "图层数：" + mapControl.FocusMap.LayerCount;
        }

        private void ResetProgress()
        {
            progressBar.Value = 0;
            labProMsg.Text = string.Empty;
        }

        private static void ShowFeaturePlaceholder(string featureName)
        {
            MessageBox.Show("当前展示版本保留了“" + featureName + "”菜单入口，尚未接入稳定算法。", featureName);
        }

        private static void RunSafely(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "执行失败");
            }
        }
    }
}
