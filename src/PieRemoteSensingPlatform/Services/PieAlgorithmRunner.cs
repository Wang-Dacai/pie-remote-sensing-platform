using System;
using PIE.SystemAlgo;

namespace PieRemoteSensingPlatform.Services
{
    public sealed class PieAlgorithmRunner
    {
        public ISystemAlgo CreateAlgorithm(string assemblyName, string className, string name, string description, object parameters)
        {
            var algorithm = AlgoFactory.Instance().CreateAlgo(assemblyName, className);
            if (algorithm == null)
            {
                throw new InvalidOperationException("无法创建 PIE 算法：" + className);
            }

            algorithm.Name = name;
            algorithm.Description = description;
            algorithm.Params = parameters;
            return algorithm;
        }

        public bool ExecuteSynchronously(ISystemAlgo algorithm)
        {
            if (algorithm == null)
            {
                throw new ArgumentNullException("algorithm");
            }

            return AlgoFactory.Instance().ExecuteAlgo(algorithm);
        }

        public void ExecuteAsynchronously(ISystemAlgo algorithm)
        {
            if (algorithm == null)
            {
                throw new ArgumentNullException("algorithm");
            }

            AlgoFactory.Instance().AsynExecuteAlgo(algorithm);
        }
    }
}
