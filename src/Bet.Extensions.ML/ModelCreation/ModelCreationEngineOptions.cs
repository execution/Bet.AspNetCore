﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bet.Extensions.ML.ModelStorageProviders;

using Microsoft.Extensions.Logging;

namespace Bet.Extensions.ML.ModelCreation
{
    public class ModelCreationEngineOptions<TInput, TResult>
        where TInput : class
        where TResult : MetricsResult
    {
        public string ModelName { get; set; } = string.Empty;

        public Func<IModelStorageProvider, ModelStorageProviderOptions, CancellationToken, Task<IEnumerable<TInput>>> DataLoader { get; set; }
            = async (storageProvider, storageOptions, cancellationToken) =>
        {
            return await storageProvider.LoadRawDataAsync<TInput>(storageOptions.ModelSourceFileName, cancellationToken);
        };

        public Func<IModelDefinitionBuilder<TInput, TResult>, IEnumerable<TInput>, ILogger, TResult> TrainModelConfigurator { get; set; }
            = (modelBuilder, data, logger) =>
        {
            // 1. load ML data set
            modelBuilder.LoadData(data);

            // 2. build data view
            modelBuilder.BuildDataView();

            // 3. build training pipeline
            var buildTrainingPipelineResult = modelBuilder.BuildTrainingPipeline();

            // 4. evaluate quality of the pipeline
            var evaluateResult = modelBuilder.Evaluate();
            logger.LogInformation(evaluateResult.ToString());

            // 5. train the model
            var trainModelResult = modelBuilder.TrainModel();

            return evaluateResult;
        };

        public Func<IModelDefinitionBuilder<TInput, TResult>, ILogger, CancellationToken, Task>? ClassifyTestConfigurator { get; set; }
    }
}
