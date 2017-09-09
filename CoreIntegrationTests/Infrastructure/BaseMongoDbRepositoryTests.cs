﻿using MongoDbGenericRepository.Models;
using Xunit;
using System.Collections.Generic;
using System.Configuration;
using System;

namespace IntegrationTests.Infrastructure
{

    public class BaseMongoDbRepositoryTests<T> : IDisposable where T : Document, new() 
    {
        public T CreateTestDocument()
        {
            return new T();
        }

        public List<T> CreateTestDocuments(int numberOfDocumentsToCreate)
        {
            var docs = new List<T>();
            for(var i = 0; i < numberOfDocumentsToCreate; i++)
            {
                docs.Add(new T());
            }
            return docs;
        }

        /// <summary>
        /// Constructor, init code
        /// </summary>
        public BaseMongoDbRepositoryTests()
        {
            Init();
            var type = CreateTestDocument();
            if (type is IPartitionedDocument)
            {
                PartitionKey = ((IPartitionedDocument)type).PartitionKey;
            }
        }

        public string PartitionKey { get; set; }

        /// <summary>
        /// SUT: System Under Test
        /// </summary>
        protected static ITestRepository SUT { get; set; }

        public void Init()
        {
            SUT = TestRepository.Instance;
        }

        public void Cleanup()
        {
            // We drop the collection at the end of each test session.
            if (!string.IsNullOrEmpty(PartitionKey))
            {
                SUT.DropTestCollection<T>(PartitionKey);
            }
            else
            {
                SUT.DropTestCollection<T>();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Cleanup();
                }

                disposedValue = true;
            }
        }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
        }
        #endregion
    }
}