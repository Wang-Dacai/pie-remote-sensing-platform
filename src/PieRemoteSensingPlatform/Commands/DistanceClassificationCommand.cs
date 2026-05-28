namespace PieRemoteSensingPlatform.Commands
{
    internal sealed class DistanceClassificationCommand : SupervisedClassificationCommandBase
    {
        protected override string DisplayName
        {
            get { return "距离分类"; }
        }

        protected override int ClassifierType
        {
            get { return 1; }
        }

        protected override string AlgorithmClassName
        {
            get { return "PIE.CommonAlgo.DistanceClassificationAlgo"; }
        }
    }
}
