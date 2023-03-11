namespace InformationRetrieval
{
    public class InvertedIndex
    {
        private readonly string[] _boolOperators = new string[] { "and", "or" };

        private readonly List<string> _docs;
        private readonly List<string> _queries;
        private readonly List<string> _stopWords;

        private Dictionary<int, List<string>> _documentDics = new Dictionary<int, List<string>>();
        private List<string> _termList = new List<string>();
        private Dictionary<string, List<int>> _postingListDics = new Dictionary<string, List<int>>();
        private List<string> _filteredQuery = new List<string>();

        public InvertedIndex(List<string> docs, List<string> queries, List<string> stopWords)
        {
            _docs = docs;
            _queries = queries;
            _stopWords = stopWords;
        }

        public void GetResultAtQueryNumber(int queryNumber, int stepSkip = 0)
        {
            PrepareData(queryNumber);
            IndicatePostingList();

            var resultMatrix = ProcessInvertedIndex(stepSkip);

            Console.WriteLine(string.Join("\t", resultMatrix));
        }

        private List<int> ProcessInvertedIndex(int stepSkip)
        {
            bool hasFirstTerm = false;
            var boolOps = string.Empty;
            var resultMatrix = new List<int>();

            for (int i = 0; i < _filteredQuery.Count; i++)
            {
                var term = _filteredQuery[i];
                if (!hasFirstTerm)
                {
                    resultMatrix = _postingListDics[term];
                    hasFirstTerm = true;
                }
                else if (_boolOperators.Contains(term))
                {
                    boolOps = term;
                }
                else if (!string.IsNullOrEmpty(boolOps))
                {
                    if (boolOps == "and")
                    {
                        resultMatrix = ProcessIntersect(resultMatrix, _postingListDics[term], stepSkip);
                    }

                    boolOps = string.Empty;
                }
                else
                {
                    resultMatrix = ProcessIntersect(resultMatrix, _postingListDics[term], stepSkip);
                }
            }

            return resultMatrix;
        }

        private List<int> ProcessIntersect(List<int> postingList1, List<int> postingList2, int skipStep)
        {
            if (skipStep == 0)
            {
                return Intersect(postingList1, postingList2);
            }
            else
            {
                return IntersectWithSkip(postingList1, postingList2, skipStep);
            }
        }

        private List<int> Intersect(List<int> postingList1, List<int> postingList2)
        {
            var result = new List<int>();
            int p1 = 0;
            int p2 = 0;

            while (p1 < postingList1.Count && p2 < postingList2.Count)
            {
                var docId1 = postingList1[p1];
                var docId2 = postingList2[p2];

                if (docId1 == docId2)
                {
                    result.Add(docId1);
                    p1++;
                    p2++;
                }
                else if (docId1 < docId2)
                {
                    p1++;
                }
                else
                {
                    p2++;
                }
            }

            return result;
        }

        private List<int> IntersectWithSkip(List<int> postingList1, List<int> postingList2, int skipStep)
        {
            var result = new List<int>();
            int p1 = 0;
            int p2 = 0;

            postingList1.Sort();
            postingList2.Sort();

            while (p1 < postingList1.Count && p2 < postingList2.Count)
            {
                var docId1 = postingList1[p1];
                var docId2 = postingList2[p2];

                if (docId1 == docId2)
                {
                    result.Add(docId1);
                    p1++;
                    p2++;
                }
                else if (docId1 < docId2)
                {
                    if ((p1 + skipStep) < postingList1.Count && postingList1[p1 + skipStep] <= docId2)
                    {
                        while ((p1 + skipStep) < postingList1.Count &&postingList1[p1 + skipStep] <= docId2)
                        {
                            p1 += skipStep;
                        }
                    }
                    else
                    {
                        p1++;
                    }
                }
                else if ((p2 + skipStep) < postingList2.Count && postingList2[p2 + skipStep] <= docId1)
                {
                    while ((p2 + skipStep) < postingList2.Count && postingList2[p2 + skipStep] <= docId1)
                    {
                        p2 += skipStep;
                    }
                }
                else
                {
                    p2++;
                }
            }

            return result;
        }

        private void IndicatePostingList()
        {
            foreach (var term in _filteredQuery)
            {
                var postingList = new List<int>();

                foreach (var keyValue in _documentDics)
                {
                    if (keyValue.Value.Contains(term))
                    {
                        postingList.Add(keyValue.Key);
                    }
                }

                _postingListDics.Add(term, postingList);
            }
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
    }
}
