namespace PieRemoteSensingPlatform.Commands
{
    internal sealed class MaximumLikelihoodClassificationCommand : SupervisedClassificationCommandBase
    {
        protected override string DisplayName
        {
            get { return "最大似然分类"; }
        }

        protected override int ClassifierType
        {
            get { return 0; }
        }

        protected override string AlgorithmClassName
        {
            get { return "PIE.CommonAlgo.MLClassificationAlgo"; }
        }
    }
}
