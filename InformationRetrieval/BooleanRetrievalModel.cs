using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval
{
    public class BooleanRetrievalModel
    {
        private readonly string[] _boolOperators = new string[] { "and", "or" };

        private readonly List<string> _docs;
        private readonly List<string> _queries;
        private readonly List<string> _stopWords;

        private Dictionary<int, List<string>> _documentDics = new Dictionary<int, List<string>>();
        private List<string> _termList = new List<string>();
        private Dictionary<string, List<int>>  _boolDics = new Dictionary<string, List<int>>();

        private List<string> _filteredQuery = new List<string>();

        public BooleanRetrievalModel(List<string> docs, List<string> queries, List<string> stopWords)
        {
            _docs = docs;
            _queries = queries;
            _stopWords = stopWords;
        }

        public void GetResultAtQueryNumber(int queryNumber)
        {
            PrepareData(queryNumber);
            IndicateMatrix();

            var resultMatrix = ProcessBooleanRetrieval();
            var docIDs = GetDocIDsFromResultMatrix(resultMatrix);

            Console.WriteLine(string.Join("\t", docIDs));
        }

        private List<int> GetDocIDsFromResultMatrix(List<int> resultMatrix)
        {
            var docIDs = new List<int>();

            for (int i = 0; i < resultMatrix.Count; i++)
            {
                if (resultMatrix[i] == 1)
                {
                    docIDs.Add(_documentDics.Keys.ElementAt(i));
                }
            }

            return docIDs;
        }

        private List<int> ProcessBooleanRetrieval()
        {
            bool hasFirstTerm = false;
            var boolOps = string.Empty;
            var resultMatrix = new List<int>();

            for (int i = 0; i < _filteredQuery.Count; i++)
            {
                var term = _filteredQuery[i];
                if (!hasFirstTerm)
                {
                    resultMatrix = _boolDics[term];
                    hasFirstTerm = true;
                }
                else if (_boolOperators.Contains(term))
                {
                    boolOps = term;
                }
                else if (!string.IsNullOrEmpty(boolOps))
                {
                    if (boolOps == "or")
                    {
                        resultMatrix = ProcessQueryOr(resultMatrix, _boolDics[term]);
                    }
                    else if (boolOps == "and")
                    {
                        resultMatrix = ProcessQueryAnd(resultMatrix, _boolDics[term]);
                    }

                    boolOps = string.Empty;
                }
                else
                {
                    resultMatrix = ProcessQueryOr(resultMatrix, _boolDics[term]);
                }
            }

            return resultMatrix;
        }

        private void PrepareData(int queryNumber)
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

                foreach (var term in content)
                {
                    if (!_termList.Contains(term))
                    {
                        _termList.Add(term);
                    }
                }

                _documentDics.Add(title, content);
            }

            _termList.Sort();

            // Filter term that not in termList
            queryNumber = queryNumber < _queries.Count ? queryNumber : 0;
            List<string> query = RemoveStopwords(_queries[queryNumber].Trim().ToLower().Split('\n')[1].Trim().Split(' '));
            _filteredQuery = FilterQueryTerm(query.ToArray());
        }

        private void IndicateMatrix()
        {
            foreach (var term in _filteredQuery)
            {
                var boolList = new List<int>();

                foreach (var content in _documentDics.Values)
                {
                    boolList.Add(content.Contains(term) ? 1 : 0);
                }

                _boolDics.Add(term, boolList);
            }
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

        private List<string> FilterQueryTerm(string[] query)
        {
            List<string> result = new List<string>();

            foreach (string term in query)
            {
                if (_termList.Contains(term) || _boolOperators.Contains(term))
                {
                    result.Add(term);
                }
            }

            return result;
        }

        private List<int> ProcessQueryAnd(List<int> boolMatrix1, List<int> boolMatrix2)
        {
            var result = new List<int>();

            for (int i = 0; i < boolMatrix1.Count; i++)
            {
                result.Add(boolMatrix1[i] & boolMatrix2[i]);
            }

            return result;
        }

        private List<int> ProcessQueryOr(List<int> boolMatrix1, List<int> boolMatrix2)
        {
            var result = new List<int>();

            for (int i = 0; i < boolMatrix1.Count; i++)
            {
                result.Add(boolMatrix1[i] | boolMatrix2[i]);
            }

            return result;
        }
    }
}
