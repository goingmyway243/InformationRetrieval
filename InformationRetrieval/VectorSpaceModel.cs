using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval
{
    public class VectorSpaceModel
    {
        private readonly List<string> _docs;
        private readonly List<string> _stopWords;

        private Dictionary<int, List<string>> _documentDics = new Dictionary<int, List<string>>();
        private Dictionary<string, double> _queryVectorDics = new Dictionary<string, double>();
        private Dictionary<int, double> _resultDics = new Dictionary<int, double>();

        private List<string> _filteredQuery = new List<string>();

        public VectorSpaceModel(List<string> docs, List<string> stopWords)
        {
            _docs = docs;
            _stopWords = stopWords;
        }

        public void GetValueFromQuery(string query)
        {
            PrepareData(query);
            ProcessVectorSpaceModel();

            _resultDics.OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
            foreach (var res in _resultDics)
            {
                Console.WriteLine($"{res.Key}: {res.Value}");
            }
        }

        private void PrepareData(string query)
        {
            // Prepare termList and documentDics
            foreach (string doc in _docs)
            {
                var cleanDoc = doc.Trim();
                if (string.IsNullOrWhiteSpace(cleanDoc))
                {
                    break;
                }

                var tmp = cleanDoc.Split('\n');

                var title = int.Parse(tmp[0].Trim());
                var content = RemoveStopwords(cleanDoc.Substring(tmp[0].Length).Trim().ToLower().Split(' '));

                _documentDics.Add(title, content);
            }

            // Filter term that not in termList
            _filteredQuery = RemoveStopwords(query.Trim().Split(' '));
        }

        private void ProcessVectorSpaceModel()
        {
            // indicate query vector
            foreach (var term in _filteredQuery)
            {
                IndicateVector(ref _queryVectorDics, _filteredQuery, term);
            }

            // calculate Cosine Similarity for each document
            foreach (var doc in _documentDics)
            {
                // indicate doc vector
                var docVectorDics = new Dictionary<string, double>();
                foreach (var term in _filteredQuery)
                {
                    IndicateVector(ref docVectorDics, doc.Value, term);
                }

                _resultDics.Add(doc.Key, CalculateCosineSimilarity(docVectorDics, _queryVectorDics));
            }
        }

        private void IndicateVector(ref Dictionary<string, double> vectorDics, List<string> doc, string term)
        {
            if (!vectorDics.ContainsKey(term))
            {
                vectorDics.Add(term, CalculateTF(doc, term) * CalculateIdf(term));
            }
        }

        private double CalculateCosineSimilarity(Dictionary<string, double> docVectorDics, Dictionary<string, double> queryVectorDics)
        {
            double sumDQ = 0;
            double sumD2 = 0;
            double sumQ2 = 0;

            foreach (var term in queryVectorDics.Keys)
            {
                sumDQ += docVectorDics[term] * queryVectorDics[term];
                sumD2 += Math.Pow(docVectorDics[term], 2);
                sumQ2 += Math.Pow(queryVectorDics[term], 2);
            }

            if (sumD2 == 0 || sumQ2 == 0)
            {
                return 0;
            }

            return sumDQ / (Math.Sqrt(sumD2) * Math.Sqrt(sumQ2));
        }

        private double CalculateTF(List<string> doc, string term)
        {
            double count = 0;

            foreach (string word in doc)
            {
                if (word == term)
                {
                    count++;
                }
            }

            if (count == 0)
            {
                return 0;
            }

            return 1 + Math.Log10(count / doc.Count);
        }

        private double CalculateIdf(string term)
        {
            int count = 0;

            foreach (var doc in _documentDics.Values)
            {
                if (doc.Contains(term))
                {
                    count++;
                }
            }

            return Math.Log(_documentDics.Count / (1 + count));
        }

        private List<string> RemoveStopwords(string[] content)
        {
            List<string> removed = new List<string>();

            foreach (string term in content)
            {
                if (!_stopWords.Contains(term))
                {
                    removed.Add(term);
                }
            }

            return removed;
        }
    }
}
