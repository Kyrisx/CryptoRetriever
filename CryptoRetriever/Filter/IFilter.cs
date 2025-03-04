﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoRetriever.Filter {
    public interface IFilter {
        /// <summary>
        /// Gets a summary of this filter and its parameters.
        /// </summary>
        /// <returns>Returns the summary.</returns>
        String Summary { get; }

        /// <summary>
        /// Filters the given input and returns a new dataset with the
        /// filtered results.
        /// </summary>
        /// <param name="input">The input to filter.</param>
        /// <returns>A new dataset with the filtered results.</returns>
        Dataset Filter(Dataset input);
    }
}
