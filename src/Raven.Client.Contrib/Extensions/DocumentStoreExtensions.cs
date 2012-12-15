﻿using System;
using System.Diagnostics;
using Raven.Abstractions.Data;

namespace Raven.Client.Contrib.Extensions
{
    public static class DocumentStoreExtensions
    {
        public static void AssertDocumentStoreErrors(this IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException("documentStore");
            }

            ServerError[] errors = documentStore.DatabaseCommands.GetStatistics().Errors;
            if (errors == null || errors.Length <= 0)
            {
                return;
            }

            // We have some Errors. NOT. GOOD. :(
            foreach (ServerError serverError in errors)
            {
                string errorMessage = string.Format("Document: {0}; Index: {1}; Error: {2}",
                                                    string.IsNullOrEmpty(serverError.Document)
                                                        ? "No Document Id"
                                                        : serverError.Document,
                                                    string.IsNullOrEmpty(serverError.Index)
                                                        ? "No Index"
                                                        : serverError.Index,
                                                    string.IsNullOrEmpty(serverError.Error)
                                                        ? "No Error message .. err??"
                                                        : serverError.Error);

                Debug.WriteLine(errorMessage);
            }

            throw new InvalidOperationException("DocumentStore has some errors. Dast is nict gut.");
        }
    }
}